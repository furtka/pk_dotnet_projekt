using Hotel.Api.Guests.Dtos;
using Hotel.Api.Reservations.Dtos;
using Hotel.Application.Domain.Models;
using Hotel.Application.UseCases.Reservations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Hotel.Application.Domain.Repositories.IReservationRepository;

namespace Hotel.Api.Reservations;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(
    GetReservationByIdUseCase getReservationByIdUseCase,
    CreateReservationUseCase createReservationUseCase,
    RemoveReservationUseCase removeReservationUseCase,
    ILogger<ReservationsController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves a specific reservation by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The reservation details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetReservationResponse?>> GetReservation(int id, CancellationToken ct)
    {
        logger.LogInformation("Getting reservation with Id: {ReservationId}", id);
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

    /// <summary>
    /// Creates a new reservation.
    /// </summary>
    /// <param name="request">The reservation details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The unique identifier of the newly created reservation.</returns>
    [HttpPost]
    public async Task<ActionResult<CreateReservationResponse>> CreateReservation(CreateReservationRequest request, CancellationToken ct)
    {
        logger.LogInformation("Creating reservation for RoomId: {RoomId}, GuestId: {GuestId}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, GuestsCount: {GuestsCount}",
            request.RoomId, request.GuestId,  request.CheckIn, request.CheckOut, request.GuestsCount);

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

    /// <summary>
    /// Cancels an existing reservation.
    /// </summary>
    /// <param name="id">The unique identifier of the reservation to cancel.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>NoContent if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelReservation(int id, CancellationToken ct)
    {
        logger.LogInformation("Cancelling reservation with Id: {ReservationId}", id);
        var result = await removeReservationUseCase.ExecuteAsync(id, ct);

        switch (result)
        {
            default:
            case ReservationCancellationResult.Ok:
                return NoContent();

            case ReservationCancellationResult.TooLateToCancel:
                return BadRequest("Reservation was not cancelled as it is too late to cancel it.");

            case ReservationCancellationResult.AlreadyInactive:
                return NotFound();
        }
    }
}
