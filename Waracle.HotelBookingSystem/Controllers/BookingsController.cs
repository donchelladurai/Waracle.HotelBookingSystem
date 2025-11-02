using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("/api/bookings")]
    [ApiVersion("1.0")]
    public class BookingsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> FindBookingByReferenceAsync(string bookingReference)
        {
            return Ok();
        }
    }
}
