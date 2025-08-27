using Domain.Reports;
using Domain.ValueObjects;


namespace MetarTaf_Backend.Models
{
    internal class Airport : IAirport
    {
        private IInfoStation infoStation;
        public AirportInfo airportInfo { get; }
        public Dictionary<DateTime, MetarReport> metars { get; }
        public Dictionary<DateTime, TafReport> tafs { get; }
        public string icao { get; }
        private int referenceCount;
        private readonly object _gate = new();
        private readonly object _debounceGate = new();
        private System.Threading.Timer? _debounceTimer;
        private static readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(50);

        public event Action? Updated;


        public Airport(IInfoStation infoStation, AirportInfo airportInfo)
        {
            this.infoStation = infoStation;
            this.airportInfo = airportInfo;
            metars = new Dictionary<DateTime, MetarReport>();
            tafs = new Dictionary<DateTime, TafReport>();
            icao = airportInfo.icaoId;
            referenceCount = 1;
            
        }

        public void updateAirportInfo()
        {
            throw new NotImplementedException();
        }

        public void updateMetars()
        {
            var newMetars = infoStation.getMetars(icao);
            var added = false;
            lock (_gate)
            {
                foreach (var kvp in newMetars)
                    if (metars.TryAdd(kvp.Key, kvp.Value))
                        added = true;
            }
            if (added)
            {
                RaiseUpdated();
            }
        }

        public void updateTafs()
        {
            var newTafs = infoStation.getTafs(icao);
            var added = false;
            lock (_gate)
            {
                foreach (var kvp in newTafs)
                    if (tafs.TryAdd(kvp.Key, kvp.Value))
                        added = true;
            }
            if (added)
            {
                RaiseUpdated();
            }
        }

        private void RaiseUpdated()
        {
            // Coalesce flere kald inden for debounce-vinduet til ét event
            lock (_debounceGate)
            {
                // nulstil (så vi udskyder, hvis der kommer et nyt “ping” hurtigt efter)
                _debounceTimer?.Dispose();
                _debounceTimer = new System.Threading.Timer(_ =>
                {
                    // fyr eventet udenfor lock
                    var h = Updated;
                    h?.Invoke();
                }, null, _debounceDelay, System.Threading.Timeout.InfiniteTimeSpan);
            }
        }

        public void incrementReferenceCount()
        {
            referenceCount++;
        }

        public void decrementReferenceCount()
        {
            referenceCount--;
        }

        public int getReferenceCount()
        {
            return referenceCount;
        }

        public AirportInfo getAirportInfo()
        {
            return airportInfo;
        }

        public IReadOnlyDictionary<DateTime, MetarReport> getMetars()
        {
            lock (_gate)
            {
                return new Dictionary<DateTime, MetarReport>(metars);
            }
            
        }

        public IReadOnlyDictionary<DateTime, TafReport> getTafs()
        {
            lock (_gate)
            {
                return new Dictionary<DateTime, TafReport>(tafs);
            }
        }

        public void Dispose()
        {
            lock (_debounceGate)
            {
                _debounceTimer?.Dispose();
                _debounceTimer = null;
            }
        }

    }
}
