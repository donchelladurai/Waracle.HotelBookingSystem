using FluentValidation;
using Waracle.HotelBookingSystem.Common.Dtos;
using Waracle.HotelBookingSystem.Web.Api.Models;

namespace Waracle.HotelBookingSystem.Web.Api.Validators
{
    public class CreateBookingModelValidator : AbstractValidator<CreateBookingModel>
    {
        public CreateBookingModelValidator()
        {
            RuleFor(b => b.HotelId).GreaterThan(0).WithMessage("Hotel Id is invalid");

            RuleFor(b => b.RoomId).GreaterThan(0).WithMessage("Room Id is invalid");

            RuleFor(b => b.NumberOfGuests).GreaterThan(0).WithMessage("Number of guests must be at least 1");

            RuleFor(x => x.CheckInDate.Date)
            .NotEqual(x => x.CheckOutDate.Date)
            .WithMessage("Check-in date and check-out date cannot be the same.");

            RuleFor(b => b.CheckOutDate)
                .GreaterThan(b => b.CheckInDate)
                .WithMessage("Check-out date must be later than the Check-in date");
        }
    }
}
