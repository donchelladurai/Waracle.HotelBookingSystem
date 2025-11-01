using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Application.Queries
{
    /// <summary>
    /// Represents a query to retrieve a hotel by its name.
    /// </summary>
    /// <remarks>This query is used to find a specific hotel based on the provided name. It implements the
    /// <see cref="IRequest{TResponse}"/> interface, where TResponse is a <see cref="Hotel"/>.</remarks>
    public class GetHotelByNameQuery : IRequest<Hotel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHotelByNameQuery"/> class with the specified hotel name.
        /// </summary>
        /// <param name="name">The name of the hotel to query. Cannot be null or empty.</param>
        public GetHotelByNameQuery(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }
    }
}
