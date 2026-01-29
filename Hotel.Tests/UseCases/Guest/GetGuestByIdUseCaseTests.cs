using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Hotel.Infrastructure.Repositories;
using Xunit;

namespace Hotel.Tests.UseCases.Guest;

public class GetGuestByIdUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly GetGuestByIdUseCase _useCase;

    public GetGuestByIdUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new GuestRepository(_dbContext);
        _useCase = new GetGuestByIdUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnGuest_WhenGuestExists()
    {
        // Arrange
        var entity = new Hotel.Infrastructure.Entities.Guest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com"
        };
        _dbContext.Guests.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(entity.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenGuestDoesNotExist()
    {
        // Act
        var result = await _useCase.ExecuteAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
