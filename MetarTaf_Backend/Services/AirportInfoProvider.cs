using Domain.Ports;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Services
{
    public sealed class AirportInfoProvider : IAirportInfoProvider
    {
        public Task<AirportInfo?> GetByIcaoAsync(string icao, CancellationToken ct = default)
        {
            // Din nuværende service er sync. Wrap den.
            var info = AirportInfoService.getAirportInfo(icao);
            return Task.FromResult(info);
        }
    }
}
