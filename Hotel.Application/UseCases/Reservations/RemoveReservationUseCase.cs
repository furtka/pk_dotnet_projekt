using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Guest
{
    public class RemoveReservationUseCase(IReservationRepository reservationRepository)
    {
        public Task<bool> ExecuteAsync(int id, CancellationToken ct)
        {
            return reservationRepository.DeleteAsync(id, ct);
        }
    }
}
