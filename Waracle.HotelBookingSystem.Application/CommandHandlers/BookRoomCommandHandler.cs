using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using System.Threading;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Application.Helpers;
using Waracle.HotelBookingSystem.Data.Repositories;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    /// <summary>
    /// The command handler responsible for processing room booking requests.
    /// </summary>
    public class BookRoomCommandHandler : IRequestHandler<BookRoomCommand, Booking>
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IRoomsRepository _roomsRepository;
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger<BookRoomCommandHandler> _logger;

        private readonly Random _random = new();
        private static long _counter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRoomCommandHandler"/> class.
        /// </summary>
        /// <param name="hotelsRepository">The repository used to access bookings data and perform booking operations.</param>
        /// /// <param name="hotelsRepository">The repository used to access rooms data and perform booking operations.</param>
        /// <param name="logger">The logger used to log information and errors related to the booking process.</param>
        public BookRoomCommandHandler(
            IBookingsRepository bookingsRepository,
            IRoomsRepository roomsRepository,
            IHotelsRepository hotelsRepository,
            ILogger<BookRoomCommandHandler> logger)
        {
            _bookingsRepository = bookingsRepository ?? throw new ArgumentNullException(nameof(bookingsRepository));
            _roomsRepository = roomsRepository ?? throw new ArgumentNullException(nameof(roomsRepository));
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

                var allRooms = await _roomsRepository
                    .GetAllAsync(cancellationToken).ConfigureAwait(false);

                var availableRooms = allRooms.GetAvailableRooms(request.NumberOfGuests, request.CheckInDate, request.CheckOutDate);

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

                if (await IsRoomAvailable(request.Room.Id, request.CheckInDate, request.CheckOutDate, cancellationToken))
                {
                    booking.Reference = GenerateBookingReference();

                    await _bookingsRepository.CreateAsync(booking, cancellationToken).ConfigureAwait(false);

                    _logger.LogInformation($"Booking created successfully for room {request.Room.Id}.");

                    return booking;
                }
                 
                throw new Exception("Room not available");
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

            var hotel = await _hotelsRepository.GetByIdAsync(request.HotelId, cancellationToken).ConfigureAwait(false);
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

        /// <summary>
        /// Generates a unique hotel booking reference.
        /// </summary>
        /// <returns>A unique booking reference string.</returns>
        private string GenerateBookingReference()
        {
            DateTime now = DateTime.UtcNow;
            string datePart = now.ToString("yyyyMMdd-HHmmss");

            long uniqueId = Interlocked.Increment(ref _counter);
            string idPart = uniqueId.ToString("D3");

            return $"{datePart}-{idPart}";
        }

        private async Task<bool> IsRoomAvailable(int roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
        {
            try
            {
                var res = await _bookingsRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

                return !res.Any(b => b.RoomId == roomId && (b.CheckInDate < checkOut && b.CheckOutDate > checkIn));
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while checking room availability", e);
            }
        }
    }
}