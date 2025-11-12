using MediatR;
using Microsoft.Extensions.Logging;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Common;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    /// <summary>
    /// The command handler responsible for processing room booking requests.
    /// </summary>
    public class BookRoomCommandHandler : IRequestHandler<BookRoomCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookRoomCommandHandler> _logger;

        private readonly Random _random = new();
        private static long _counter = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRoomCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work object</param>
        /// <param name="logger">The logger used to log information and errors related to the booking process.</param>
        public BookRoomCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<BookRoomCommandHandler> logger)
        {
            this._unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles the booking of a room by creating a new booking record.
        /// </summary>
        /// <param name="command">The command request containing details of the room booking.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the booking operation.</param>
        /// <returns>A <see cref="BookRoomCommandResult"/> object representing the changed state.</returns>
        public async Task<Result<string>> Handle(BookRoomCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                return Result<string>.Failure(new Error("Command is null", $"The command inside {typeof(BookRoomCommandHandler)} is null"));


            if (cancellationToken == null)
                return Result<string>.Failure(new Error("CancellationToken is null", $"The CancellationToken inside {typeof(BookRoomCommandHandler)} is null"));

            if (command.NumberOfGuests <= 0)
                return Result<string>.Failure(new Error("Invalid Number of Guests", $"The value for NumberOfGuests was ${command.NumberOfGuests}"));

            if (command.IsCheckoutDateAfterCheckInDate() == false)
                return Result<string>.Failure(new Error("Check-Out date is before Check-In date", $"The Check-Out date {command.CheckOutDate.ToShortDateString()} is before the Check-In date {command.CheckInDate}"));

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var allRooms = await _unitOfWork.RoomsRepository.GetByHotelIdAsync(command.HotelId, cancellationToken).ConfigureAwait(false);

                var availableRooms = allRooms.GetAvailableRooms(command.NumberOfGuests, command.CheckInDate, command.CheckOutDate);

                if (!availableRooms.Any(r => r.Id == command.RoomId))
                {
                    _logger.LogError($"Room {command.RoomId} is not available from {command.CheckInDate} to {command.CheckOutDate}.");

                    return Result<string>.Failure(new Error("Room not available to book", $"The room {command.RoomId} is not available from {command.CheckInDate} to {command.CheckOutDate}"));
                }

                var room = availableRooms.Single(r => r.Id == command.RoomId);

                var booking = new Booking
                {
                    Reference = GenerateBookingReference(),
                    Room = room,
                    CheckInDate = command.CheckInDate.ToUniversalTime(),
                    CheckOutDate = command.CheckOutDate.ToUniversalTime(),
                    NumberOfGuests = command.NumberOfGuests
                };

                await _unitOfWork.BookingsRepository.AddAsync(booking, cancellationToken).ConfigureAwait(false);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation($"Booking created successfully for room {command.RoomId}.");

                return Result<string>.Success(booking.Reference);
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