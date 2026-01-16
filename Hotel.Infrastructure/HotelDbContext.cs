namespace Hotel.Infrastructure;

using Hotel.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;


public class HotelDbContext(DbContextOptions<HotelDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Room>().HasIndex(r => r.Number).IsUnique();
        b.Entity<Reservation>().Property(x => x.RowVersion).IsRowVersion();
    }
}