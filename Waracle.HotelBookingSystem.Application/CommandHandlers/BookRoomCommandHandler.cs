using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    /// <summary>
    /// The command handler responsible for processing room booking requests.
    /// </summary>
    public class BookRoomCommandHandler : IRequestHandler<BookRoomCommand, Booking>
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger<BookRoomCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRoomCommandHandler"/> class.
        /// </summary>
        /// <param name="hotelsRepository">The repository used to access hotel data and perform booking operations.</param>
        /// <param name="logger">The logger used to log information and errors related to the booking process.</param>
        public BookRoomCommandHandler(IHotelsRepository hotelsRepository, ILogger<BookRoomCommandHandler> logger)
        {
            _hotelsRepository = hotelsRepository ?? throw new ArgumentNullException(nameof(hotelsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the booking of a room by creating a new booking record.
        /// </summary>
        /// <param name="request">The command request containing details of the room booking.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the booking operation.</param>
        /// <returns>A <see cref="Booking"/> object representing the newly created booking.</returns>
        public async Task<Booking> Handle(BookRoomCommand request, CancellationToken cancellationToken)
        {
            await CheckForArgumentExceptions(request, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"Starting booking process for room {request.Room.Id} from {request.CheckInDate} to {request.CheckOutDate}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var availableRooms = await _hotelsRepository.GetAvailableRoomsInHotelAsync(
                    request.HotelId,
                    request.CheckInDate,
                    request.CheckOutDate,
                    request.NumberOfGuests,
                    cancellationToken)
                    .ConfigureAwait(false);

                if (!availableRooms.Any(r => r.Id == request.Room.Id))
                {
                    _logger.LogError($"Room {request.Room.Id} is not available from {request.CheckInDate} to {request.CheckOutDate}.");

                    throw new InvalidOperationException("The selected room is not available for the specified dates.");
                }

                var booking = new Booking
                {
                    Room = request.Room,
                    CheckInDate = request.CheckInDate,
                    CheckOutDate = request.CheckOutDate,
                    NumberOfGuests = request.NumberOfGuests
                };

                await _hotelsRepository.AddBookingAsync(booking, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Booking created successfully for room {request.Room.Id}.");

                return booking;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Booking operation for room {request.Room} was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while booking room {Room}.", request.Room);

                throw;
            }
        }

        private async Task CheckForArgumentExceptions(BookRoomCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Room);

            if (request.NumberOfGuests <= 0)
            {
                _logger.LogError($"Number of guests {request.NumberOfGuests} is not valid.");

                throw new ArgumentException("Number of guests must be greater than zero.");
            }

            if (request.NumberOfGuests > request.Room.RoomType.Capacity)
            {
                _logger.LogError($"Number of guests {request.NumberOfGuests} exceeds room capacity {request.Room.RoomType.Capacity}.");

                throw new ArgumentException("Number of guests exceeds room capacity.");
            }

            if (request.IsCheckoutDateAfterCheckInDate() == false)
            {
                _logger.LogError($"Check out date {request.CheckOutDate} is not after check in date {request.CheckInDate}.");

                throw new ArgumentException("Check out date must be after check in date.");
            }

            var hotel = await _hotelsRepository.GetHotelById(request.HotelId, cancellationToken).ConfigureAwait(false);
            if (hotel == null)
            {
                _logger.LogError($"Hotel with Id {request.HotelId} does not exist.");

                throw new ArgumentException($"HotelId {request.HotelId} does not exist.");
            }

            if (hotel.Rooms.All(r => r.Id != request.Room.Id))
            {
                _logger.LogError($"Room with Id {request.Room.Id} does not exist in Hotel with Id {request.HotelId}.");

                throw new ArgumentException($"RoomId {request.Room.Id} does not exist in Hotel with Id {request.HotelId}.");
            }
        }
    }
}