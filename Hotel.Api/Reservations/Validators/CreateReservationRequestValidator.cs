using FluentValidation;
using Hotel.Api.Reservations.Dtos;

namespace Hotel.Api.Reservations.Validators;

public sealed class CreateReservationRequestValidator
    : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn)
            .WithMessage("Invalid dates - CheckOut has to be later than CheckIn");

        RuleFor(x => x)
            .Must(x => x.CheckOut.DayNumber - x.CheckIn.DayNumber <= 30)
            .WithMessage("Invalid dates - Reservation longer than 30 nights");

        RuleFor(x => x.CheckIn)
            .Must(BeInTheFuture)
            .WithMessage("Invalid check in date - date needs to be in the future");
    }

    private static bool BeInTheFuture(DateOnly checkIn)
    {
        return checkIn > DateOnly.FromDateTime(DateTime.UtcNow);
    }
}