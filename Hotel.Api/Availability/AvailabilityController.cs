using Hotel.Api.Availability.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Availability;

[ApiController]
[Route("api/availability")]
public class AvailabilityController(
    GetAvailableRoomsUseCase getAvailableRoomsUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetAvailabilityResponse>>> GetAvailability(
        [FromQuery] GetAvailabilityRequest request,
        CancellationToken ct)
    {
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
