using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Infrastructure.AvisCarHire.Interfaces;

namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    public class AvisHireCarRepository : IAvisHireCarRepository
    {
        private readonly IAvisRentalDataSource _avisRentalDataSource;

        public AvisHireCarRepository(IAvisRentalDataSource avisRentalDataSource)
        {
            _avisRentalDataSource = avisRentalDataSource;
        }
        public IEnumerable<HireCar> GetAll()
        {
            return _avisRentalDataSource.HireCars.ToList();
        }
    }
}
