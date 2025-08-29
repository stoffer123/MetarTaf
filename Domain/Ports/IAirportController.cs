using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IAirportController
    {
        void ResetFetchTimerAfterFetch();
        Task<IAirport?> GetAirportAsync(string icao, CancellationToken ct = default);
        void releaseAirport(string icao);
    }
}
