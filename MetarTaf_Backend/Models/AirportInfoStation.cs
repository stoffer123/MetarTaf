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

        // Til at undgå overlappende fetch-kørsler
        private readonly SemaphoreSlim _fetchGate = new(1, 1);

        // NY: injicer NorthAviMetFetcher og giv den videre til services
        public AirportInfoStation(AirportController airportController, NorthAviMetFetcher fetcher)
        {
            this.airportController = airportController;
            observers = new List<IAirport>();

            // MetarService/TafService er dine eksisterende klasser – vi bruger de opdaterede ctor’er,
            // der tager (IInfoStation, AirportController, NorthAviMetFetcher)
            metarService = new MetarService(this, airportController, fetcher);
            tafService   = new TafService(this, airportController, fetcher);
        }

        public void addObserver(IAirport observer)
        {
            observers.Add(observer);
            // fire-and-forget men med gate, så vi ikke kører flere fetches parallelt
            _ = FetchIfIdle();
        }

        public void removeObserver(IAirport observer)
        {
            observers.Remove(observer);
        }

        public void notifyAirportInfoChange()
        {
            foreach (IAirport observer in observers)
                observer.updateAirportInfo();
        }

        public void notifyMetarChange()
        {
            foreach (IAirport observer in observers)
                observer.updateMetars();
        }

        public void notifyTafChange()
        {
            foreach (IAirport observer in observers)
                observer.updateTafs();
        }

        public Dictionary<DateTime, MetarReport> getMetars(string icao)
            => metarService.getMetars(icao);

        public Dictionary<DateTime, TafReport> getTafs(string icao)
            => tafService.getTafs(icao);

        public async Task fetchNewReportsFromAPI()
        {
            // Kør begge fetch i parallel, men kontrolleret af gate
            await Task.WhenAll(
                metarService.fetchMetars(),
                tafService.fetchTafs()
            );

            airportController.ResetFetchTimerAfterFetch();
        }

        // Helper der sikrer single-flight
        private async Task FetchIfIdle()
        {
            if (!await _fetchGate.WaitAsync(0)) return; // en fetch kører allerede
            try
            {
                await fetchNewReportsFromAPI();
            }
            finally
            {
                _fetchGate.Release();
            }
        }
    }
}
