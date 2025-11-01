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
    public class GetHotelByNameQueryHandler : IRequestHandler<GetHotelByNameQuery, Hotel>
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger<GetHotelByNameQueryHandler> _logger;

        public GetHotelByNameQueryHandler(IHotelsRepository hotelsRepository, ILogger<GetHotelByNameQueryHandler> logger)
        { 
            _hotelsRepository = hotelsRepository;
            _logger = logger;
        }

        public async Task<Hotel> Handle(GetHotelByNameQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNullOrEmpty(request.Name);

            _logger.LogInformation($"Searching for hotel with name {request.Name}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hotels = await _hotelsRepository.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);

                return hotels;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"The operation to find a hotel with name {request.Name} was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while finding hotels by name {request.Name}");

                throw;
            }
        }
    }
}
