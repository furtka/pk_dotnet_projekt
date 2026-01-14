using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class UpdateRoomUseCase(IRoomRepository roomRepository)
{
    public Task ExecuteAsync(Room room, CancellationToken ct)
    {
        return roomRepository.UpdateAsync(room, ct);
    }
}
