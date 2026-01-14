using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class GetRoomsUseCase(IRoomRepository roomRepository)
{
    public Task<List<Room>> ExecuteAsync(int? minCapacity, bool onlyActive, CancellationToken ct)
    {
        return roomRepository.GetAllAsync(minCapacity, onlyActive, ct);
    }
}
