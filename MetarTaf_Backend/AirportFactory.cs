
using MetarTaf_Backend.Models;
using System.Text;

namespace MetarTaf_Backend
{
    public static class AirportFactory
    {
        private static readonly object lockObject = new object();
        static readonly Dictionary<string, Airport> airports = new Dictionary<string, Airport>();
        private static IInfoStation infoStation = new InfoStation();

        static Airport GetAirport(string icao)
        {
            lock (lockObject)
            {
                if (!airports.ContainsKey(icao))
                {
                    var airport = new Airport(infoStation, icao);
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
                        airports.Remove(icao);
                        Console.WriteLine("[AirportFactory] Removed airport: " + icao);
                    }
                }

                
            }
        }

        public static string getIcaoString()
        {
            // return a comma seperated string of all airportICAOS in the airports list.
            StringBuilder sb = new StringBuilder();

            int counter = 0;
            foreach (KeyValuePair<string,Airport> kvp in airports)
            {
                Airport airport = kvp.Value;
                sb.Append(airport.icao);

                if(counter == 0 || counter == airports.Count() - 1)
                {
                    sb.Append(",");
                }
                counter++;
            }
            return sb.ToString();
        }
    }
}
