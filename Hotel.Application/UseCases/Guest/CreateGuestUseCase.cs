using Hotel.Application.Domain.Models;
using Hotel.Application.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.UseCases.Guest
{
    public class CreateGuestUseCase (IGuestRepository guestRepository)
    {
        public Task<int> ExecuteAsync(Domain.Models.Guest guest, CancellationToken ct)
        {
            return guestRepository.AddAsync(guest, ct);
        }
    }
}
