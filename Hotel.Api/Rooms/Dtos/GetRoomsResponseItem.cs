namespace Hotel.Api.Rooms.Dtos;

public sealed class GetRoomsResponseItem
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
}