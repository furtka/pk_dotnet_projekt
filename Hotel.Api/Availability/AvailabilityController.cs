using Hotel.Api.Availability.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Availability;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Availability;

[ApiController]
[Route("api/availability")]
public class AvailabilityController(
    GetAvailableRoomsUseCase getAvailableRoomsUseCase,
    ILogger<AvailabilityController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves room availability for a given date range.
    /// </summary>
    /// <param name="request">The date range for availability check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of available rooms and their status.</returns>
    [HttpGet]
    public async Task<ActionResult<List<GetAvailabilityResponse>>> GetAvailability(
        [FromQuery] GetAvailabilityRequest request,
        CancellationToken ct)
    {
        logger.LogInformation("Getting availability for dates: {CheckIn} - {CheckOut}", request.CheckIn, request.CheckOut);

        if (request.CheckIn >= request.CheckOut)
        {
            return BadRequest("Invalid dates - CheckOut has to be later than CheckIn");
        }
        else if (request.CheckOut.DayNumber - request.CheckIn.DayNumber > 30)
        {
            return BadRequest("Invalid dates - Reservation longer than 30 nights");
        } else if (request.CheckIn <= DateOnly.FromDateTime(DateTime.Now))
        {
            return BadRequest("Invalid check in date - date needs to be in the future");
        }

        var filter = new AvailabilityFilter()
        {
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            MinCapacity = request.MinCapacity
        };

        var roomsAvailable = await getAvailableRoomsUseCase.ExecuteAsync(filter, ct);

        return Ok(roomsAvailable.Select(r => new GetAvailabilityResponse()
        {
            RoomId = r.Id,
            RoomNumber = r.Number,
            Capacity = r.Capacity,
            Price = r.PricePerNight * (filter.CheckOut.DayNumber - filter.CheckIn.DayNumber)
        }));
    }
}
