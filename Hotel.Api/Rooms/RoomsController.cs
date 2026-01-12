using Hotel.Api.Rooms.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Rooms;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    public async Task<ActionResult<List<GetRoomsResponseItem>>> GetRooms(
        [FromQuery] GetRoomsRequest request,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetRoomResponse>> GetRoom(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<CreateRoomResponse>> CreateRoom(CreateRoomRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRoom(int id, UpdateRoomRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoom(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}