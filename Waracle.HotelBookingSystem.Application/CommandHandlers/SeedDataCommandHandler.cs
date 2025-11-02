using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Data.Repositories;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.DatabaseContexts;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    public class SeedDataCommandHandler : IRequestHandler<SeedDataCommand, bool>
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger<SeedDataCommandHandler> _logger;

        private readonly List<string> _hotelNamesSeedData = [
            "Travelodge",
            "Premier Inn",
            "Holiday Inn",
            "Budget Inn",
            "Savoy"];

        public SeedDataCommandHandler(IHotelsRepository hotelsRepository, ILogger<SeedDataCommandHandler> logger)
        {
            _hotelsRepository = hotelsRepository ?? throw new ArgumentNullException(nameof(hotelsRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(SeedDataCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(cancellationToken);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var allHotels = await _hotelsRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

                if (allHotels.Any())
                {
                    _logger.LogInformation("The database is not empty so it cannot be seeded");

                    return false;
                }

                var hotelsSeedData = new List<Hotel>();

                foreach (var hotelName in _hotelNamesSeedData)
                {
                    var hotel = new Hotel 
                    { 
                        Name = hotelName,
                        Rooms = new List<Room>()
                    };

                    hotel.Rooms.Add(new Room { RoomTypeId = 1 });
                    hotel.Rooms.Add(new Room { RoomTypeId = 1 });
                    hotel.Rooms.Add(new Room { RoomTypeId = 2 });
                    hotel.Rooms.Add(new Room { RoomTypeId = 2 });
                    hotel.Rooms.Add(new Room { RoomTypeId = 3 });
                    hotel.Rooms.Add(new Room { RoomTypeId = 3 });

                    hotelsSeedData.Add(hotel);
                }

                await _hotelsRepository.CreateAsync(hotelsSeedData, cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"The seed data operation was cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding data");

                throw;
            }
        }
    }
}
