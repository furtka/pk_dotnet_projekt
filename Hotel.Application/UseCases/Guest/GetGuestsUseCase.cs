using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Guest
{
    public class GetGuestsUseCase (IGuestRepository guestRepository)
    {
        public Task<List<Domain.Models.Guest>> ExecuteAsync(CancellationToken ct)
        {
            return guestRepository.GetAllAsync(ct);
        }
    }
}
