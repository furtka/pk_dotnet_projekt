namespace Hotel.Api.Reservations.Dtos;

public sealed class CreateReservationRequest
{
    public int RoomId { get; set; }
    public int GuestId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestsCount { get; set; }
}
