using FluentValidation;
using Waracle.HotelBookingSystem.Web.Api.Models;

namespace Waracle.HotelBookingSystem.Web.Api.Validators
{
    public class GetAvailableRoomsModelValidator : AbstractValidator<GetAvailableRoomsModel>
    {
        public GetAvailableRoomsModelValidator()
        {
            RuleFor(x => x.CheckInDate.Date)
                .NotEqual(x => x.CheckOutDate.Date)
                .WithMessage("Check-in date and check-out date cannot be the same.");

            RuleFor(x => x.CheckInDate)
                .LessThan(x => x.CheckOutDate)
                .WithMessage("Check-in date must be earlier than check-out date.");

            RuleFor(x => x.NumberOfOccupants)
                .GreaterThan(0)
                .WithMessage("Number of occupants must be greater than zero.");
        }
    }
}
