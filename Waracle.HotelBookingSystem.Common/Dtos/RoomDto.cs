using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waracle.HotelBookingSystem.Common.Dtos
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
    }
}
