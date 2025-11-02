using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Application.Queries;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Domain.Entities;
using Waracle.HotelBookingSystem.Web.Api.Models;
using Waracle.HotelBookingSystem.Web.Api.Validators;

namespace Waracle.HotelBookingSystem.Web.Api.Controllers
{
    [ApiController]
    [Route("/api/bookings")]
    [ApiVersion("1.0")]
    public class BookingsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        /// <summary>
        /// Get a list of all bookings.
        /// </summary>
        /// <returns>A list of BookingDto objects containing booking data</returns>
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
        /// Returns a booking based on the provided booking reference.
        /// </summary>
        /// <param name="bookingReference">The booking reference</param>
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
        /// Creates a new booking based on the provided booking details.
        /// </summary>
        /// <param name="model">The <see cref="CreateBookingModel"/> create booking model</param>
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

                if(commandResult.IsSucessful is false)
                {
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
