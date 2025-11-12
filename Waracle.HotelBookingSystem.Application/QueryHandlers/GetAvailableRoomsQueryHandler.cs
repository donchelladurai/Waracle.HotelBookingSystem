using MediatR;
using Microsoft.Extensions.Logging;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Common.Helpers;

namespace Waracle.HotelBookingSystem.Application.QueryHandlers
{
    public class GetAvailableRoomsQueryHandler : IRequestHandler<GetAvailableRoomsQuery, IEnumerable<RoomDto>>
    {
        private readonly IRoomsRepository _roomsRepository;
        private readonly ILogger<GetAvailableRoomsQueryHandler> _logger;

        public GetAvailableRoomsQueryHandler(IRoomsRepository roomsRepository, ILogger<GetAvailableRoomsQueryHandler> logger)
        {
            _roomsRepository = roomsRepository ?? throw new ArgumentNullException(nameof(roomsRepository)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<RoomDto>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(cancellationToken);

            if (request.NumberOfGuests <= 0)
            {
                _logger.LogError($"Number of guests {request.NumberOfGuests} is not valid.");

                throw new ArgumentException("Number of guests must be greater than zero.");
            }

            _logger.LogInformation($"Searching for available rooms between dates {request.CheckInDate} and {request.CheckOutDate}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                //var allRooms = await _roomsRepository
                //    .GetAllAsync(cancellationToken).ConfigureAwait(false);

                //var availableRooms = allRooms.GetAvailableRooms(request.NumberOfGuests, request.CheckInDate, request.CheckOutDate);

                //return availableRooms.Select(ar => new RoomDto()
                //{
                //    RoomId = ar.Id,
                //    HotelId = ar.HotelId,
                //    HotelName = ar.Hotel.Name,
                //    RoomType = ar.RoomType.Name,
                //    Capacity = ar.RoomType.Capacity
                //});

                return null;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"The operation to find available rooms between dates {request.CheckInDate} and {request.CheckOutDate} was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while finding available rooms between dates {request.CheckInDate} and {request.CheckOutDate}");

                throw;
            }
        }
    }
}
