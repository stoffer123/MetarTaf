using MetarTaf_Backend.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MetarTaf_Backend.Services;

namespace MetarTaf_Backend.Models
{
    internal class AirportInfoStation : IInfoStation
    {
        private readonly List<IAirport> observers;
        private readonly MetarService metarService;
        private readonly TafService tafService;
        private readonly AirportController airportController;

        // Sikrer at kun en fetch kan kører ad gangen
        private readonly SemaphoreSlim _fetchGate = new(1, 1);

        public AirportInfoStation(AirportController airportController, NorthAviMetFetcher fetcher)
        {
            this.airportController = airportController;
            observers = new List<IAirport>();
            metarService = new MetarService(this, airportController, fetcher);
            tafService   = new TafService(this, airportController, fetcher);
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
                await Task.WhenAll(metarService.fetchMetars(), tafService.fetchTafs());
                airportController.ResetFetchTimerAfterFetch();
                return true;
            }
            finally
            {
                _fetchGate.Release();
            }
        }

    }
}
