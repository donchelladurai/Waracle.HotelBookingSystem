using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.QueryHandlers
{
    public class GetAvailableRoomsQueryHandler : IRequestHandler<GetAvailableRoomsQuery, IEnumerable<Room>>
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger<GetAvailableRoomsQueryHandler> _logger;

        public GetAvailableRoomsQueryHandler(IHotelsRepository hotelsRepository, ILogger<GetAvailableRoomsQueryHandler> logger)
        {
            _hotelsRepository = hotelsRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Room>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.NumberOfGuests <= 0)
            {
                _logger.LogError($"Number of guests {request.NumberOfGuests} is not valid.");

                throw new ArgumentException("Number of guests must be greater than zero.");
            }

            _logger.LogInformation($"Searching for available rooms between dates {request.CheckInDate} and {request.CheckOutDate}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var availableRooms = await _hotelsRepository.GetAvailableRoomsAsync(request.CheckInDate, request.CheckOutDate, request.NumberOfGuests, cancellationToken).ConfigureAwait(false);

                return availableRooms;
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
