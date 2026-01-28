using Hotel.Application.Domain.Models;

public class RoomPage
{
    public List<Room> Rooms { get; set; } = [];
    public bool HasNext { get; set; }
    public int? NextId { get; set; }
}
