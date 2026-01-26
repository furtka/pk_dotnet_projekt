using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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

    public async Task<RoomPage> GetAllAsync(int pageSize, int? nextId, int? minCapacity, bool onlyActive, CancellationToken ct)
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

        if (nextId.HasValue)
        {
            query = query.Where(r => r.Id >= nextId.Value);
        }

        var entities = await query
            .OrderBy(r => r.Id)
            .Take(pageSize)
            .ToListAsync(ct);
        
        var hasNext = entities.Count == pageSize;
        int? newNextId = hasNext ? entities.Last().Id + 1 : null;

        return new RoomPage
        {
            Rooms = [.. entities.Select(MapToDomain)],
            HasNext = hasNext,
            NextId = newNextId
        };
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

    public async Task UpdateAsync(Room room, CancellationToken ct)
    {
        var entity = await dbContext.Rooms.FindAsync([room.Id], ct);
        if (entity is null) return;

        entity.Number = room.Number;
        entity.Capacity = room.Capacity;
        entity.PricePerNight = room.PricePerNight;
        entity.IsActive = room.IsActive;
        entity.Type = room.Type;

        await dbContext.SaveChangesAsync(ct);
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
