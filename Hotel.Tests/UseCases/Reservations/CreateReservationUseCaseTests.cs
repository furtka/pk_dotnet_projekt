using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Reservations;
using Hotel.Infrastructure.Repositories;
using Xunit;
using static Hotel.Application.Domain.Repositories.IReservationRepository;

namespace Hotel.Tests.UseCases.Reservations;

public class CreateReservationUseCaseTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly CreateReservationUseCase _useCase;

    public CreateReservationUseCaseTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var repository = new ReservationRepository(_dbContext);
        _useCase = new CreateReservationUseCase(repository);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    private async Task<(int roomId, int guestId)> SeedRoomAndGuest(int capacity = 2)
    {
        var room = new Hotel.Infrastructure.Entities.Room
        {
            Number = "101",
            Capacity = capacity,
            PricePerNight = 100,
            IsActive = true,
            Type = "Standard"
        };
        var guest = new Hotel.Infrastructure.Entities.Guest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _dbContext.Rooms.Add(room);
        _dbContext.Guests.Add(guest);
        await _dbContext.SaveChangesAsync();

        return (room.Id, guest.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAddReservation_WhenDataIsValid()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest();
        var reservation = new Reservation
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            GuestsCount = 2
        };

        // Act
        var (result, id) = await _useCase.ExecuteAsync(reservation, CancellationToken.None);

        // Assert
        Assert.Equal(ReservationResult.Ok, result);
        Assert.NotNull(id);
        var reservationInDb = await _dbContext.Reservations.FindAsync(id);
        Assert.NotNull(reservationInDb);
        Assert.Equal("Active", reservationInDb.Status);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnConflict_WhenDatesOverlap()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest();
        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var checkOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        await _useCase.ExecuteAsync(new Reservation
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            GuestsCount = 1
        }, CancellationToken.None);

        var overlappingReservation = new Reservation
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = checkIn.AddDays(1),
            CheckOut = checkOut.AddDays(1),
            GuestsCount = 1
        };

        // Act
        var (result, id) = await _useCase.ExecuteAsync(overlappingReservation, CancellationToken.None);

        // Assert
        Assert.Equal(ReservationResult.Conflict, result);
        Assert.Null(id);
    }
}
