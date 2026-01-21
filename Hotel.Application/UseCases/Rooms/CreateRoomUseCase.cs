using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class CreateRoomUseCase(IRoomRepository roomRepository)
{
    public Task<int> ExecuteAsync(Room room, CancellationToken ct)
    {
        return roomRepository.AddAsync(room, ct);
    }
}
