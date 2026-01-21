using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure;
using Hotel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Tests.UseCases.Rooms;

public class UpdateRoomUseCaseTests : IDisposable
{
    private readonly HotelDbContext _dbContext;
    private readonly UpdateRoomUseCase _useCase;

    public UpdateRoomUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new RoomRepository(_dbContext);
        _useCase = new UpdateRoomUseCase(repository);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallUpdateAsync_WhenCalled()
    {
        // Arrange
        var entity = new EntityRoom { Id = 1, Number = "101", Capacity = 2 };
        _dbContext.Rooms.Add(entity);
        await _dbContext.SaveChangesAsync();

        var room = new Room { Id = 1, Number = "101", Capacity = 5 };

        // Act
        await _useCase.ExecuteAsync(room, CancellationToken.None);

        // Assert
        var updatedEntity = await _dbContext.Rooms.FindAsync(1);
        Assert.NotNull(updatedEntity);
        Assert.Equal(5, updatedEntity.Capacity);
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
