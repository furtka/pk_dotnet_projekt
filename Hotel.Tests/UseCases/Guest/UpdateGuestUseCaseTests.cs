using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Hotel.Infrastructure.Repositories;
using Xunit;

namespace Hotel.Tests.UseCases.Guest;

public class UpdateGuestUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly UpdateGuestUseCase _useCase;

    public UpdateGuestUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new GuestRepository(_dbContext);
        _useCase = new UpdateGuestUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateGuestDetails()
    {
        // Arrange
        var entity = new Hotel.Infrastructure.Entities.Guest
        {
            FirstName = "Old",
            LastName = "Name",
            Email = "old@email.com"
        };
        _dbContext.Guests.Add(entity);
        await _dbContext.SaveChangesAsync();

        var updatedGuest = new Hotel.Application.Domain.Models.Guest
        {
            Id = entity.Id,
            FirstName = "New",
            LastName = "Name",
            Email = "new@email.com",
            Phone = "111222333"
        };

        // Act
        await _useCase.ExecuteAsync(updatedGuest, CancellationToken.None);

        // Assert
        var guestInDb = await _dbContext.Guests.FindAsync(entity.Id);
        Assert.NotNull(guestInDb);
        Assert.Equal("New", guestInDb.FirstName);
        Assert.Equal("new@email.com", guestInDb.Email);
        Assert.Equal("111222333", guestInDb.Phone);
    }
}
