namespace Hotel.Api.Guests.Dtos;

public sealed class GetGuestsRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
