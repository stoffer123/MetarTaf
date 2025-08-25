using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IAirportRepository
    {
        Task<Airport?> GetAsync(string icao, CancellationToken ct = default);
        Task UpsertAsync(Airport airport, CancellationToken ct = default);
    }
}
