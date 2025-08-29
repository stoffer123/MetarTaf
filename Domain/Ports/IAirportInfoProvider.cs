using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IAirportInfoProvider
    {
        Task<AirportInfo?> GetByIcaoAsync(string icao, CancellationToken ct = default);
    }
}
