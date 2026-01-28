using FluentValidation;
using Hotel.Api.Availability.Dtos;

namespace Hotel.Api.Availability.Validators;

public sealed class GetAvailabilityRequestValidator
    : AbstractValidator<GetAvailabilityRequest>
{
    public GetAvailabilityRequestValidator()
    {
        RuleFor(x => x.CheckOut)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(x => x.CheckIn)
            .WithMessage("Invalid dates - CheckOut has to be later than CheckIn");

        RuleFor(x => x)
            .Must(x => x.CheckOut.DayNumber - x.CheckIn.DayNumber <= 30)
            .WithMessage("Invalid dates - Reservation longer than 30 nights");

        RuleFor(x => x.CheckIn)
            .Must(BeInTheFuture)
            .WithMessage("Invalid check in date - date needs to be in the future");

        RuleFor(x => x.MinCapacity)
            .GreaterThan(0)
            .When(x => x.MinCapacity.HasValue);
    }

    private static bool BeInTheFuture(DateOnly checkIn)
    {
        return checkIn > DateOnly.FromDateTime(DateTime.UtcNow);
    }
}