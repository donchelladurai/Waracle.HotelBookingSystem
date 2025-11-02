using MediatR;
using Microsoft.AspNetCore.Mvc;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Web.Api.Models;
using Waracle.HotelBookingSystem.Web.Api.Validators;

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

        /// <summary>
        /// Returns a list of all available rooms within the specified date range and for the specified number of occupants.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A list of RoomDto objects</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int numberOfOccupants)
        {
            var model = new GetAvailableRoomsModel
            {
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                NumberOfOccupants = numberOfOccupants
            };
            var validator = new GetAvailableRoomsModelValidator();

            var validationResult = await validator.ValidateAsync(model).ConfigureAwait(false);

            if (validationResult.IsValid is false)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var rooms = await _mediator.Send(new GetAvailableRoomsQuery(model.CheckInDate, model.CheckOutDate, model.NumberOfOccupants)).ConfigureAwait(false);

                return rooms.Any() ? Ok(rooms) : NotFound($"No rooms were found between the dates {model.CheckInDate.ToShortDateString()} and {model.CheckOutDate.ToShortDateString()} for {numberOfOccupants} occupants");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");

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
