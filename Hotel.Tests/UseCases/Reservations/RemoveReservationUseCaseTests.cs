using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Reservations;
using Hotel.Infrastructure.Repositories;
using Xunit;
using static Hotel.Application.Domain.Repositories.IReservationRepository;

namespace Hotel.Tests.UseCases.Reservations;

public class RemoveReservationUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly RemoveReservationUseCase _useCase;

    public RemoveReservationUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new ReservationRepository(_dbContext);
        _useCase = new RemoveReservationUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSetStatusToInactive_WhenValid()
    {
        // Arrange
        var room = new Hotel.Infrastructure.Entities.Room { Number = "101", Capacity = 2, PricePerNight = 100, IsActive = true, Type = "Standard" };
        var guest = new Hotel.Infrastructure.Entities.Guest { FirstName = "A", LastName = "B", Email = "a@b.com" };
        _dbContext.Rooms.Add(room);
        _dbContext.Guests.Add(guest);
        await _dbContext.SaveChangesAsync();

        var entity = new Hotel.Infrastructure.Entities.Reservation
        {
            RoomId = room.Id,
            GuestId = guest.Id,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
            GuestsCount = 1,
            TotalPrice = 200,
            Status = "Active"
        };
        _dbContext.Reservations.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(entity.Id, CancellationToken.None);

        // Assert
        Assert.Equal(ReservationCancellationResult.Ok, result);
        var reservationInDb = await _dbContext.Reservations.FindAsync(entity.Id);
        Assert.Equal("Inactive", reservationInDb!.Status);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnTooLate_WhenCheckInIsTodayOrPast()
    {
        // Arrange
        var room = new Hotel.Infrastructure.Entities.Room { Number = "101", Capacity = 2, PricePerNight = 100, IsActive = true, Type = "Standard" };
        var guest = new Hotel.Infrastructure.Entities.Guest { FirstName = "A", LastName = "B", Email = "a@b.com" };
        _dbContext.Rooms.Add(room);
        _dbContext.Guests.Add(guest);
        await _dbContext.SaveChangesAsync();

        var entity = new Hotel.Infrastructure.Entities.Reservation
        {
            RoomId = room.Id,
            GuestId = guest.Id,
            CheckIn = DateOnly.FromDateTime(DateTime.Now),
            CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            GuestsCount = 1,
            TotalPrice = 200,
            Status = "Active"
        };
        _dbContext.Reservations.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(entity.Id, CancellationToken.None);

        // Assert
        Assert.Equal(ReservationCancellationResult.TooLateToCancel, result);
    }
}
