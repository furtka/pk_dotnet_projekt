namespace Hotel.Api.Rooms.Dtos;

public sealed class UpdateRoomRequest
{
    public string Number { get; set; } = default!;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public string Type { get; set; } = "Standard";
    public decimal PricePerNight { get; set; }
}
