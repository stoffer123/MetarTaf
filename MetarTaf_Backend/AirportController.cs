using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;
using MetarTaf_Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetarTaf_Backend
{
    public class AirportController
    {
        private readonly Dictionary<string, IAirport> airports = new();
        private readonly IInfoStation infoStation;
        private readonly AirportFactory airportFactory;
        private readonly object lockObject = new();

        private Timer fetchTimer;
        private readonly object timerLock = new();
        private readonly int timerDelayMinutes;

        // Gate for at undgå overlappende fetch-kørsler
        private readonly SemaphoreSlim _fetchGate = new(1, 1);

        // NY: tag fetcher ind og giv den videre til AirportInfoStation
        public AirportController(NorthAviMetFetcher fetcher, int timerDelayMinutes = 1)
        {
            this.timerDelayMinutes = timerDelayMinutes;

            infoStation = new AirportInfoStation(this, fetcher); // <-- ændret ctor
            airportFactory = new AirportFactory(infoStation);

            InitializeFetchTimer();
        }

        private void InitializeFetchTimer()
        {
            fetchTimer = new Timer(async _ => await FetchReportsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(timerDelayMinutes));
        }

        private async Task FetchReportsAsync()
        {
            if (!await _fetchGate.WaitAsync(0)) return; // en fetch kører allerede
            try
            {
                Console.WriteLine($"Fetching reports at {DateTime.UtcNow:HH:mm:ss} UTC...");
                await infoStation.fetchNewReportsFromAPI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during fetch: {ex.Message}");
            }
            finally
            {
                _fetchGate.Release();
            }
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

        public IAirport getAirport(string icao)
        {
            lock (lockObject)
            {
                if (airports.TryGetValue(icao, out var airport))
                {
                    airport.incrementReferenceCount();
                    return airport;
                }

                airport = airportFactory.createAirport(icao);
                if (airport != null)
                {
                    airports.Add(airport.getAirportInfo().icaoId, airport);
                    infoStation.addObserver(airport);
                }
                return airport;
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
                    }
                }
            }
        }

        public List<string> getAirportIcaoList() => airports.Keys.ToList();
    }
}
