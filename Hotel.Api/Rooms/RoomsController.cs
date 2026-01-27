using Hotel.Api.Rooms.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Rooms;

[ApiController]
[Route("api/rooms")]
public class RoomsController(
    GetRoomsUseCase getRoomsUseCase,
    GetRoomByIdUseCase getRoomByIdUseCase,
    CreateRoomUseCase createRoomUseCase,
    UpdateRoomUseCase updateRoomUseCase,
    DeleteRoomUseCase deleteRoomUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetRoomsResponseItem>>> GetRooms(
        [FromQuery] GetRoomsRequest request,
        CancellationToken ct)
    {
        var rooms = await getRoomsUseCase.ExecuteAsync(
            request.MinCapacity,
            request.OnlyActive,
            ct);

        return rooms.Select(r => new GetRoomsResponseItem
        {
            Id = r.Id,
            Number = r.Number,
            Capacity = r.Capacity,
            IsActive = r.IsActive,
            PricePerNight = r.PricePerNight,
            Type = r.Type
        }).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetRoomResponse>> GetRoom(
        int id,
        CancellationToken ct)
    {
        var room = await getRoomByIdUseCase.ExecuteAsync(id, ct);

        if (room is null)
        {
            return NotFound();
        }

        return new GetRoomResponse
        {
            Id = room.Id,
            Number = room.Number,
            Capacity = room.Capacity,
            IsActive = room.IsActive,
            PricePerNight = room.PricePerNight,
            Type = room.Type
        };
    }

    [HttpPost]
    public async Task<ActionResult<CreateRoomResponse>> CreateRoom(
        CreateRoomRequest request,
        CancellationToken ct)
    {
        var room = new Room
        {
            Number = request.Number,
            Capacity = request.Capacity,
            IsActive = request.IsActive,
            PricePerNight = request.PricePerNight,
            Type = request.Type
        };

        var id = await createRoomUseCase.ExecuteAsync(room, ct);

        return new CreateRoomResponse { Id = id };
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRoom(
        int id,
        UpdateRoomRequest request,
        CancellationToken ct)
    {
        var room = new Room
        {
            Id = id,
            Number = request.Number,
            Capacity = request.Capacity,
            IsActive = request.IsActive,
            PricePerNight = request.PricePerNight,
            Type = request.Type
        };

        await updateRoomUseCase.ExecuteAsync(room, ct);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoom(int id, CancellationToken ct)
    {
        await deleteRoomUseCase.ExecuteAsync(id, ct);
        return NoContent();
    }
}