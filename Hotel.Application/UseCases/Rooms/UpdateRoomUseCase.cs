using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using static Hotel.Application.Domain.Repositories.IRoomRepository;

namespace Hotel.Application.UseCases.Rooms;

public class UpdateRoomUseCase(IRoomRepository roomRepository)
{
    public Task<RoomUpdateResult> ExecuteAsync(Room room, CancellationToken ct)
    {
        return roomRepository.UpdateAsync(room, ct);
    }
}
