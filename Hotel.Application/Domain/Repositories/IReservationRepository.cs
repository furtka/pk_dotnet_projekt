using Hotel.Application.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Domain.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int id, CancellationToken ct);
        Task<int?> AddAsync(Reservation resv, CancellationToken ct);

        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
