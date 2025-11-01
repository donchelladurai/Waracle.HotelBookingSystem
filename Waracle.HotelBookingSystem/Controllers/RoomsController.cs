using Microsoft.AspNetCore.Mvc;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
