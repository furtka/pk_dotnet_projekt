using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure;
using Hotel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Tests.UseCases.Rooms;

public class GetRoomByIdUseCaseTests : IDisposable
{
    private readonly HotelDbContext _dbContext;
    private readonly GetRoomByIdUseCase _useCase;

    public GetRoomByIdUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new RoomRepository(_dbContext);
        _useCase = new GetRoomByIdUseCase(repository);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnRoom_WhenRoomExists()
    {
        // Arrange
        var room = new EntityRoom { Id = 1, Number = "101" };
        _dbContext.Rooms.Add(room);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("101", result.Number);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
