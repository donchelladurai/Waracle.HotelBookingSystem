using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Common.Helpers
{
    public static class DateHelper
    {
        public static string ToFormattedDateString(this DateTime dateTime)
        {
            return dateTime.Date.ToString("dd/MM/yyyy");
        }
    }
}
