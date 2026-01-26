using Hotel.Api.Reservations.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Reservations;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    /// <summary>
    /// Retrieves a specific reservation by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The reservation details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetReservationResponse>> GetReservation(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a new reservation.
    /// </summary>
    /// <param name="request">The reservation details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The unique identifier of the newly created reservation.</returns>
    [HttpPost]
    public async Task<ActionResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancels an existing reservation.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation to cancel.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelReservation(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
