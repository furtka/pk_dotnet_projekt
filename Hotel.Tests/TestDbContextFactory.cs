using Hotel.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Tests;

public static class TestDbContextFactory
{
    public static HotelDbContext Create()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<HotelDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new HotelDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
