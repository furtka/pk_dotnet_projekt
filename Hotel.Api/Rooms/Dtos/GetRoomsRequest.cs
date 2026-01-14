namespace Hotel.Api.Rooms.Dtos;

public sealed class GetRoomsRequest
{
    public int? MinCapacity { get; set; }
    public bool OnlyActive { get; set; } = true;
}