using System.IO.Compression;
using System.Text.Json;

namespace DecoderTesting
{
    public class AirportFactory
    {
        // icao, airport
        private Dictionary<string, Airport> airports = new Dictionary<string, Airport>();
        private string jsonPath = ".stations.json";  //Path to the JSON file
        public IInfoStation infoStation;
        private readonly string infoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Info"); //Set info directiory

        public AirportFactory()
        {
            this.infoStation = new InfoStation();
        }

        //Return an airport from the dictionary
        public Airport getAirport(string icao) 
        {
            Airport result = null;
            airports.TryGetValue(icao, out result);

            if (result != null) return result;

            throw new KeyNotFoundException($"The airport with ICAO code '{icao}' was not found.");
        }

        public async Task createAirports()
        {
            // Define the URL and file paths
            string jsonUrl = "https://aviationweather.gov/data/cache/stations.cache.json.gz"; // Replace with the actual URL
            string gzipFilePath = Path.Combine(infoDirectory, "airports.json.gz");
            string jsonFilePath = Path.Combine(infoDirectory, "airports.json");

            // Ensure the /Info directory exists
            Directory.CreateDirectory(infoDirectory);

            // Download the GZIP file if it doesn't already exist locally
            if (!File.Exists(jsonFilePath))
            {
                try
                {
                    Console.WriteLine("JSON file not found. Attempting to download...");

                    using (HttpClient client = new HttpClient())
                    {
                        // Download the GZIP file
                        var response = await client.GetAsync(jsonUrl);
                        response.EnsureSuccessStatusCode();
                        await using (var fileStream = new FileStream(gzipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await response.Content.CopyToAsync(fileStream);
                        }
                        Console.WriteLine($"GZIP file downloaded to: {gzipFilePath}");
                    }

                    // Extract the GZIP file to JSON
                    Console.WriteLine("Extracting the GZIP file...");
                    using (var gzipStream = new GZipStream(new FileStream(gzipFilePath, FileMode.Open), CompressionMode.Decompress))
                    using (var outputFileStream = new FileStream(jsonFilePath, FileMode.Create))
                    {
                        gzipStream.CopyTo(outputFileStream);
                    }
                    Console.WriteLine($"JSON file extracted to: {jsonFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download or extract JSON file: {ex.Message}");
                    return;
                }
            }

            // Read the JSON content from the extracted file
            string fileContent = File.ReadAllText(jsonFilePath);

            // Deserialize JSON into a list of Airport objects
            List<Airport> airportsList = DeserializeAirports(fileContent);

            // Initialize the airports and associate them with the infoStation
            foreach (var airport in airportsList)
            {
                airport.infoStation = infoStation;

                // Check if the airport already exists in the dictionary
                if (airports.ContainsKey(airport.icaoId))
                {
                    Console.WriteLine($"Updating existing airport: {airport.icaoId}");
                    airports[airport.icaoId] = airport; // Overwrite the existing airport data
                }
                else
                {
                    Console.WriteLine($"Adding new airport: {airport.icaoId}");
                    airports.Add(airport.icaoId, airport);
                }

                // Add the airport to the infoStation
                infoStation.addAirport(airport);

                Console.WriteLine($"ICAO: {airport.icaoId}, Site: {airport.site}, State: {airport.state}, Country: {airport.country}");
            }

            Console.WriteLine("Total number of airports: " + airports.Count());
        }



        private static List<Airport> DeserializeAirports(string jsonContent)
        {
            try
            {
                var airportList = JsonSerializer.Deserialize<List<Airport>>(jsonContent);
                if (airportList == null)
                {
                    throw new Exception("Failed to deserialize airports.");
                }
                return airportList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return new List<Airport>();
            }
        }



    }
}
