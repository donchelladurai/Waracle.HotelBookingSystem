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
        /// Seeds the database with test data asynchronously.
        /// </summary>
        /// <remarks>This method populates the database with predefined test data, which can be used for
        /// development and testing purposes. Ensure that the database is in a suitable state before calling this
        /// method, as it may overwrite existing data.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SeedTestDataAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously removes all transactional data from the system.
        /// </summary>
        /// <remarks>This method should be used with caution as it will delete all transactional data,
        /// which may not be recoverable. Ensure that this operation is intended and that any necessary backups are in
        /// place before calling this method.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveAllTransactionalDataAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves a hotel by its name.
        /// </summary>
        /// <remarks>This method performs a case-insensitive search for the hotel name.</remarks>
        /// <param name="name">The name of the hotel to retrieve. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Hotel"/> object
        /// if found; otherwise, <see langword="null"/>.</returns>
        Task<Hotel> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves a list of available rooms for the specified date range and number of guests.
        /// </summary>
        /// <param name="checkIn">The desired check-in date for the room reservation.</param>
        /// <param name="checkOut">The desired check-out date for the room reservation. Must be later than <paramref name="checkIn"/>.</param>
        /// <param name="numberOfGuests">The number of guests that will occupy the room. Must be a positive integer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Room"/>
        /// objects that are available for the specified criteria. The list will be empty if no rooms are available.</returns>
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int numberOfGuests, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a list of available rooms in a specified hotel for the given date range and number of guests.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel to search for available rooms.</param>
        /// <param name="checkInDate">The date on which the guests plan to check in. Must be a future date.</param>
        /// <param name="checkOutDate">The date on which the guests plan to check out. Must be later than <paramref name="checkInDate"/>.</param>
        /// <param name="numberOfGuests">The number of guests that will occupy the room. Must be a positive integer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Room"/>
        /// objects that are available for the specified criteria. The list will be empty if no rooms are available.</returns>
        Task<IEnumerable<Room>> GetAvailableRoomsInHotelAsync(int hotelId, DateTime checkInDate, DateTime checkOutDate, int numberOfGuests, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a booking by its unique reference identifier.
        /// </summary>
        /// <param name="reference">The unique reference identifier of the booking to retrieve. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Booking"/> object
        /// associated with the specified reference.</returns>
        Task<Booking> GetBookingByReferenceAsync(string reference, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously adds a new booking to the system.
        /// </summary>
        /// <remarks>This method adds the specified booking to the system and should be awaited to ensure
        /// the operation completes.</remarks>
        /// <param name="booking">The booking details to be added. Cannot be null.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddBookingAsync(Booking booking, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the details of a hotel by its unique identifier.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel to retrieve. Must be a positive integer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. Can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Hotel"/> object
        /// with the specified identifier, or <see langword="null"/> if no hotel is found.</returns>
        Task<Hotel> GetHotelById(int hotelId, CancellationToken cancellationToken);
    }
}
