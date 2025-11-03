using MediatR;
using Microsoft.AspNetCore.Mvc;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Domain.Entities;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("/api/hotels")]
    [ApiVersion("1.0")]
    public class HotelsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(IMediator mediator, ILogger<HotelsController> logger) 
        { 
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Given a hotel name, retrieves a list of hotels that contain the provided string. I've gone with this approach since the requirement "Find a hotel based on its name" was somewhat open to interpretation.
        /// </summary>
        /// <param name="name">The hotel name to search with</param>
        /// <returns>
        /// A list of hotels that match the name or HTTP 204 if no matches
        /// - 200 OK: If any hotels are found.
        /// - 400 Bad Request: If the hotel name is null or empty.
        /// - 404 Not Found: If no hotels are found with the given name.
        /// - 500 Internal Server Error: If an error occurs.
        /// </returns>
        [HttpGet("{name}", Name = "GetHotelsByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Hotel name must be provided.");
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hotels = await _mediator.Send(new GetHotelsByNameQuery(name));

                if (hotels == null || !hotels.Any())
                {
                    return NotFound($"No hotels were found with the name {name}");
                }

                return Ok(hotels);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetByNameAsync operation was cancelled.");

                return StatusCode(499, "Operation cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting hotels by {name}");

                return StatusCode(
                    500,
                    new
                    {
                        error = "Internal Server Error",
                        message = ex.Message
                    });
            }
        }
    }
}
