using MediatR;
using Microsoft.AspNetCore.Mvc;
using Waracle.HotelBookingSystem.Common.Dtos;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("/api/data")]
    [ApiVersion("1.0")]
    public class DataController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DataController> _logger;

        public DataController(IMediator mediator, ILogger<DataController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> SeedData()
        {
            try
            {
                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error seeding data");
                return StatusCode(
                    500,
                    new
                    {
                        error = "Internal Server Error",
                        message = e.Message
                    });
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ResetAllTransactionalData()
        {
            try
            {
                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error clearing data");
                return StatusCode(
                    500,
                    new
                    {
                        error = "Internal Server Error",
                        message = e.Message
                    });
            }
        }
    }
}
