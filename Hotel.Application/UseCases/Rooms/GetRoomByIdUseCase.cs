using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class GetRoomByIdUseCase(IRoomRepository roomRepository)
{
    public Task<Room?> ExecuteAsync(int id, CancellationToken ct)
    {
        return roomRepository.GetByIdAsync(id, ct);
    }
}
