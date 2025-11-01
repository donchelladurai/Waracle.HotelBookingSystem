using Microsoft.AspNetCore.Mvc;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
