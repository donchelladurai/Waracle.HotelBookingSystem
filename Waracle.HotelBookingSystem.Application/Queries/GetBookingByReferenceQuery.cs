using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Common.Dtos;

namespace Waracle.HotelBookingSystem.Application.Queries
{
    public record GetBookingByReferenceQuery : IRequest<BookingDto?>
    {
        public GetBookingByReferenceQuery(string bookingReference)
        {
            BookingReference = bookingReference;
        }

        public string BookingReference { get; protected set; }
    }
}
