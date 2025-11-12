using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waracle.HotelBookingSystem.Infrastructure.AvisCarHire.Interfaces
{
    using Waracle.HotelBookingSystem.Domain.Entities;

    public interface IAvisRentalDataSource
    {
        IEnumerable<HireCar> HireCars { get; }
    }
}
