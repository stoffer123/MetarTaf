using Application.DTO;
using Application.Mappers;
using Domain.Entities;
using Domain.Factories;
using Domain.Ports;
using Domain.ValueObjects;

namespace Application
{
    public class AirportController
    {
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

        public void ResetFetchTimerAfterFetch()
        {
            lock (timerLock)
            {
                StopFetchTimer();
                StartFetchTimer();
                Console.WriteLine("Fetch timer reset after fetch.");
            }
        }

        private void StopFetchTimer()
        {
            lock (timerLock)
            {
                fetchTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine("Fetch timer stopped.");
            }
        }

        private void StartFetchTimer()
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
                IAirport? airport = infoStation.GetObservers().FirstOrDefault(a => a.airportInfo.icaoId == icao);
                if (airport != null)
                {
                    airport.incrementReferenceCount();
                    return airport;
                }
            }

            var created = await airportFactory.CreateAsync(icao, ct);
            if (created is null) return null;

            lock (lockObject)
            {
                IAirport? airport = infoStation.GetObservers().FirstOrDefault(a => a.airportInfo.icaoId == icao);
                if (airport != null)
                {
                    // en anden tråd nåede at lave den i mellemtiden
                    created.Dispose();
                    airport.incrementReferenceCount();
                    return airport;
                }

              
                infoStation.addObserver(created);
                return created;
            }
        }

        public void releaseAirport(string icao)
        {
            lock (lockObject)
            {
                IAirport? airport = infoStation.GetObservers().FirstOrDefault(a => a.airportInfo.icaoId == icao);
                if (airport != null)
                {
                    airport.decrementReferenceCount();
                    if (airport.getReferenceCount() < 1)
                    {
                        infoStation.removeObserver(airport);
                        airport.Dispose();
                    }
                }
            }
        }

        public Task<bool> ForceFetchAsync(CancellationToken ct = default)
             => infoStation.FetchNewReportsAsync(ct);

        public async Task<AirportOverviewDto?> GetOverviewAsync(string icao, CancellationToken ct = default)
        {
            var ap = await GetAirportAsync(icao, ct);
            return ap is null ? null : AirportMapper.ToOverviewDto(ap);
        }

        // Snapshotter alle trackede ICAO’er og returnerer DTO’er i en batch
        public Task<IReadOnlyList<AirportOverviewDto>> GetAllOverviewsAsync(CancellationToken ct = default)
        {
            List<IAirport> snapshot;
            lock (lockObject)
            {
                snapshot = infoStation.GetObservers().ToList();
            }

            var list = snapshot
                .Select(AirportMapper.ToOverviewDto)
                .ToList()
                .AsReadOnly();

            return Task.FromResult((IReadOnlyList<AirportOverviewDto>)list);
        }


    }
}
