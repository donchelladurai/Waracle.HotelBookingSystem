using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Common.Dtos;

namespace Waracle.HotelBookingSystem.Application.Queries
{
    public record GetAllBookingsQuery : IRequest<IEnumerable<BookingDto>>
    {
    }
}
