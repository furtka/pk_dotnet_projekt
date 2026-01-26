using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure;
using Hotel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Tests.UseCases.Rooms;

public class GetRoomsUseCaseTests : IDisposable
{
    private readonly HotelDbContext _dbContext;
    private readonly GetRoomsUseCase _useCase;

    public GetRoomsUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new RoomRepository(_dbContext);
        _useCase = new GetRoomsUseCase(repository);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFirstPage_WhenMultipleRoomsExist()
    {
        // Arrange
        var rooms = Enumerable.Range(1, 5).Select(i => new EntityRoom 
        { 
            Id = i, Number = $"10{i}", Capacity = 2, IsActive = true 
        }).ToList();
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(3, null, null, true, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Rooms.Count);
        Assert.True(result.HasNext);
        Assert.Equal(4, result.NextId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnSecondPage_WhenNextIdIsProvided()
    {
        // Arrange
        var rooms = Enumerable.Range(1, 5).Select(i => new EntityRoom 
        { 
            Id = i, Number = $"10{i}", Capacity = 2, IsActive = true 
        }).ToList();
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(3, 4, null, true, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Rooms.Count);
        Assert.False(result.HasNext);
        Assert.Null(result.NextId);
        Assert.Contains(result.Rooms, r => r.Id == 4);
        Assert.Contains(result.Rooms, r => r.Id == 5);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldFilterByMinCapacity()
    {
        // Arrange
        var rooms = new List<EntityRoom>
        {
            new() { Id = 1, Number = "101", Capacity = 1, IsActive = true },
            new() { Id = 2, Number = "102", Capacity = 3, IsActive = true },
            new() { Id = 3, Number = "103", Capacity = 5, IsActive = true }
        };
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(10, null, 4, true, CancellationToken.None);

        // Assert
        Assert.Single(result.Rooms);
        Assert.Equal("103", result.Rooms[0].Number);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldFilterByOnlyActive()
    {
        // Arrange
        var rooms = new List<EntityRoom>
        {
            new() { Id = 1, Number = "101", Capacity = 2, IsActive = true },
            new() { Id = 2, Number = "102", Capacity = 2, IsActive = false }
        };
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(10, null, null, true, CancellationToken.None);

        // Assert
        Assert.Single(result.Rooms);
        Assert.True(result.Rooms[0].IsActive);
        Assert.Equal("101", result.Rooms[0].Number);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmpty_WhenNoRoomsMatch()
    {
        // Arrange
        var rooms = new List<EntityRoom>
        {
            new() { Id = 1, Number = "101", Capacity = 2, IsActive = true }
        };
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(10, null, 10, true, CancellationToken.None);

        // Assert
        Assert.Empty(result.Rooms);
        Assert.False(result.HasNext);
        Assert.Null(result.NextId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnHasNextTrue_WhenRoomsCountEqualsPageSize()
    {
        // Arrange
        var rooms = Enumerable.Range(1, 3).Select(i => new EntityRoom 
        { 
            Id = i, Number = $"10{i}", Capacity = 2, IsActive = true 
        }).ToList();
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(3, null, null, true, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Rooms.Count);
        Assert.True(result.HasNext); 
        Assert.Equal(4, result.NextId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnHasNextFalse_WhenRoomsCountIsLessThanPageSize()
    {
        // Arrange
        var rooms = Enumerable.Range(1, 2).Select(i => new EntityRoom 
        { 
            Id = i, Number = $"10{i}", Capacity = 2, IsActive = true 
        }).ToList();
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(3, null, null, true, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Rooms.Count);
        Assert.False(result.HasNext);
        Assert.Null(result.NextId);
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
