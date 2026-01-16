using Hotel.Api.Guests.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Guests;

[ApiController]
[Route("api/guests")]
public class GuestsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetGuestsResponseItem>>> GetGuests(
        [FromQuery] GetGuestsRequest request,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetGuestResponse>> GetGuest(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<CreateGuestResponse>> CreateGuest(CreateGuestRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGuest(int id, UpdateGuestRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
