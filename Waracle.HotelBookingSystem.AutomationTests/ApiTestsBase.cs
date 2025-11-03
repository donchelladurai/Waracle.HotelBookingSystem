using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waracle.HotelBookingSystem.AutomationTests.Helpers;

namespace Waracle.HotelBookingSystem.AutomationTests
{
    public class ApiTestsBase
    {
        private const string BaseUrl = "https://waracle-hotelbookingsystem-webapi-fjgpd5d5hwdrbxbt.canadacentral-01.azurewebsites.net";

        protected readonly RestClientHelper HttpClient = new RestClientHelper(BaseUrl);
    }
}
