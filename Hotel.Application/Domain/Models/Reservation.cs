namespace Hotel.Application.Domain.Models;

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestsCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Active";
    public byte[] RowVersion { get; set; } = [];

    public Room Room { get; set; } = default!;
    public Guest Guest { get; set; } = default!;
}
