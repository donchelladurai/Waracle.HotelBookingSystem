using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using System.Threading;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Common;
using Waracle.HotelBookingSystem.Data.Repositories;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    /// <summary>
    /// The command handler responsible for processing room booking requests.
    /// </summary>
    public class BookRoomCommandHandler : IRequestHandler<BookRoomCommand, BookRoomCommandResult>
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
        /// <param name="command">The command request containing details of the room booking.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the booking operation.</param>
        /// <returns>A <see cref="BookRoomCommandResult"/> object representing the changed state.</returns>
        public async Task<BookRoomCommandResult> Handle(BookRoomCommand command, CancellationToken cancellationToken)
        {
            await CheckForArgumentExceptions(command, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"Starting booking process for room {command.RoomId} from {command.CheckInDate} to {command.CheckOutDate}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var allRooms = await _roomsRepository.GetByHotelIdAsync(command.HotelId, cancellationToken).ConfigureAwait(false);

                var availableRooms = allRooms.GetAvailableRooms(command.NumberOfGuests, command.CheckInDate, command.CheckOutDate);

                if (!availableRooms.Any(r => r.Id == command.RoomId))
                {
                    _logger.LogError($"Room {command.RoomId} is not available from {command.CheckInDate} to {command.CheckOutDate}.");

                    return new BookRoomCommandResult(false, true, string.Empty);
                }

                var booking = new Booking()
                {
                    Reference = GenerateBookingReference(),
                    Room = availableRooms.Single(r => r.Id == command.RoomId),
                    CheckInDate = command.CheckInDate.ToUniversalTime(),
                    CheckOutDate = command.CheckOutDate.ToUniversalTime(),
                    NumberOfGuests = command.NumberOfGuests
                };

                await _bookingsRepository.CreateAsync(booking, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Booking created successfully for room {command.RoomId}.");

                return new BookRoomCommandResult(true, false, booking.Reference);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Booking operation for room {command.RoomId} was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while booking room {Room}.", command.RoomId);

                throw;
            }
        }

        private async Task CheckForArgumentExceptions(BookRoomCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(cancellationToken);

            if (command.NumberOfGuests <= 0)
            {
                _logger.LogError($"Number of guests {command.NumberOfGuests} is not valid.");

                throw new ArgumentOutOfRangeException("Number of guests must be greater than zero.");
            }

            if (command.IsCheckoutDateAfterCheckInDate() == false)
            {
                _logger.LogError($"Check out date {command.CheckOutDate} is not after check in date {command.CheckInDate}.");

                throw new ArgumentException("Check out date must be after check in date.");
            }

            var hotel = await _hotelsRepository.GetByIdAsync(command.HotelId, cancellationToken).ConfigureAwait(false);
            if (hotel == null)
            {
                _logger.LogError($"Hotel with Id {command.HotelId} does not exist.");

                throw new ArgumentException($"HotelId {command.HotelId} does not exist.");
            }

            if (hotel.Rooms.All(r => r.Id != command.RoomId))
            {
                _logger.LogError($"Room with Id {command.RoomId} does not exist in Hotel with Id {command.HotelId}.");

                throw new ArgumentException($"RoomId {command.RoomId} does not exist in Hotel with Id {command.HotelId}.");
            }
        }

        /// <summary>
        /// Generates a unique hotel booking reference.
        /// </summary>
        /// <returns>A unique booking reference string.</returns>
        private string GenerateBookingReference()
        {
            DateTime now = DateTime.UtcNow;
            string datePart = now.ToString("yyyyMMddHHmmss");

            long counter = Interlocked.Increment(ref _counter);
            string counterPart = counter.ToString("D3");

            return $"{datePart}-{counterPart}";
        }
    }
}