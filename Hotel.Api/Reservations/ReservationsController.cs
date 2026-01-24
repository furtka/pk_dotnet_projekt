using Hotel.Api.Guests.Dtos;
using Hotel.Api.Reservations.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Reservations;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(
    GetReservationByIdUseCase getReservationByIdUseCase,
    CreateReservationUseCase createReservationUseCase,
    RemoveReservationUseCase removeReservationUseCase) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<GetReservationResponse?>> GetReservation(int id, CancellationToken ct)
    {
        var reservation = await getReservationByIdUseCase.ExecuteAsync(id, ct);

        if (reservation == null) return NotFound();

        return new GetReservationResponse()
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            GuestId = reservation.GuestId,
            CheckIn = reservation.CheckIn,
            CheckOut = reservation.CheckOut,
            GuestsCount = reservation.GuestsCount,
            TotalPrice = reservation.TotalPrice,
            Status = reservation.Status,
        };
    }

    [HttpPost]
    public async Task<ActionResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request, CancellationToken ct)
    {
        var reservation = new Reservation()
        {
            RoomId = request.RoomId,
            GuestId = request.GuestId,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            GuestsCount = request.GuestsCount
        };
        var resId = await createReservationUseCase.ExecuteAsync(reservation, ct);

        if (resId == null) return BadRequest();

        return new CreateReservationResponse()
        {
            Id = (int) resId
        };
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelReservation(int id, CancellationToken ct)
    {
        var result = await removeReservationUseCase.ExecuteAsync(id, ct);

        if(result)
        {
            return NoContent();
        } else
        {
            return NotFound();
        }
    }
}
