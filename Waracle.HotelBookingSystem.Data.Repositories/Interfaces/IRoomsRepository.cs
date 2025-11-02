using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    public interface IRoomsRepository
    {
        /// <summary>
        /// Returns a list of rooms for a specific hotel
        /// </summary>
        /// <param name="hotelId">The hotel Id</param>
        /// <returns>A list of rooms for a specific hotel</returns>
        Task<IEnumerable<Room>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <returns>A list of rooms</returns>
        Task<IEnumerable<Room>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Removes all rooms
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task RemoveAllAsync(CancellationToken cancellationToken);
    }
}
