
using MetarTaf_Backend.Models;
using System.Runtime.CompilerServices;

namespace MetarTaf_Backend
{
    public static class AirportFactory
    {
        private static readonly object lockObject = new object();
        public static readonly Dictionary<string, Airport> airports = new Dictionary<string, Airport>();
        private IInfoStation infoStation;

        public static Airport GetAirport(string icao)
        {
            lock (lockObject)
            {
                if (!airports.ContainsKey(icao))
                {
                    var airport = new Airport(infoStation);
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
