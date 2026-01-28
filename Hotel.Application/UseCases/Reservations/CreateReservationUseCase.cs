using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hotel.Application.Domain.Repositories.IReservationRepository;

namespace Hotel.Application.UseCases.Reservations
{
    public class CreateReservationUseCase(IReservationRepository reservationRepository)
    {
        public Task<(ReservationResult, int?)> ExecuteAsync(Reservation resv, CancellationToken ct)
        {
            return reservationRepository.AddAsync(resv, ct);
        }
    }
}
