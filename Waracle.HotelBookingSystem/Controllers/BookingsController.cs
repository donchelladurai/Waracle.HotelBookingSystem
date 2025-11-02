using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Common.Helpers;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Web.Api.Models;
using Waracle.HotelBookingSystem.Web.Api.Validators;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("/api/bookings")]
    [ApiVersion("1.0")]
    /// <summary>
    /// A controller for managing bookings.
    /// </summary>
    public class BookingsController : Controller
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
        public async Task<IActionResult> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await _mediator.Send(new GetAllBookingsQuery()).ConfigureAwait(false);
                return bookings.Any() ? Ok(bookings) : NotFound("No bookings were found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings");
                return StatusCode(
                    500,
                    new
                    {
                        error = "Internal Server Error",
                        message = ex.Message
                    });
            }
        }

        /// <summary>
        /// Returns a booking based on the provided booking reference. The largest room accommodates up to 4 people.
        /// </summary>
        /// <param name="bookingReference">The booking reference (You can get the booking reference either from the response from [HttpPost] api/bookings or from [HttpGet] api/bookings)</param>
        /// <returns>HTTP 200 if a booking exists along with the booking data, HTTP 404 if not.</returns>
        [HttpGet("bookingReference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindBookingByReferenceAsync(string bookingReference)
        {
            try
            {
                if (string.IsNullOrEmpty(bookingReference))
                {
                    return BadRequest("Booking reference must be provided.");
                }

                var booking = await _mediator.Send(new GetBookingByReferenceQuery(bookingReference)).ConfigureAwait(false);

                return booking is not null ? Ok(booking) : NotFound($"No booking found with reference {bookingReference}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking by reference {bookingReference}");

                return StatusCode(
                    500,
                    new
                    {
                        error = "Internal Server Error",
                        message = ex.Message
                    });
            }
        }

        /// <summary>
        /// Creates a new booking based on the provided booking details. You can get the HotelId and RoomId from the /api/rooms endpoint.
        /// </summary>
        /// <param name="model">Note: The date fields accept the ISO format YYYY-MM-DD</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBookingAsync(CreateBookingModel model)
        {
            var createBookingValidator = new CreateBookingModelValidator();
            var createBookingModelValidationResult = await createBookingValidator.ValidateAsync(model);

            if (createBookingModelValidationResult.IsValid is false)
            {
                return BadRequest(createBookingModelValidationResult.Errors);
            }

            try
            {
                var commandResult = await _mediator.Send(new BookRoomCommand(model.HotelId, model.RoomId, model.CheckInDate, model.CheckOutDate, model.NumberOfGuests)).ConfigureAwait(false);

                //The selected room is not available for the specified dates.
                if (commandResult.IsSuccessful is false)
                {
                    if (commandResult.IsRoomUnavailable)
                    {
                        return Ok($"The selected room is not available for {model.NumberOfGuests} occupants between {model.CheckInDate.ToFormattedDateString()} and {model.CheckOutDate.ToFormattedDateString()}");
                    }

                    return StatusCode(
                        500,
                        new
                        {
                            error = "An unexpected error occurred. The booking was not successful."
                        });
                }

                return Ok($"The booking was created with Booking Reference {commandResult.BookingReference}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
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
