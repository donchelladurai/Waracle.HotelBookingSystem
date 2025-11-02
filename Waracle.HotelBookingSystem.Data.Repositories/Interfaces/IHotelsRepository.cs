using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Data.Repositories.Interfaces
{
    public interface IHotelsRepository
    {
        /// <summary>
        /// Get all hotels
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of all hotels</returns>
        Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a hotel by its hotel id
        /// </summary>
        /// <param name="hotelId">The hotel Id</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A hotel that corresponds to the given id</returns>
        Task<Hotel?> GetByIdAsync(int hotelId, CancellationToken cancellationToken);

        /// <summary>
        /// Get all hotels where the hotel name contains the given name
        /// </summary>
        /// <param name="name">The name to search by</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A hotel that contains the given name</returns>
        Task<IEnumerable<Hotel>> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a hotel
        /// </summary>
        /// <param name="hotels">The list of htoels to add</param>
        /// <param name="cancellationToken"></param>
        Task CreateAsync(IEnumerable<Hotel> hotels, CancellationToken cancellationToken);
    }
}
