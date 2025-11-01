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
        /// Given a hotel name, retrieves a list of hotels that match the provided name.
        /// </summary>
        /// <param name="name">The hotel name to search with</param>
        /// <returns>A list of hotels that match the name or HTTP 204 if no matches</returns>
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelDto>> GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Hotel name must be provided.");
            }

            try
            {
                var hotels = await _mediator.Send(new GetHotelsByNameQuery(name));

                if (hotels == null || !hotels.Any())
                {
                    return NoContent();
                }

                return Ok(hotels);
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
