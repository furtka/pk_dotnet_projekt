using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Guest
{
    public class GetGuestByIdUseCase(IGuestRepository guestRepository)
    {
        public Task<Domain.Models.Guest?> ExecuteAsync(int id, CancellationToken ct)
        {
            return guestRepository.GetByIdAsync(id, ct);
        }
    }
}
