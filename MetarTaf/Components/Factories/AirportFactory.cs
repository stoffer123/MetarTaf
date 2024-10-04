using System;
using System.Collections.Generic;
using MetarTaf.Components.Models;
using MetarTaf.Components.Services;
using MetarTaf.Components.Services.Avwx;

namespace MetarTaf.Components.Factories
{
    public static class AirportFactory
    {
        private static readonly object lockObject = new object();
        public static readonly Dictionary<string, Airport> airports = new Dictionary<string, Airport>();
        private static IMetarService metarService;
        private static ITafService tafService;
        private static IAirportInfoService airportInfoService;

        public static void Initialize(IMetarService metarSvc, ITafService tafSvc, IAirportInfoService airportInfoSvc)
        {
            metarService = metarSvc;
            tafService = tafSvc;
            airportInfoService = airportInfoSvc;
        }

        public static Airport GetAirport(string icao)
        {
            lock (lockObject)
            {
                if (!airports.ContainsKey(icao))
                {
                    var airport = new Airport(icao, metarService, tafService, airportInfoService);
                    airport.IncrementReferenceCount();
                    airports[icao] = airport;
                    Console.WriteLine("[AirportFactory] Created new airport: " + icao);
                }
                else
                {
                    airports[icao].IncrementReferenceCount();
                    Console.WriteLine("[AirportFactory] Reused existing airport: " + icao);
                }

                

                return airports[icao];
            }
        }

        public static void ReleaseAirport(string icao)
        {
            lock (lockObject)
            {
                if (airports.ContainsKey(icao))
                {
                    airports[icao].DecrementReferenceCount();

                    if (!airports[icao].IsInUse())
                    {
                        airports[icao].Dispose();
                        airports.Remove(icao);
                        Console.WriteLine("[AirportFactory] Removed airport: " + icao);
                    }
                }

                
            }
        }
    }
}
