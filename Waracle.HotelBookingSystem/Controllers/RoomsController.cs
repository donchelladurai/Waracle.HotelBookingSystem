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
        /// <param name="checkInDate">The check in date in ISO format YYYY/MM/DD</param>
        /// <param name="checkOutDate">The check out date in ISO format YYYY/MM/DD</param>
        /// <param name="numberOfOccupants">The number of occupants (The largest room accommodates up to 4 people)</param>
        /// <returns>
        /// A list of RoomDto objects
        /// - 200 OK: If any rooms are found.
        /// - 400 Bad Request: If the input parameters are invalid.
        /// - 404 Not Found: If no rooms are found for the given criteria.
        /// - 499 If operation is cancelled
        /// - 500 Internal Server Error: If an error occurs.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int numberOfOccupants, CancellationToken cancellationToken)
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
                cancellationToken.ThrowIfCancellationRequested();

                var rooms = await _mediator.Send(new GetAvailableRoomsQuery(model.CheckInDate, model.CheckOutDate, model.NumberOfOccupants)).ConfigureAwait(false);

                return rooms.Any() ? Ok(rooms) : NotFound($"No rooms were found between the dates {model.CheckInDate.ToShortDateString()} and {model.CheckOutDate.ToShortDateString()} for {numberOfOccupants} occupants");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetByNameAsync operation was cancelled.");

                return StatusCode(499, "Operation cancelled.");
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
