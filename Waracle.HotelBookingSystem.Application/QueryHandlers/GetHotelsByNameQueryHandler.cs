using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.QueryHandlers
{
    public class GetHotelsByNameQueryHandler : IRequestHandler<GetHotelsByNameQuery, IEnumerable<HotelDto>>
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger<GetHotelsByNameQueryHandler> _logger;

        public GetHotelsByNameQueryHandler(IHotelsRepository hotelsRepository, ILogger<GetHotelsByNameQueryHandler> logger)
        { 
            _hotelsRepository = hotelsRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<HotelDto>> Handle(GetHotelsByNameQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNullOrEmpty(request.Name);

            _logger.LogInformation($"Searching for hotel with name {request.Name}.");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hotels = await _hotelsRepository.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);

                return hotels.Select(h => new HotelDto()
                {
                    Id = h.Id,
                    Name = h.Name
                });
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
