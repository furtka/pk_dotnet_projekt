using Hotel.Api.Reservations.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Reservations;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<GetReservationResponse>> GetReservation(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelReservation(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
