using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;

namespace Waracle.HotelBookingSystem.Application.QueryHandlers
{
    public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly ILogger<GetAllBookingsQueryHandler> _logger;

        public GetAllBookingsQueryHandler(IBookingsRepository bookingsRepository, ILogger<GetAllBookingsQueryHandler> logger)
        {
            _bookingsRepository = bookingsRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<BookingDto>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var bookings = await _bookingsRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

                return bookings.Select(b => new BookingDto
                {
                    HotelId = b.Room.HotelId,
                    RoomId = b.RoomId,
                    BookingReference = b.Reference,
                    HotelName = b.Room.Hotel.Name,
                    RoomType = b.Room.RoomType.Name,
                    CheckInDate = b.CheckInDate.ToFormattedDateString(),
                    CheckOutDate = b.CheckOutDate.ToFormattedDateString(),
                    NumberOfGuests = b.NumberOfGuests.GetValueOrDefault()
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetAllBookings operation was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all bookings.");

                throw;
            }
        }
    }
}
