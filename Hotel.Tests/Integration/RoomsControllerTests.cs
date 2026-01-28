using Hotel.Api.Rooms;
using Hotel.Api.Rooms.Dtos;
using Hotel.Api.Rooms.Validators;
using FluentValidation;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Rooms;
using Hotel.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hotel.Tests.Integration;

public class RoomsControllerTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly RoomsController _controller;

    public RoomsControllerTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var roomRepository = new RoomRepository(_dbContext);

        var getRoomsUseCase = new GetRoomsUseCase(roomRepository);
        var getRoomByIdUseCase = new GetRoomByIdUseCase(roomRepository);
        var createRoomUseCase = new CreateRoomUseCase(roomRepository);
        var updateRoomUseCase = new UpdateRoomUseCase(roomRepository);
        var deleteRoomUseCase = new DeleteRoomUseCase(roomRepository);
        var loggerMock = new Mock<ILogger<RoomsController>>();

        _controller = new RoomsController(
            getRoomsUseCase,
            getRoomByIdUseCase,
            createRoomUseCase,
            updateRoomUseCase,
            deleteRoomUseCase,
            loggerMock.Object
        );
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task CreateRoom_ShouldAddRoomToDatabase()
    {
        // Arrange
        var request = new CreateRoomRequest
        {
            Number = "101",
            Capacity = 2,
            PricePerNight = 100,
            IsActive = true,
            Type = "Standard"
        };

        // Act
        var result = await _controller.CreateRoom(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CreateRoomResponse>>(result);
        var response = Assert.IsType<CreateRoomResponse>(actionResult.Value);
        
        var roomInDb = await _dbContext.Rooms.FindAsync(response.Id);
        Assert.NotNull(roomInDb);
        Assert.Equal("101", roomInDb.Number);
        Assert.Equal(2, roomInDb.Capacity);
        Assert.Equal(100, roomInDb.PricePerNight);
        Assert.True(roomInDb.IsActive);
        Assert.Equal("Standard", roomInDb.Type);
    }

    [Fact]
    public async Task GetRoom_ShouldReturnRoom_WhenRoomExists()
    {
        // Arrange
        var request = new CreateRoomRequest
        {
            Number = "102",
            Capacity = 3,
            PricePerNight = 150,
            IsActive = true,
            Type = "Deluxe"
        };
        var createResult = await _controller.CreateRoom(request, CancellationToken.None);
        var createdId = ((CreateRoomResponse)((ActionResult<CreateRoomResponse>)createResult).Value!).Id;

        // Act
        var result = await _controller.GetRoom(createdId, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<GetRoomResponse>>(result);
        var response = Assert.IsType<GetRoomResponse>(actionResult.Value);
        
        Assert.Equal(createdId, response.Id);
        Assert.Equal("102", response.Number);
        Assert.Equal("Deluxe", response.Type);
    }

    [Fact]
    public async Task GetRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        // Act
        var result = await _controller.GetRoom(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetRooms_ShouldReturnFilteredRooms()
    {
        // Arrange
        await _controller.CreateRoom(new CreateRoomRequest { Number = "201", Capacity = 2, IsActive = true }, CancellationToken.None);
        await _controller.CreateRoom(new CreateRoomRequest { Number = "202", Capacity = 4, IsActive = true }, CancellationToken.None);
        await _controller.CreateRoom(new CreateRoomRequest { Number = "203", Capacity = 2, IsActive = false }, CancellationToken.None);

        var request = new GetRoomsRequest
        {
            MinCapacity = 3,
            OnlyActive = true,
            PageSize = 10
        };

        // Act
        var result = await _controller.GetRooms(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<GetRoomsResponsePage>>(result);
        var response = Assert.IsType<GetRoomsResponsePage>(actionResult.Value);

        Assert.Single(response.Rooms);
        Assert.Equal("202", response.Rooms.First().Number);
    }

    [Fact]
    public async Task UpdateRoom_ShouldUpdateRoomDetails()
    {
        // Arrange
        var createResult = await _controller.CreateRoom(new CreateRoomRequest { Number = "301", Capacity = 2 }, CancellationToken.None);
        var createdId = ((CreateRoomResponse)((ActionResult<CreateRoomResponse>)createResult).Value!).Id;

        var updateRequest = new UpdateRoomRequest
        {
            Number = "301-Updated",
            Capacity = 5,
            PricePerNight = 200,
            IsActive = true,
            Type = "Suite"
        };

        // Act
        var result = await _controller.UpdateRoom(createdId, updateRequest, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var roomInDb = await _dbContext.Rooms.FindAsync(createdId);
        Assert.NotNull(roomInDb);
        Assert.Equal("301-Updated", roomInDb.Number);
        Assert.Equal(5, roomInDb.Capacity);
        Assert.Equal(200, roomInDb.PricePerNight);
        Assert.Equal("Suite", roomInDb.Type);
    }

    [Fact]
    public async Task UpdateRoom_ShouldReturnConflict_WhenNumberAlreadyExists()
    {
        // Arrange
        await _controller.CreateRoom(new CreateRoomRequest { Number = "401" }, CancellationToken.None);
        var createResult2 = await _controller.CreateRoom(new CreateRoomRequest { Number = "402" }, CancellationToken.None);
        var id2 = ((CreateRoomResponse)((ActionResult<CreateRoomResponse>)createResult2).Value!).Id;

        var updateRequest = new UpdateRoomRequest
        {
            Number = "401", // Conflict with first room
            Capacity = 2
        };

        // Act
        var result = await _controller.UpdateRoom(id2, updateRequest, CancellationToken.None);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("Room number already exists.", conflictResult.Value);
    }

    [Fact]
    public async Task DeleteRoom_ShouldSoftDeleteRoom()
    {
        // Arrange
        var createResult = await _controller.CreateRoom(new CreateRoomRequest { Number = "501", IsActive = true }, CancellationToken.None);
        var createdId = ((CreateRoomResponse)((ActionResult<CreateRoomResponse>)createResult).Value!).Id;

        // Act
        var result = await _controller.DeleteRoom(createdId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var roomInDb = await _dbContext.Rooms.FindAsync(createdId);
        Assert.NotNull(roomInDb);
        Assert.False(roomInDb.IsActive);
    }

    [Theory]
    [InlineData("", 2, 100, "Standard", "Number is empty")]
    [InlineData("101", 0, 100, "Standard", "Capacity is 0")]
    [InlineData("101", -1, 100, "Standard", "Capacity is negative")]
    [InlineData("101", 2, 0, "Standard", "PricePerNight is 0")]
    [InlineData("101", 2, -1, "Standard", "PricePerNight is negative")]
    [InlineData("101", 2, 100, "", "Type is empty")]
    public void CreateRoomRequest_Validation_ShouldFailForInvalidData(string number, int capacity, decimal pricePerNight, string type, string reason)
    {
        // Arrange
        var validator = new CreateRoomRequestValidator();
        var request = new CreateRoomRequest
        {
            Number = number,
            Capacity = capacity,
            PricePerNight = pricePerNight,
            Type = type
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid, $"Validation should fail because: {reason}");
    }

    [Fact]
    public void CreateRoomRequest_Validation_ShouldSucceedForValidData()
    {
        // Arrange
        var validator = new CreateRoomRequestValidator();
        var request = new CreateRoomRequest
        {
            Number = "101",
            Capacity = 2,
            PricePerNight = 100,
            Type = "Standard"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", 2, 100, "Standard", "Number is empty")]
    [InlineData("101", 0, 100, "Standard", "Capacity is 0")]
    [InlineData("101", -1, 100, "Standard", "Capacity is negative")]
    [InlineData("101", 2, 0, "Standard", "PricePerNight is 0")]
    [InlineData("101", 2, -1, "Standard", "PricePerNight is negative")]
    [InlineData("101", 2, 100, "", "Type is empty")]
    public void UpdateRoomRequest_Validation_ShouldFailForInvalidData(string number, int capacity, decimal pricePerNight, string type, string reason)
    {
        // Arrange
        var validator = new UpdateRoomRequestValidator();
        var request = new UpdateRoomRequest
        {
            Number = number,
            Capacity = capacity,
            PricePerNight = pricePerNight,
            Type = type
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid, $"Validation should fail because: {reason}");
    }
}
