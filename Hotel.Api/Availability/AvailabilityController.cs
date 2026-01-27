using Hotel.Api.Availability.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Availability;

[ApiController]
[Route("api/availability")]
public class AvailabilityController : ControllerBase
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
        throw new NotImplementedException();
    }
}
