using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure;
using Hotel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EntityRoom = Hotel.Infrastructure.Entities.Room;

namespace Hotel.Tests.UseCases.Rooms;

public class DeleteRoomUseCaseTests : IDisposable
{
    private readonly HotelDbContext _dbContext;
    private readonly DeleteRoomUseCase _useCase;

    public DeleteRoomUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new RoomRepository(_dbContext);
        _useCase = new DeleteRoomUseCase(repository);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallDeleteAsync_WhenRoomExists()
    {
        // Arrange
        var entity = new EntityRoom { Id = 1, Number = "101", IsActive = true };
        _dbContext.Rooms.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        var deletedEntity = await _dbContext.Rooms.FindAsync(1);
        Assert.NotNull(deletedEntity);
        Assert.False(deletedEntity.IsActive);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCallDeleteAsync_WhenRoomDoesNotExist()
    {
        // Act
        await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        // No exception should be thrown
    }

    public void Dispose()
    {
        _dbContext.Database.GetDbConnection().Close();
        _dbContext.Dispose();
    }
}
