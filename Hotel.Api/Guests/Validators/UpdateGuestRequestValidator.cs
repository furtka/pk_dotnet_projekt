using FluentValidation;
using Hotel.Api.Guests.Dtos;

namespace Hotel.Api.Guests.Validators;

public sealed class UpdateGuestRequestValidator
    : AbstractValidator<UpdateGuestRequest>
{
    public UpdateGuestRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(@"^(?:\+48\s?)?(?:\d{3}[\s-]?){2}\d{3}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number format is invalid");
    }
}