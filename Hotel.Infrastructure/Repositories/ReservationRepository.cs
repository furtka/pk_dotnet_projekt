using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Hotel.Application.Domain.Repositories.IReservationRepository;
using EntityGuest = Hotel.Infrastructure.Entities.Guest;
using EntityReservation = Hotel.Infrastructure.Entities.Reservation;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Infrastructure.Repositories
{
    public class ReservationRepository(HotelDbContext dbContext) : IReservationRepository
    {
        public async Task<(ReservationResult, int?)> AddAsync(Reservation resv, CancellationToken ct)
        {
            // Check input data validity
            // CheckIn must be before checkout
            // Room must exist
            // guest must exist
            // room must have capacity for guests
            var room = await dbContext.Rooms.FindAsync([resv.RoomId], ct);
            var guest = await dbContext.Guests.FindAsync([resv.GuestId], ct);

            if (room == null || !room.IsActive || guest == null)
            {
                return (ReservationResult.InvalidRoomOrGuest, null);
            }

            if(room.Capacity < resv.GuestsCount)
            {
                return (ReservationResult.RoomTooSmall, null);
            }

            var hasCollisions = await dbContext.Reservations.AnyAsync(r => // find reservations
                r.RoomId == resv.RoomId && // for the same room
                r.Status == "Active" && // which are activ
                resv.CheckIn < r.CheckOut && r.CheckIn < resv.CheckOut /// which overlap in dates
                );


            if (hasCollisions)
            {
                return (ReservationResult.Conflict, null);
            }

            // Everything seems ok, construct a reservation
            var totalPrice = (resv.CheckOut.DayNumber - resv.CheckIn.DayNumber) * room.PricePerNight;

            var reservation = new EntityReservation()
            {
                RoomId = resv.RoomId,
                GuestId = resv.GuestId,
                CheckIn = resv.CheckIn,
                CheckOut = resv.CheckOut,
                GuestsCount = resv.GuestsCount,
                TotalPrice = totalPrice,
                Status = "Active", // Active by default
            };

            await dbContext.Reservations.AddAsync(reservation);
            await dbContext.SaveChangesAsync(ct);

            resv.Id = reservation.Id;
            return (ReservationResult.Ok, resv.Id);
        }

        public async Task<Reservation?> GetByIdAsync(int id, CancellationToken ct)
        {
            var reservation = await dbContext.Reservations.FindAsync([id], ct);
            if (reservation is null) return null;

            return MapToDomain(reservation);
        }
        public async Task<ReservationCancellationResult> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await dbContext.Reservations.FindAsync([id], ct);
            if (entity is null || entity.Status == "Inactive") return ReservationCancellationResult.AlreadyInactive;

            if (DateOnly.FromDateTime(DateTime.Now) >= entity.CheckIn)
                return ReservationCancellationResult.TooLateToCancel;

            entity.Status = "Inactive";
            await dbContext.SaveChangesAsync(ct);

            return ReservationCancellationResult.Ok;
        }

        private static Reservation MapToDomain(EntityReservation entity)
        {
            return new Reservation
            {
                Id = entity.Id,
                RoomId = entity.RoomId,
                GuestId = entity.GuestId,
                CheckIn = entity.CheckIn,
                CheckOut = entity.CheckOut,
                GuestsCount = entity.GuestsCount,
                TotalPrice = entity.TotalPrice,
                Status = entity.Status,
            };
        }
    }
}
