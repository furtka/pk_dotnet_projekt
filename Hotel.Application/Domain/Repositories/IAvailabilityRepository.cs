using Hotel.Application.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Domain.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<List<Room>> GetAvailable(AvailabilityFilter f, CancellationToken ct);
    }
}
