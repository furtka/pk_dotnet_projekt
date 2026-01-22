namespace Hotel.Api.Guests.Dtos;

public sealed class CreateGuestRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; } = default!;
}
