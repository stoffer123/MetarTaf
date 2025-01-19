using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetarTaf_Backend
{
    public class AirportController
    {
        private Dictionary<string, IAirport> airports = new();
        private IInfoStation infoStation;
        private AirportFactory airportFactory;
        private readonly object lockObject;

        private Timer fetchTimer;
        private readonly object timerLock = new();
        private int timerDelayMinutes;

        public AirportController()
        {
            infoStation = new AirportInfoStation(this);
            airportFactory = new AirportFactory(infoStation);
            lockObject = new object();
            timerDelayMinutes = 1;

            InitializeFetchTimer();
        }

        private void InitializeFetchTimer()
        {
            // Create a timer that fetches reports every 5 minutes
            fetchTimer = new Timer(async _ => await FetchReportsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(timerDelayMinutes));
        }

        private async Task FetchReportsAsync()
        {
            try
            {
                Console.WriteLine($"Fetching reports at {DateTime.UtcNow:HH:mm:ss} UTC...");
                await infoStation.fetchNewReportsFromAPI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during fetch: {ex.Message}");
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
                // Stop the current timer
                StopFetchTimer();

                // Start the timer immediately to reset the countdown to zero
                StartFetchTimer();
                Console.WriteLine("Fetch timer reset after fetch.");
            }
        }


        public void StopFetchTimer()
        {
            lock (timerLock)
            {
                // Stop the timer
                fetchTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                Console.WriteLine("Fetch timer stopped.");
            }
        }

        public void StartFetchTimer()
        {
            lock (timerLock)
            {
                // Restart the timer with a fresh delay of 5 minutes
                fetchTimer?.Change(TimeSpan.FromMinutes(timerDelayMinutes), TimeSpan.FromMinutes(timerDelayMinutes));
                Console.WriteLine("Fetch timer started.");
            }
        }

        public IAirport getAirport(string icao)
        {
            lock (lockObject)
            {
                IAirport airport = null;
                if (airports.TryGetValue(icao, out airport))
                {
                    airport.incrementReferenceCount();
                }
                else
                {
                    airport = airportFactory.createAirport(icao);

                    if (airport != null)
                    {
                        airports.Add(airport.getAirportInfo().icaoId, airport);
                        infoStation.addObserver(airport);
                    }
                }
                return airport;
            }
        }

        public void releaseAirport(string icao)
        {
            lock (lockObject)
            {
                IAirport airport = null;

                if (airports.TryGetValue(icao, out airport))
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

        public List<string> getAirportIcaoList()
        {
            List<string> strings = airports.Keys.ToList();
            return strings;
        }
    }
}
