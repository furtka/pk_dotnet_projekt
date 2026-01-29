using Hotel.Api.Reservations;
using Hotel.Api.Reservations.Dtos;
using Hotel.Api.Reservations.Validators;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Reservations;
using Hotel.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentValidation;

namespace Hotel.Tests.Integration;

public class ReservationsControllerTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly ReservationsController _controller;

    public ReservationsControllerTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var reservationRepository = new ReservationRepository(_dbContext);
        var roomRepository = new RoomRepository(_dbContext);
        var guestRepository = new GuestRepository(_dbContext);

        var getReservationByIdUseCase = new GetReservationByIdUseCase(reservationRepository);
        var createReservationUseCase = new CreateReservationUseCase(reservationRepository);
        var removeReservationUseCase = new RemoveReservationUseCase(reservationRepository);
        var loggerMock = new Mock<ILogger<ReservationsController>>();

        _controller = new ReservationsController(
            getReservationByIdUseCase,
            createReservationUseCase,
            removeReservationUseCase,
            loggerMock.Object
        );
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
    public async Task CreateReservation_ShouldAddReservationToDatabase_WhenDataIsValid()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest();
        var request = new CreateReservationRequest
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            GuestsCount = 2
        };

        // Act
        var result = await _controller.CreateReservation(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CreateReservationResponse>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<CreateReservationResponse>(okResult.Value);
        
        var reservationInDb = await _dbContext.Reservations.FindAsync(response.Id);
        Assert.NotNull(reservationInDb);
        Assert.Equal(roomId, reservationInDb.RoomId);
        Assert.Equal(guestId, reservationInDb.GuestId);
        Assert.Equal("Active", reservationInDb.Status);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnConflict_WhenDatesOverlap()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest();
        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var checkOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        await _controller.CreateReservation(new CreateReservationRequest
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            GuestsCount = 1
        }, CancellationToken.None);

        var overlappingRequest = new CreateReservationRequest
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = checkIn.AddDays(1),
            CheckOut = checkOut.AddDays(1),
            GuestsCount = 1
        };

        // Act
        var result = await _controller.CreateReservation(overlappingRequest, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CreateReservationResponse>>(result);
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
        Assert.Equal("Reservation would conflict with another", conflictResult.Value);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnBadRequest_WhenRoomDoesNotExist()
    {
        // Arrange
        var (_, guestId) = await SeedRoomAndGuest();
        var request = new CreateReservationRequest
        {
            RoomId = 999,
            GuestId = guestId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            GuestsCount = 1
        };

        // Act
        var result = await _controller.CreateReservation(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CreateReservationResponse>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Room or Guest with given Id was not found", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateReservation_ShouldReturnBadRequest_WhenRoomTooSmall()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest(capacity: 1);
        var request = new CreateReservationRequest
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            GuestsCount = 2 // Room capacity is 1
        };

        // Act
        var result = await _controller.CreateReservation(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CreateReservationResponse>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Room is too small for a given number of guests", badRequestResult.Value);
    }

    [Fact]
    public async Task GetReservation_ShouldReturnReservation_WhenExists()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest();
        var createResult = await _controller.CreateReservation(new CreateReservationRequest
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            GuestsCount = 1
        }, CancellationToken.None);
        var createdId = ((CreateReservationResponse)((OkObjectResult)((ActionResult<CreateReservationResponse>)createResult).Result!).Value!).Id;

        // Act
        var result = await _controller.GetReservation(createdId, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<GetReservationResponse>>(result);
        var response = Assert.IsType<GetReservationResponse>(actionResult.Value);
        Assert.Equal(createdId, response.Id);
        Assert.Equal(roomId, response.RoomId);
    }

    [Fact]
    public async Task GetReservation_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Act
        var result = await _controller.GetReservation(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CancelReservation_ShouldSetStatusToInactive_WhenValid()
    {
        // Arrange
        var (roomId, guestId) = await SeedRoomAndGuest();
        var createResult = await _controller.CreateReservation(new CreateReservationRequest
        {
            RoomId = roomId,
            GuestId = guestId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), // Future date
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
            GuestsCount = 1
        }, CancellationToken.None);
        var createdId = ((CreateReservationResponse)((OkObjectResult)((ActionResult<CreateReservationResponse>)createResult).Result!).Value!).Id;

        // Act
        var result = await _controller.CancelReservation(createdId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var reservationInDb = await _dbContext.Reservations.FindAsync(createdId);
        Assert.Equal("Inactive", reservationInDb!.Status);
    }

    [Theory]
    [InlineData(0, "CheckOut same as CheckIn")]
    [InlineData(-1, "CheckOut before CheckIn")]
    public void CreateReservationRequest_Validation_ShouldFailForInvalidDates(int daysToAdd, string reason)
    {
        // Arrange
        var validator = new CreateReservationRequestValidator();
        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new CreateReservationRequest
        {
            CheckIn = checkIn,
            CheckOut = checkIn.AddDays(daysToAdd)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid, $"Validation should fail because: {reason}");
    }

    [Fact]
    public void CreateReservationRequest_Validation_ShouldFail_WhenReservationTooLong()
    {
        // Arrange
        var validator = new CreateReservationRequestValidator();
        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var request = new CreateReservationRequest
        {
            CheckIn = checkIn,
            CheckOut = checkIn.AddDays(31)
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Reservation longer than 30 nights"));
    }

    [Fact]
    public void CreateReservationRequest_Validation_ShouldSucceedForValidData()
    {
        // Arrange
        var validator = new CreateReservationRequestValidator();
        var request = new CreateReservationRequest
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5))
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
