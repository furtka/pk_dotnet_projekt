using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Hotel.Infrastructure.Repositories;
using Xunit;

namespace Hotel.Tests.UseCases.Guest;

public class CreateGuestUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly CreateGuestUseCase _useCase;

    public CreateGuestUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new GuestRepository(_dbContext);
        _useCase = new CreateGuestUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAddGuestToDatabaseAndReturnId()
    {
        // Arrange
        var guest = new Hotel.Application.Domain.Models.Guest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "123456789"
        };

        // Act
        var id = await _useCase.ExecuteAsync(guest, CancellationToken.None);

        // Assert
        Assert.True(id > 0);
        var guestInDb = await _dbContext.Guests.FindAsync(id);
        Assert.NotNull(guestInDb);
        Assert.Equal("John", guestInDb.FirstName);
        Assert.Equal("Doe", guestInDb.LastName);
    }
}
