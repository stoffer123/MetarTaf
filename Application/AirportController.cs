using Domain.Factories;
using Domain.Ports;
using Domain.ValueObjects;

namespace Application
{
    public class AirportController : IAirportController
    {
        private readonly Dictionary<string, IAirport> airports = new();
        private readonly IInfoStation infoStation;
        private readonly AirportFactory airportFactory;
        private readonly object lockObject = new();

        private Timer fetchTimer;
        private readonly object timerLock = new();
        private readonly int timerDelayMinutes;

        public AirportController(IInfoStation infoStation, AirportFactory airportFactory, int timerDelayMinutes = 1)
        {
            this.timerDelayMinutes = timerDelayMinutes;

            this.infoStation = infoStation;
            this.airportFactory = airportFactory;

            InitializeFetchTimer();

        }

        private void InitializeFetchTimer()
        {
            fetchTimer = new Timer(async _ => await infoStation.FetchNewReportsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(timerDelayMinutes));
            ResetFetchTimerAfterFetch();
        }

        public void ResetFetchTimer()
        {
            lock (timerLock)
            {
                StopFetchTimer();
                StartFetchTimer();
                Console.WriteLine("Fetch timer reset.");
            }
        }

        public void ResetFetchTimerAfterFetch()
        {
            lock (timerLock)
            {
                StopFetchTimer();
                StartFetchTimer();
                Console.WriteLine("Fetch timer reset after fetch.");
            }
        }

        public void StopFetchTimer()
        {
            lock (timerLock)
            {
                fetchTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine("Fetch timer stopped.");
            }
        }

        public void StartFetchTimer()
        {
            lock (timerLock)
            {
                fetchTimer?.Change(TimeSpan.FromMinutes(timerDelayMinutes), TimeSpan.FromMinutes(timerDelayMinutes));
                Console.WriteLine("Fetch timer started.");
            }
        }

        public async Task<IAirport?> GetAirportAsync(string icao, CancellationToken ct = default)
        {
            lock (lockObject)
            {
                if (airports.TryGetValue(icao, out var existing))
                {
                    existing.incrementReferenceCount();
                    return existing;
                }
            }

            var created = await airportFactory.CreateAsync(icao, ct);
            if (created is null) return null;

            lock (lockObject)
            {
                if (airports.TryGetValue(icao, out var racing))
                {
                    // en anden tråd nåede at lave den i mellemtiden
                    created.Dispose();
                    racing.incrementReferenceCount();
                    return racing;
                }

                airports.Add(created.getAirportInfo().icaoId, created);
                infoStation.addObserver(created);
                return created;
            }
        }

        public void releaseAirport(string icao)
        {
            lock (lockObject)
            {
                if (airports.TryGetValue(icao, out var airport))
                {
                    airport.decrementReferenceCount();
                    if (airport.getReferenceCount() < 1)
                    {
                        airports.Remove(icao);
                        infoStation.removeObserver(airport);
                        airport.Dispose();
                    }
                }
            }
        }



        public List<string> getAirportIcaoList() => airports.Keys.ToList();
    }
}
