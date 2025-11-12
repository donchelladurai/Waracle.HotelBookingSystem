namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        BookingsRepository BookingsRepository { get; }

        HotelRepository HotelsRepository { get; }

        RoomsRepository RoomsRepository { get; }

        IAvisHireCarRepository AvisHireCarRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
