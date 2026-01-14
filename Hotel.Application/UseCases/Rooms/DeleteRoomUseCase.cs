using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class DeleteRoomUseCase(IRoomRepository roomRepository)
{
    public async Task ExecuteAsync(int id, CancellationToken ct)
    {
        var room = await roomRepository.GetByIdAsync(id, ct);
        if (room is not null)
        {
            await roomRepository.DeleteAsync(room, ct);
        }
    }
}
