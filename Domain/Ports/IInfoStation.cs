using Domain.Reports;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    public interface IInfoStation
    {
        void removeObserver(IAirport observer);
        void addObserver(IAirport observer);
        void notifyTafChange();
        void notifyMetarChange();
        void notifyAirportInfoChange();
        Dictionary<DateTime, MetarReport> getMetars(string icao);
        Dictionary<DateTime, TafReport> getTafs(string icao);
        Task<bool> FetchNewReportsAsync(CancellationToken ct = default);
        List<string> GetObserverIcaos();
        ImmutableList<IAirport> GetObservers();
    }
}
