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
    UpdateGuestUseCase updateGuestUseCase,
    ILogger<GuestsController> logger
    ) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all guests.
    /// </summary>
    /// <param name="request">The filtering criteria (currently unused but reserved for future use).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of guests.</returns>
    [HttpGet]
    public async Task<ActionResult<List<GetGuestsResponseItem>>> GetGuests(
        [FromQuery] GetGuestsRequest request,
        CancellationToken ct)
    {
        logger.LogInformation("Getting guests");
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

    /// <summary>
    /// Retrieves a specific guest by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the guest.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The guest details if found; otherwise, NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetGuestResponse>> GetGuest(int id, CancellationToken ct)
    {
        logger.LogInformation("Getting guest with id: {Id}", id);
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

    /// <summary>
    /// Creates a new guest record.
    /// </summary>
    /// <param name="request">The guest details to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The unique identifier of the newly created guest.</returns>
    [HttpPost]
    public async Task<ActionResult<CreateGuestResponse>> CreateGuest(CreateGuestRequest request, CancellationToken ct)
    {
        logger.LogInformation("Creating guest: {FirstName} {LastName}", request.FirstName, request.LastName);
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

    /// <summary>
    /// Updates an existing guest's information.
    /// </summary>
    /// <param name="id">The unique identifier of the guest to update.</param>
    /// <param name="request">The updated guest details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent if successful.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGuest(int id, UpdateGuestRequest request, CancellationToken ct)
    {
        logger.LogInformation("Updating guest with id: {Id}", id);
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
