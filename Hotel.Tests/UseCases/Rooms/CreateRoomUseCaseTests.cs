using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure;
using Hotel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

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
        Assert.True(result > 0);
        var entity = await _dbContext.Rooms.FindAsync(result);
        Assert.NotNull(entity);
        Assert.Equal("101", entity.Number);
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
