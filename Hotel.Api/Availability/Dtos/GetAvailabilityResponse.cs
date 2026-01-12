namespace Hotel.Api.Availability.Dtos;

public sealed class GetAvailabilityResponse
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal? Price { get; set; }
}
