using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using static Hotel.Application.Domain.Repositories.IRoomRepository;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Infrastructure.Repositories;

public class RoomRepository(HotelDbContext dbContext) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await dbContext.Rooms.FindAsync([id], ct);
        if (entity is null) return null;

        return MapToDomain(entity);
    }

    public async Task<List<Room>> GetAllAsync(int? minCapacity, bool onlyActive, CancellationToken ct)
    {
        var query = dbContext.Rooms.AsQueryable();

        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }

        if (onlyActive)
        {
            query = query.Where(r => r.IsActive);
        }

        var entities = await query
            .ToListAsync(ct);

        return entities.Select(MapToDomain).ToList();
    }

    public async Task<int> AddAsync(Room room, CancellationToken ct)
    {
        var entity = new EntityRoom
        {
            Number = room.Number,
            Capacity = room.Capacity,
            PricePerNight = room.PricePerNight,
            IsActive = room.IsActive,
            Type = room.Type
        };

        dbContext.Rooms.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        room.Id = entity.Id;
        return entity.Id;
    }

    public async Task<RoomUpdateResult> UpdateAsync(Room room, CancellationToken ct)
    {
        var checkDuplicateNumber = await dbContext.Rooms.AnyAsync(r => r.Id != room.Id && r.Number == room.Number, ct);

        if (checkDuplicateNumber)
        {
            return RoomUpdateResult.NumberConflict;
        }

        var entity = await dbContext.Rooms.FindAsync([room.Id], ct);
        if (entity is null) return RoomUpdateResult.NotFound;


        entity.Number = room.Number;
        entity.Capacity = room.Capacity;
        entity.PricePerNight = room.PricePerNight;
        entity.IsActive = room.IsActive;
        entity.Type = room.Type;

        await dbContext.SaveChangesAsync(ct);

        return RoomUpdateResult.Ok;
    }

    public async Task DeleteAsync(Room room, CancellationToken ct)
    {
        var entity = await dbContext.Rooms.FindAsync([room.Id], ct);
        if (entity is null) return;

        entity.IsActive = false;
        await dbContext.SaveChangesAsync(ct);
    }

    private static Room MapToDomain(EntityRoom entity)
    {
        return new Room
        {
            Id = entity.Id,
            Number = entity.Number,
            Capacity = entity.Capacity,
            PricePerNight = entity.PricePerNight,
            IsActive = entity.IsActive,
            Type = entity.Type
        };
    }
}
