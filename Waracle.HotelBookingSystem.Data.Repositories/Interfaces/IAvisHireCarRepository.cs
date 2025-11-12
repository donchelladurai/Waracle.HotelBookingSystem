namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    using Waracle.HotelBookingSystem.Domain.Entities;

    public interface IAvisHireCarRepository
    {
        IEnumerable<HireCar> GetAll();
    }
}
