namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingsRepository BookingsRepository { get; }

        IHotelsRepository HotelsRepository { get; }

        IRoomsRepository RoomsRepository { get; }

        IAvisHireCarRepository AvisHireCarRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
