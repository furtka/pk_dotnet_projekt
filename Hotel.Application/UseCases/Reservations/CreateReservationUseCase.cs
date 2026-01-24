using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Guest
{
    public class CreateReservationUseCase(IReservationRepository reservationRepository)
    {
        public Task<int?> ExecuteAsync(Reservation resv, CancellationToken ct)
        {
            var result = reservationRepository.AddAsync(resv, ct);
            if(result == null)
            {
                return null;
            }

            return result;

        }
    }
}
