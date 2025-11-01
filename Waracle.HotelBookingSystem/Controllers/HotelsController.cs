using Microsoft.AspNetCore.Mvc;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    public class HotelsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
