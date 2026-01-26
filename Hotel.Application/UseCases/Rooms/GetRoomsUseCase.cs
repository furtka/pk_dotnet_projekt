using Hotel.Application.Domain.Repositories;

namespace Hotel.Application.UseCases.Rooms;

public class GetRoomsUseCase(IRoomRepository roomRepository)
{
    public Task<RoomPage> ExecuteAsync(int pageSize, int? nextId, int? minCapacity, bool onlyActive, CancellationToken ct)
    {
        return roomRepository.GetAllAsync(pageSize, nextId, minCapacity, onlyActive, ct);
    }
}
