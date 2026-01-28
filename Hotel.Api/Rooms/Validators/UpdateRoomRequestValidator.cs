
using FluentValidation;
using Hotel.Api.Rooms.Dtos;

namespace Hotel.Api.Rooms.Validators;

public sealed class UpdateRoomRequestValidator
    : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty();

        RuleFor(x => x.Capacity)
            .GreaterThan(0);

        RuleFor(x => x.PricePerNight)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("PricePerNight must be greater than 0");
        
        RuleFor(x => x.Type)
            .NotEmpty();
    }
}