using MediatR;
using Microsoft.AspNetCore.Mvc;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Web.Api.Models;
using Waracle.HotelBookingSystem.Web.Api.Validators;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Waracle.HotelBookingSystem.Common.Enums;

    [Authorize]
    [ApiController]
    [Route("/api/bookings")]
    [ApiVersion("1.0")]
    public class BookingsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of all bookings. I've included this so that getting the booking reference to test api/bookings/bookingReference is easier.
        /// </summary>
        /// <returns>A list of BookingDto objects containing booking data</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        public async Task<IActionResult> GetAllBookingsAsync(CancellationToken cancellationToken)
        {
            return await HandleActionAsync(
                       _logger,
                       async () =>
                           {
                               cancellationToken.ThrowIfCancellationRequested();

                               var bookings = await _mediator.Send(new GetAllBookingsQuery()).ConfigureAwait(false);

                               return bookings.Any() ? Ok(bookings) : StatusCode(204, "No bookings were found");
                           },
                       nameof(GetAllBookingsAsync),
                       "Error getting all bookings");
        }

        /// <summary>
        /// Returns a booking based on the provided booking reference. The largest room accommodates up to 4 people.
        /// </summary>
        /// <param name="bookingReference">The booking reference (You can get the booking reference either from the response from [HttpPost] api/bookings or from [HttpGet] api/bookings)</param>
        /// <returns>
        /// HTTP 200 if a booking exists along with the booking data, HTTP 404 if not.
        /// 400 if the booking reference is invalid
        /// 499 If operation is cancelled
        /// 500 Internal Server Error: If an error occurs.
        /// </returns>
        [HttpGet("{bookingReference}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindBookingByReferenceAsync(
            string bookingReference,
            CancellationToken cancellationToken)
        {
            return await HandleActionAsync(
                       _logger,
                       async () =>
                           {
                               cancellationToken.ThrowIfCancellationRequested();
                               if (string.IsNullOrEmpty(bookingReference))
                               {
                                   return BadRequest("Booking reference must be provided.");
                               }

                               var booking = await _mediator.Send(new GetBookingByReferenceQuery(bookingReference))
                                                 .ConfigureAwait(false);
                               return booking is not null
                                          ? Ok(booking)
                                          : StatusCode(204, $"No booking found with reference {bookingReference}");
                           },
                       nameof(FindBookingByReferenceAsync),
                       $"Error getting booking by reference {bookingReference}");
        }

        /// <summary>
        /// Creates a new booking based on the provided booking details. You can get the HotelId and RoomId from the /api/rooms endpoint.
        /// </summary>
        /// <param name="model">The date fields accept the ISO format YYYY-MM-DD</param>
        /// <returns>
        /// 200 OK: If booking was successful.
        /// 409 If the selected room is not available for the specified dates.
        /// 499 If operation is cancelled
        /// 500 Internal Server Error: If an error occurs.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBookingAsync(
            CreateBookingModel model,
            CancellationToken cancellationToken)
        {
            var createBookingValidator = new CreateBookingModelValidator();
            var createBookingModelValidationResult = await createBookingValidator.ValidateAsync(model);

            if (createBookingModelValidationResult.IsValid is false)
            {
                return BadRequest(createBookingModelValidationResult.Errors);
            }

            return await HandleActionAsync(
                       _logger,
                       async () =>
                           {
                               cancellationToken.ThrowIfCancellationRequested();
                               var commandResult = await _mediator.Send(
                                                       new BookRoomCommand(
                                                           model.HotelId,
                                                           model.RoomId, 
                                                           model.CheckInDate,
                                                           model.CheckOutDate,
                                                           model.NumberOfGuests)).ConfigureAwait(false);
                               // The selected room is not available for the specified dates.
                               if (commandResult.IsFailure)
                               {
                                   if (commandResult.Error.ErrorType == Errors.CommandIsNull)
                                   {
                                       return StatusCode(
                                           409,
                                           $"The selected room is not available for {model.NumberOfGuests} occupants between {model.CheckInDate.ToFormattedDateString()} and {model.CheckOutDate.ToFormattedDateString()}");
                                   }

                                   return StatusCode(
                                       500,
                                       new { error = "An unexpected error occurred. The booking was not successful." });
                               }

                               return StatusCode(201, $"The booking was created with Booking Reference {commandResult.Value}");
                           },
                       nameof(CreateBookingAsync),
                       "Error creating booking");
        }
    }
}
