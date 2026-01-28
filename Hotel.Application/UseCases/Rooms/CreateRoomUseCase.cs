using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class CreateRoomUseCase(IRoomRepository roomRepository)
{
    public async Task<CreateRoomResult> ExecuteAsync(Room room, CancellationToken ct)
    {
        if (await roomRepository.ExistsByNumberAsync(room.Number, ct))
        {
            return CreateRoomResult.Conflict();
        }

        var id = await roomRepository.AddAsync(room, ct);
        return CreateRoomResult.Success(id);
    }
}
