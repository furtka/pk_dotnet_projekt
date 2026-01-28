using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Reservations;
using Hotel.Infrastructure.Repositories;
using Xunit;

namespace Hotel.Tests.UseCases.Reservations;

public class GetReservationByIdUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly GetReservationByIdUseCase _useCase;

    public GetReservationByIdUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new ReservationRepository(_dbContext);
        _useCase = new GetReservationByIdUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnReservation_WhenExists()
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
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            GuestsCount = 1,
            TotalPrice = 200,
            Status = "Active"
        };
        _dbContext.Reservations.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(entity.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(room.Id, result.RoomId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenDoesNotExist()
    {
        // Act
        var result = await _useCase.ExecuteAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
