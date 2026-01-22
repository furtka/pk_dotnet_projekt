using Hotel.Api.Guests.Dtos;
using Hotel.Api.Rooms.Dtos;
using Hotel.Application.UseCases.Guest;
using Hotel.Application.UseCases.Rooms;
using Microsoft.AspNetCore.Mvc;
namespace Hotel.Api.Guests;

[ApiController]
[Route("api/guests")]
public class GuestsController(
    GetGuestsUseCase getGuestsUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetGuestsResponseItem>>> GetGuests(
        [FromQuery] GetGuestsRequest request,
        CancellationToken ct)
    {
        var guests = await getGuestsUseCase.ExecuteAsync(ct);

        return guests.Select(g => new GetGuestsResponseItem()
        {
            Id = g.Id,
            FirstName = g.FirstName,
            LastName =g.LastName,
            Email = g.Email, 
            Phone = g.Phone,
        }).ToList();
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
