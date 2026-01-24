using Hotel.Api.Guests.Dtos;
using Hotel.Api.Reservations.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Guest;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Hotel.Application.Domain.Repositories.IReservationRepository;

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
        if (request.CheckIn >= request.CheckOut)
        {
            return BadRequest("Invalid dates - CheckOut has to be later than CheckIn");
        }

        var reservation = new Reservation()
        {
            RoomId = request.RoomId,
            GuestId = request.GuestId,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            GuestsCount = request.GuestsCount
        };
        var (result, id) = await createReservationUseCase.ExecuteAsync(reservation, ct);

        switch (result)
        {
            case ReservationResult.Conflict:
                return Conflict("Reservation would conflict with another");

            case ReservationResult.InvalidRoomOrGuest:
                return BadRequest("Room or Guest with given Id was not found");

            case ReservationResult.RoomTooSmall:
                return BadRequest("Room is too small for a given number of guests");
        }

        // this should never happen
        if (id == null)
        {
            return StatusCode(500, "Reservation creation result Ok, but no id returned");
        }

        return Ok(new CreateReservationResponse()
        {
            Id = (int) id
        });
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
