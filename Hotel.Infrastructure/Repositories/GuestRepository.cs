using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using EntityGuest = Hotel.Infrastructure.Entities.Guest;

namespace Hotel.Infrastructure.Repositories
{
    public class GuestRepository(HotelDbContext dbContext) : IGuestRepository
    {
        public Task<int> AddAsync(Guest guest, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Guest>> GetAllAsync(CancellationToken ct)
        {
            return dbContext.Guests.Select(MapToDomain).ToList();

           // return entities.Select(MapToDomain).ToList();
        }

        public Task<Guest?> GetByIdAsync(int id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guest guest, CancellationToken ct)
        {
            throw new NotImplementedException();
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
