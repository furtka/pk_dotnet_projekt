using Hotel.Api.Guests;
using Hotel.Api.Guests.Dtos;
using Hotel.Api.Guests.Validators;
using FluentValidation;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Hotel.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hotel.Tests.Integration;

public class GuestsControllerTests : IDisposable
{
    private readonly Hotel.Infrastructure.HotelDbContext _dbContext;
    private readonly GuestsController _controller;

    public GuestsControllerTests()
    {
        _dbContext = TestDbContextFactory.Create();
        var guestRepository = new GuestRepository(_dbContext);

        var getGuestsUseCase = new GetGuestsUseCase(guestRepository);
        var createGuestUseCase = new CreateGuestUseCase(guestRepository);
        var getGuestByIdUseCase = new GetGuestByIdUseCase(guestRepository);
        var updateGuestUseCase = new UpdateGuestUseCase(guestRepository);
        var loggerMock = new Mock<ILogger<GuestsController>>();

        _controller = new GuestsController(
            getGuestsUseCase,
            createGuestUseCase,
            getGuestByIdUseCase,
            updateGuestUseCase,
            loggerMock.Object
        );
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task CreateGuest_ShouldAddGuestToDatabase()
    {
        // Arrange
        var request = new CreateGuestRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "123456789"
        };

        // Act
        var result = await _controller.CreateGuest(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<CreateGuestResponse>>(result);
        var response = Assert.IsType<CreateGuestResponse>(actionResult.Value);
        
        var guestInDb = await _dbContext.Guests.FindAsync(response.Id);
        Assert.NotNull(guestInDb);
        Assert.Equal("John", guestInDb.FirstName);
        Assert.Equal("Doe", guestInDb.LastName);
        Assert.Equal("john.doe@example.com", guestInDb.Email);
        Assert.Equal("123456789", guestInDb.Phone);
    }

    [Fact]
    public async Task GetGuest_ShouldReturnGuest_WhenGuestExists()
    {
        // Arrange
        var request = new CreateGuestRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "987654321"
        };
        var createResult = await _controller.CreateGuest(request, CancellationToken.None);
        var createdId = ((CreateGuestResponse)((ActionResult<CreateGuestResponse>)createResult).Value!).Id;

        // Act
        var result = await _controller.GetGuest(createdId, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<GetGuestResponse>>(result);
        var response = Assert.IsType<GetGuestResponse>(actionResult.Value);
        
        Assert.Equal(createdId, response.Id);
        Assert.Equal("Jane", response.FirstName);
        Assert.Equal("Smith", response.LastName);
        Assert.Equal("jane.smith@example.com", response.Email);
        Assert.Equal("987654321", response.Phone);
    }

    [Fact]
    public async Task GetGuest_ShouldReturnNotFound_WhenGuestDoesNotExist()
    {
        // Act
        var result = await _controller.GetGuest(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetGuests_ShouldReturnAllGuests()
    {
        // Arrange
        await _controller.CreateGuest(new CreateGuestRequest { FirstName = "A", LastName = "B", Email = "a@b.com" }, CancellationToken.None);
        await _controller.CreateGuest(new CreateGuestRequest { FirstName = "C", LastName = "D", Email = "c@d.com" }, CancellationToken.None);

        var request = new GetGuestsRequest();

        // Act
        var result = await _controller.GetGuests(request, CancellationToken.None);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<GetGuestsResponseItem>>>(result);
        var response = Assert.IsType<List<GetGuestsResponseItem>>(actionResult.Value);

        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task UpdateGuest_ShouldUpdateGuestDetails()
    {
        // Arrange
        var createResult = await _controller.CreateGuest(new CreateGuestRequest { FirstName = "Old", LastName = "Name", Email = "old@email.com" }, CancellationToken.None);
        var createdId = ((CreateGuestResponse)((ActionResult<CreateGuestResponse>)createResult).Value!).Id;

        var updateRequest = new UpdateGuestRequest
        {
            FirstName = "New",
            LastName = "Name",
            Email = "new@email.com",
            Phone = "111222333"
        };

        // Act
        var result = await _controller.UpdateGuest(createdId, updateRequest, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var guestInDb = await _dbContext.Guests.FindAsync(createdId);
        Assert.NotNull(guestInDb);
        Assert.Equal("New", guestInDb.FirstName);
        Assert.Equal("Name", guestInDb.LastName);
        Assert.Equal("new@email.com", guestInDb.Email);
        Assert.Equal("111222333", guestInDb.Phone);
    }

    [Theory]
    [InlineData("", "Doe", "john@doe.com", "FirstName is empty")]
    [InlineData("John", "", "john@doe.com", "LastName is empty")]
    [InlineData("John", "Doe", "invalid-email", "Invalid email format")]
    [InlineData("John", "Doe", "john@doe.com", "Invalid phone format", "123")]
    public void CreateGuestRequest_Validation_ShouldFailForInvalidData(string firstName, string lastName, string email, string reason, string? phone = null)
    {
        // Arrange
        var validator = new CreateGuestRequestValidator();
        var request = new CreateGuestRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid, $"Validation should fail because: {reason}");
    }

    [Fact]
    public void CreateGuestRequest_Validation_ShouldSucceedForValidData()
    {
        // Arrange
        var validator = new CreateGuestRequestValidator();
        var request = new CreateGuestRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+48 123 456 789"
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Doe", "john@doe.com", "FirstName is empty")]
    [InlineData("John", "", "john@doe.com", "LastName is empty")]
    [InlineData("John", "Doe", "invalid-email", "Invalid email format")]
    [InlineData("John", "Doe", "john@doe.com", "Invalid phone format", "123")]
    public void UpdateGuestRequest_Validation_ShouldFailForInvalidData(string firstName, string lastName, string email, string reason, string? phone = null)
    {
        // Arrange
        var validator = new UpdateGuestRequestValidator();
        var request = new UpdateGuestRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid, $"Validation should fail because: {reason}");
    }
}
