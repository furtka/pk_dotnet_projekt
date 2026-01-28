using Hotel.Application.Domain.Models;

namespace Hotel.Application.Domain.Repositories;

public interface IRoomRepository
{

    public enum RoomUpdateResult
    {
        NotFound,
        NumberConflict,
        Ok
    };
    Task<Room?> GetByIdAsync(int id, CancellationToken ct);
    Task<RoomPage> GetAllAsync(int pageSize, int? nextId, int? minCapacity, bool onlyActive, CancellationToken ct);
    Task<int> AddAsync(Room room, CancellationToken ct);
    Task<RoomUpdateResult> UpdateAsync(Room room, CancellationToken ct);
    Task DeleteAsync(Room room, CancellationToken ct);
    Task<bool> ExistsByNumberAsync(string number, CancellationToken ct);
}
