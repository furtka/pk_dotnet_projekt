using Hotel.Api.Guests.Dtos;
using Hotel.Api.Rooms.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Hotel.Application.UseCases.Rooms;
using Microsoft.AspNetCore.Mvc;
namespace Hotel.Api.Guests;

[ApiController]
[Route("api/guests")]
public class GuestsController(
    GetGuestsUseCase getGuestsUseCase,
    CreateGuestUseCase createGuestUseCase,
    GetGuestByIdUseCase getGuestByIdUseCase,
    UpdateGuestUseCase updateGuestUseCase
    ) : ControllerBase
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
        var guest = await getGuestByIdUseCase.ExecuteAsync(id, ct);

        if (guest is null)
        {
            return NotFound();
        }

        return new GetGuestResponse()
        {
            Id = guest.Id,
            FirstName = guest.FirstName,
            LastName = guest.LastName,
            Email = guest.Email,
            Phone = guest.Phone
        };
    }

    [HttpPost]
    public async Task<ActionResult<CreateGuestResponse>> CreateGuest(CreateGuestRequest request, CancellationToken ct)
    {
        var guest = new Guest
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone
          
        };

        var id = await createGuestUseCase.ExecuteAsync(guest, ct);

        return new CreateGuestResponse { Id = id };
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGuest(int id, UpdateGuestRequest request, CancellationToken ct)
    {
        var guest = new Guest
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone
        };

        await updateGuestUseCase.ExecuteAsync(guest, ct);

        return NoContent();
    }
}
