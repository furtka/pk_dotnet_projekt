using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Guest
{
    public class GetAvailableRoomsUseCase(IAvailabilityRepository availabilityRepository)
    {
        public Task<List<Room>> ExecuteAsync(AvailabilityFilter f, CancellationToken ct)
        {
            return availabilityRepository.GetAvailable(f, ct);
        }
    }
}
