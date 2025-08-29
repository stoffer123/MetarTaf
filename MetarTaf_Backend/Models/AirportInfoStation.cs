using MetarTaf_Backend.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MetarTaf_Backend.Services;
using Domain.Reports;
using Domain.Ports;
using System.Collections.Immutable;

namespace MetarTaf_Backend.Models
{
    public class AirportInfoStation : IInfoStation
    {
        private readonly List<IAirport> observers;
        private readonly MetarService metarService;
        private readonly TafService tafService;

        // Sikrer at kun en fetch kan kører ad gangen
        private readonly SemaphoreSlim _fetchGate = new(1, 1);

        public AirportInfoStation(IOpmetFetcher fetcher)
        {
            observers = new List<IAirport>();
            metarService = new MetarService(this, fetcher);
            tafService   = new TafService(this, fetcher);
        }

        public ImmutableList<IAirport> GetObservers()
        {
            return observers.ToImmutableList();
        }

        public List<string> GetObserverIcaos()
        {
            var icaoSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var observer in observers)
            {
                icaoSet.Add(observer.airportInfo.icaoId);
            }
            return icaoSet.ToList();
        }

        public async void addObserver(IAirport observer)
        {
            observers.Add(observer);
            // fire-and-forget men med gate, så vi ikke kører flere fetches parallelt
            await FetchNewReportsAsync();
        }

        public void removeObserver(IAirport observer)
        {
            observers.Remove(observer);
        }

        public void notifyAirportInfoChange()
        {
            foreach (IAirport observer in observers)
            {
                observer.updateAirportInfo();
            }
        }

        public void notifyMetarChange()
        {
            foreach (IAirport observer in observers)
            {
                observer.updateMetars();
            }
        }

        public void notifyTafChange()
        {
            foreach (IAirport observer in observers)
            {
                observer.updateTafs();
            }
        }

        public Dictionary<DateTime, MetarReport> getMetars(string icao)
        {
            return metarService.getMetars(icao);
        }

        public Dictionary<DateTime, TafReport> getTafs(string icao)
        {
            return tafService.getTafs(icao);
        }

        public async Task<bool> FetchNewReportsAsync(CancellationToken ct = default)
        {
            if (!await _fetchGate.WaitAsync(0, ct)) return false;
            try
            {
                Console.WriteLine($"Fetching reports at {DateTime.UtcNow:HH:mm:ss} UTC...");
                await Task.WhenAll(metarService.fetchMetars(), tafService.FetchTafs());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during fetch: {ex.Message}");
                return false;
            }
            finally
            {
                _fetchGate.Release();
            }
        }

    }
}
