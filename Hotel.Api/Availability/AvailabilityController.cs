using Hotel.Api.Availability.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Availability;

[ApiController]
[Route("api/availability")]
public class AvailabilityController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetAvailabilityResponse>>> GetAvailability(
        [FromQuery] GetAvailabilityRequest request,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
