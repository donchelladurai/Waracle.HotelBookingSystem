using MediatR;
using Microsoft.Extensions.Logging;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Common;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    using Waracle.HotelBookingSystem.Common.Enums;

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
            if (ArgumentsInvalid(command, cancellationToken, out var result))
            {
                return result;
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var allRooms = await _unitOfWork.RoomsRepository.GetByHotelIdAsync(command.HotelId, cancellationToken).ConfigureAwait(false);

                var availableRooms = allRooms.GetAvailableRooms(command.NumberOfGuests, command.CheckInDate, command.CheckOutDate);

                if (!availableRooms.Any(r => r.Id == command.RoomId))
                {
                    var roomNotAvailableMessage = $"Room {command.RoomId} is not available from {command.CheckInDate} to {command.CheckOutDate}.";

                    _logger.LogError(roomNotAvailableMessage);

                    return Result<string>.Failure(new Error(Errors.RoomNotAvailable, roomNotAvailableMessage));
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

        private static bool ArgumentsInvalid(BookRoomCommand command, CancellationToken cancellationToken, out Result<string> result)
        {
            if (command == null)
            {
                result = Result<string>.Failure(new Error(Errors.CommandIsNull, $"The command inside {typeof(BookRoomCommandHandler)} is null"));
                return true;
            }

            if (cancellationToken == null)
            {
                result = Result<string>.Failure(new Error(Errors.CancellationTokenNotProvided, $"The CancellationToken inside {typeof(BookRoomCommandHandler)} is null"));
                return true;
            }

            if (command.NumberOfGuests <= 0)
            {
                result = Result<string>.Failure(new Error(Errors.InvalidNumberOfGuests, $"The value for NumberOfGuests was ${command.NumberOfGuests}"));
                return true;
            }

            if (command.IsCheckoutDateAfterCheckInDate() == false)
            {
                result = Result<string>.Failure(new Error(Errors.CheckOutDateBeforeCheckInDate, $"The Check-Out date {command.CheckOutDate.ToShortDateString()} is before the Check-In date {command.CheckInDate}"));
                return true;
            }

            result = Result<string>.Success("All arguments are valid");
            return true;
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