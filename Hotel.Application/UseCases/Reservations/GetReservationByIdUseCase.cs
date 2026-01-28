using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Reservations
{
    public class GetReservationByIdUseCase(IReservationRepository reservationRepository)
    {
        public Task<Domain.Models.Reservation?> ExecuteAsync(int id, CancellationToken ct)
        {
            return reservationRepository.GetByIdAsync(id, ct);
        }
    }
}
