
using FluentValidation;
using Hotel.Api.Rooms.Dtos;

namespace Hotel.Api.Rooms.Validators;

public sealed class CreateRoomRequestValidator 
    : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
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