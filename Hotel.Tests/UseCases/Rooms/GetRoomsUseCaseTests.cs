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
    public async Task ExecuteAsync_ShouldReturnRooms_WhenCalled()
    {
        // Arrange
        var rooms = new List<EntityRoom>
        {
            new() { Id = 1, Number = "101", Capacity = 2, IsActive = true },
            new() { Id = 2, Number = "102", Capacity = 3, IsActive = true }
        };
        _dbContext.Rooms.AddRange(rooms);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(null, true, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Number == "101");
        Assert.Contains(result, r => r.Number == "102");
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
