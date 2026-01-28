namespace Hotel.Api.Rooms.Dtos;

public sealed class GetRoomsResponsePage
{
    public List<GetRoomsResponseItem> Rooms { get; set; } = [];
    public bool HasNext { get; set; }
    public int? NextId { get; set; }
}