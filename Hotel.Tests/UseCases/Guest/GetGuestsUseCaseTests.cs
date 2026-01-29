using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Hotel.Infrastructure.Repositories;
using Xunit;

namespace Hotel.Tests.UseCases.Guest;

public class GetGuestsUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly GetGuestsUseCase _useCase;

    public GetGuestsUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new GuestRepository(_dbContext);
        _useCase = new GetGuestsUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAllGuests()
    {
        // Arrange
        _dbContext.Guests.AddRange(
            new Hotel.Infrastructure.Entities.Guest { FirstName = "A", LastName = "B", Email = "a@b.com" },
            new Hotel.Infrastructure.Entities.Guest { FirstName = "C", LastName = "D", Email = "c@d.com" }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
