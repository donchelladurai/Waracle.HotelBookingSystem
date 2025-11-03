using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> SeedDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await _mediator.Send(new Waracle.HotelBookingSystem.Application.Commands.SeedDataCommand())
                    .ConfigureAwait(false);

                return result
                    ? Ok("The data seeding was successful")
                    : StatusCode(409, "The data already seems to be seeded");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("SeedDataAsync operation was cancelled.");

                return StatusCode(499, "Operation cancelled.");
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
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> ClearAllTransactionalDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _mediator
                    .Send(new Waracle.HotelBookingSystem.Application.Commands.ClearAllTransactionalDataCommand())
                    .ConfigureAwait(false);

                return Ok("All transactional data has been cleared successfully");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("ClearAllTransactionalDataAsync operation was cancelled.");

                return StatusCode(499, "Operation cancelled.");
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
