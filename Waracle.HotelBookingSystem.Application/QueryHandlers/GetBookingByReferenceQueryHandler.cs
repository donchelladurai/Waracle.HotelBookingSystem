using MediatR;
using Microsoft.Extensions.Logging;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;

namespace Waracle.HotelBookingSystem.Application.QueryHandlers
{
    public class GetBookingByReferenceQueryHandler : IRequestHandler<GetBookingByReferenceQuery, BookingDto?>
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly ILogger<GetBookingByReferenceQueryHandler> _logger;

        public GetBookingByReferenceQueryHandler(IBookingsRepository bookingsRepository, ILogger<GetBookingByReferenceQueryHandler> logger)
        {
            _bookingsRepository = bookingsRepository ?? throw new ArgumentNullException(nameof(bookingsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BookingDto?> Handle(GetBookingByReferenceQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(cancellationToken);
            ArgumentNullException.ThrowIfNullOrEmpty(request.BookingReference);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var booking = await _bookingsRepository.FindByReferenceAsync(request.BookingReference, cancellationToken).ConfigureAwait(false);
                if (booking == null)
                {
                    _logger.LogInformation($"No booking found for reference {request.BookingReference}");

                    return null;
                }

                return new BookingDto
                {
                    HotelId = booking.Room.HotelId,
                    RoomId = booking.RoomId,
                    HotelName = booking.Room.Hotel.Name,
                    RoomType = booking.Room.RoomType.Name,
                    BookingReference = booking.Reference,
                    CheckInDate = booking.CheckInDate.ToFormattedDateString(),
                    CheckOutDate = booking.CheckOutDate.ToFormattedDateString(),
                    NumberOfGuests = booking.NumberOfGuests.GetValueOrDefault()
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"The operation to get booking by reference {request.BookingReference} was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while handling GetBookingByReferenceQuery for reference {request.BookingReference}");
                throw;
            }
        }
    }
}
