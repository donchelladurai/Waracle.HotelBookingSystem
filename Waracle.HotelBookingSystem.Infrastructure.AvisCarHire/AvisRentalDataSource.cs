using Waracle.HotelBookingSystem.Infrastructure.AvisCarHire.Interfaces;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Infrastructure.AvisCarHire
{
    public class AvisRentalDataSource : IAvisRentalDataSource
    {
        public IEnumerable<HireCar> HireCars => new List<HireCar>()
                                                    {
                                                        new HireCar("Avis", "Honda", "Civic", 5),
                                                        new HireCar("Avis", "Vauxhall", "Corsa", 2),
                                                        new HireCar("Avis", "Mini", "Countryman", 5)
                                                    };
    }
}
