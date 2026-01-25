using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using Hotel.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using EntityGuest = Hotel.Infrastructure.Entities.Guest;
using Guest = Hotel.Application.Domain.Models.Guest;

namespace Hotel.Infrastructure.Repositories
{
    public class GuestRepository(HotelDbContext dbContext) : IGuestRepository
    {
        public async Task<int> AddAsync(Guest guest, CancellationToken ct)
        {

            var entity = new EntityGuest
            {
                FirstName = guest.FirstName,
                LastName  = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone,
            };

            dbContext.Guests.Add(entity);
            await dbContext.SaveChangesAsync(ct);

            guest.Id = entity.Id;
            return entity.Id;
        }

        public async Task<List<Guest>> GetAllAsync(CancellationToken ct)
        {
            return dbContext.Guests.Select(MapToDomain).ToList();

           
        }

        public async Task<Guest?> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await dbContext.Guests.FindAsync([id], ct);
            if (entity is null) return null;

            return MapToDomain(entity);
        }

        public async Task UpdateAsync(Guest guest, CancellationToken ct)
        {
            var entity = await dbContext.Guests.FindAsync([guest.Id], ct);
            if (entity is null) return;

            entity.FirstName = guest.FirstName;
            entity.LastName = guest.LastName;
            entity.Email = guest.Email;
            entity.Phone = guest.Phone;

            await dbContext.SaveChangesAsync(ct);
        }


        private static Guest MapToDomain(EntityGuest entity)
        {
            return new Guest
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Phone = entity.Phone,
            };
        }
    }
}
