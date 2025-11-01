using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    [ApiVersion("1.0")]
    public class RoomsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IMediator mediator, ILogger<RoomsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
    }
}
