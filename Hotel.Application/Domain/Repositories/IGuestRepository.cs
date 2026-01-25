using Hotel.Application.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Domain.Repositories
{
    public interface IGuestRepository
    {
        Task<Guest?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<Guest>> GetAllAsync(CancellationToken ct);
        Task<int> AddAsync(Guest guest, CancellationToken ct);
        Task UpdateAsync(Guest guest, CancellationToken ct);

        //Task DeleteAsync(Guest guest, CancellationToken ct);
    }
}
