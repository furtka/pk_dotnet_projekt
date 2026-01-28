using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure;
using Hotel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Tests.UseCases.Rooms;

public class CreateRoomUseCaseTests : IDisposable
{
    private readonly HotelDbContext _dbContext;
    private readonly CreateRoomUseCase _useCase;

    public CreateRoomUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new RoomRepository(_dbContext);
        _useCase = new CreateRoomUseCase(repository);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallAddAsync_WhenCalled()
    {
        // Arrange
        var room = new Room { Number = "101", Capacity = 2 };

        // Act
        var result = await _useCase.ExecuteAsync(room, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Id > 0);
        var entity = await _dbContext.Rooms.FindAsync(result.Id);
        Assert.NotNull(entity);
        Assert.Equal("101", entity.Number);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnConflict_WhenRoomNumberExists()
    {
        // Arrange
        var existingRoom = new Hotel.Infrastructure.Entities.Room
        {
            Number = "102",
            Capacity = 2,
            IsActive = true,
            PricePerNight = 100.00m
        };
        _dbContext.Rooms.Add(existingRoom);
        await _dbContext.SaveChangesAsync();

        var room = new Room { Number = "102", Capacity = 3 };

        // Act
        var result = await _useCase.ExecuteAsync(room, CancellationToken.None);

        // Assert
        Assert.True(result.IsConflict);
        Assert.Null(result.Id);
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
