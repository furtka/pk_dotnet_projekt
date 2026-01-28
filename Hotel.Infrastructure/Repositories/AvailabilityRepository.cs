using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using EntityGuest = Hotel.Infrastructure.Entities.Guest;
using EntityReservation = Hotel.Infrastructure.Entities.Reservation;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Infrastructure.Repositories
{
    public class AvailabilityRepository(HotelDbContext dbContext) : IAvailabilityRepository
    {
        public async Task<List<Room>> GetAvailable(AvailabilityFilter f, CancellationToken ct)
        {
            var unavailableRooms = await dbContext.Reservations.Where(r => r.Status == "Active" && r.CheckIn < f.CheckOut && f.CheckIn < r.CheckOut)
                .Select(r => r.RoomId).Distinct().ToListAsync(ct);

            if (f.MinCapacity == null) f.MinCapacity = 0;

            return dbContext.Rooms.Where(r => r.IsActive && r.Capacity >= f.MinCapacity && !unavailableRooms.Contains(r.Id)).Select(MapToDomain).ToList();
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
}
