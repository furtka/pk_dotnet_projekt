using Hotel.Api.Availability;
using Hotel.Api.Availability.Dtos;
using Hotel.Application.UseCases.Availability;
using Hotel.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hotel.Tests.Integration;

public class AvailabilityControllerTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly AvailabilityController _controller;

    public AvailabilityControllerTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var availabilityRepository = new AvailabilityRepository(_dbContext);
        var getAvailableRoomsUseCase = new GetAvailableRoomsUseCase(availabilityRepository);
        var loggerMock = new Mock<ILogger<AvailabilityController>>();

        _controller = new AvailabilityController(
            getAvailableRoomsUseCase,
            loggerMock.Object
        );
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    private async Task SeedData()
    {
        var room1 = new Hotel.Infrastructure.Entities.Room
        {
            Number = "101",
            Capacity = 2,
            PricePerNight = 100,
            IsActive = true,
            Type = "Standard"
        };
        var room2 = new Hotel.Infrastructure.Entities.Room
        {
            Number = "102",
            Capacity = 4,
            PricePerNight = 200,
            IsActive = true,
            Type = "Deluxe"
        };
        var room3 = new Hotel.Infrastructure.Entities.Room
        {
            Number = "103",
            Capacity = 2,
            PricePerNight = 150,
            IsActive = false, // Inactive
            Type = "Standard"
        };

        _dbContext.Rooms.AddRange(room1, room2, room3);
        await _dbContext.SaveChangesAsync();

        var guest = new Hotel.Infrastructure.Entities.Guest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        _dbContext.Guests.Add(guest);
        await _dbContext.SaveChangesAsync();

        // Add a reservation for room 101
        var reservation = new Hotel.Infrastructure.Entities.Reservation
        {
            RoomId = room1.Id,
            GuestId = guest.Id,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            GuestsCount = 2,
            TotalPrice = 500,
            Status = "Active"
        };
        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAvailability_ShouldReturnAvailableRooms_WhenNoConflicts()
    {
        // Arrange
        await SeedData();
        var request = new GetAvailabilityRequest
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4))
        };

        // Act
        var result = await _controller.GetAvailability(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<GetAvailabilityResponse>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<GetAvailabilityResponse>>(okResult.Value).ToList();

        Assert.Equal(2, response.Count);
        Assert.Contains(response, r => r.RoomNumber == "101");
        Assert.Contains(response, r => r.RoomNumber == "102");
        Assert.DoesNotContain(response, r => r.RoomNumber == "103");
    }

    [Fact]
    public async Task GetAvailability_ShouldExcludeOccupiedRooms()
    {
        // Arrange
        await SeedData();
        var request = new GetAvailabilityRequest
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(8))
        };

        // Act
        var result = await _controller.GetAvailability(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<GetAvailabilityResponse>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<GetAvailabilityResponse>>(okResult.Value).ToList();

        Assert.Single(response);
        Assert.Equal("102", response[0].RoomNumber);
    }

    [Fact]
    public async Task GetAvailability_ShouldFilterByCapacity()
    {
        // Arrange
        await SeedData();
        var request = new GetAvailabilityRequest
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
            MinCapacity = 3
        };

        // Act
        var result = await _controller.GetAvailability(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<GetAvailabilityResponse>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<GetAvailabilityResponse>>(okResult.Value).ToList();

        Assert.Single(response);
        Assert.Equal("102", response[0].RoomNumber);
    }
}
