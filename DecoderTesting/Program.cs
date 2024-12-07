using DecoderTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            // Initialize AirportFactory
            var airportFactory = new AirportFactory();

            // Create airports from JSON
            await airportFactory.createAirports();

            // Give some time for airports to initialize properly
            await Task.Delay(1000); // Small delay just in case for initialization

            // Try to fetch a specific airport by ICAO
            try
            {
                var airport = airportFactory.getAirport("EKEB");

                // Load METAR data
                Console.WriteLine("Loading METAR...");
                await airportFactory.infoStation.loadReports();

                // Ensure METAR data is loaded before accessing
                if (airport.metars.Count > 0)
                {
                    Console.WriteLine($"METAR Data Found for {airport.icaoId}");
                    foreach (var metarPair in airport.metars)
                    {
                        Console.WriteLine($"Time: {metarPair.Key}, METAR: {metarPair.Value.RawMetar}");
                    }
                }
                else
                {
                    Console.WriteLine($"No METAR Data found for {airport.icaoId}");
                }


                // Ensure TAF data is loaded before accessing
                if (airport.tafs.Count > 0)
                {
                    Console.WriteLine($"TAF Data Found for {airport.icaoId}");
                    foreach (var tafPair in airport.tafs)
                    {
                        Console.WriteLine($"Time: {tafPair.Key}, TAF: {tafPair.Value.RawTaf}");
                    }
                }
                else
                {
                    Console.WriteLine($"No TAF Data found for {airport.icaoId}");
                }

            }
            catch (KeyNotFoundException knf)
            {
                Console.WriteLine($"Airport not found: {knf.Message}");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception: {ex.Message}");
        }

        Console.ReadKey();
    }
}
