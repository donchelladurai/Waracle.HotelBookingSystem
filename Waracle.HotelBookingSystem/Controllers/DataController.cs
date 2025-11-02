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

        /// <summary>
        /// Seeds the database with test data.
        /// </summary>
        /// <returns>
        /// True if data was seeded, false if data already exists.
        /// - 200 OK: If data seeding was successful or data already exists.
        /// - 500 Internal Server Error: If an error occurs.
        /// </returns>
        [Route("seed")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> SeedDataAsync()
        {
            try
            {
                var result = await _mediator.Send(new Waracle.HotelBookingSystem.Application.Commands.SeedDataCommand()).ConfigureAwait(false);

                return Ok(result ? "The data seeding was successful" : "The data already seems to be seeded");
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

        /// <summary>
        /// Clears all transactional data in the database.
        /// </summary>
        /// <returns>
        /// True if data was cleared successfully. False otherwise.
        /// 200 OK: If data clearing was successful.
        /// 500 Internal Server Error: If an error occurs.
        /// </returns>
        [Route("clear")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ClearAllTransactionalDataAsync()
        {
            try
            {
                await _mediator.Send(new Waracle.HotelBookingSystem.Application.Commands.ClearAllTransactionalDataCommand()).ConfigureAwait(false);

                return Ok("All transactional data has been cleared successfully");
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
