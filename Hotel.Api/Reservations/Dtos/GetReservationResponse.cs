namespace Hotel.Api.Reservations.Dtos;

public sealed class GetReservationResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestsCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = default!;
}
