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
    /// <summary>
    /// Retrieves a list of rooms based on filtering criteria.
    /// </summary>
    /// <param name="request">The filtering criteria (minimum capacity, active status).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of rooms matching the criteria.</returns>
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
            IsActive = r.IsActive
        }).ToList();
    }

    /// <summary>
    /// Retrieves a specific room by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the room.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The room details if found; otherwise, NotFound.</returns>
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
            IsActive = room.IsActive
        };
    }

    /// <summary>
    /// Creates a new room.
    /// </summary>
    /// <param name="request">The room details to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The unique identifier of the newly created room.</returns>
    [HttpPost]
    public async Task<ActionResult<CreateRoomResponse>> CreateRoom(
        CreateRoomRequest request,
        CancellationToken ct)
    {
        var room = new Room
        {
            Number = request.Number,
            Capacity = request.Capacity,
            IsActive = request.IsActive
        };

        var id = await createRoomUseCase.ExecuteAsync(room, ct);

        return new CreateRoomResponse { Id = id };
    }

    /// <summary>
    /// Updates an existing room.
    /// </summary>
    /// <param name="id">The unique identifier of the room to update.</param>
    /// <param name="request">The updated room details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent if successful.</returns>
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
            IsActive = request.IsActive
        };

        await updateRoomUseCase.ExecuteAsync(room, ct);

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific room.
    /// </summary>
    /// <param name="id">The unique identifier of the room to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoom(int id, CancellationToken ct)
    {
        await deleteRoomUseCase.ExecuteAsync(id, ct);
        return NoContent();
    }
}