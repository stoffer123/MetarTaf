using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Services
{
    internal static class AirportInfoService
    {
        private static readonly string infoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Info"); //Set info directiory
        private static Dictionary<string, AirportInfo> airportInfos = new();

        

        public static AirportInfo getAirportInfo(string icao)
        {
            AirportInfo airportInfo = null;

            if (airportInfos.TryGetValue(icao, out airportInfo))
            {
                Console.WriteLine("Successfully found " + icao);
            }
            else
            {
                Console.WriteLine("Couldnt find: " + icao);
            }
            return airportInfo;
        }

        public static async Task createAirportInfo()
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

            List<AirportInfo> tempList = DeserializeAirports(fileContent);

            foreach (AirportInfo temp in tempList)
            {
                if(temp.icaoId != null)
                {
                    
                    if(airportInfos.TryAdd(temp.icaoId, temp))
                    {
                        Console.WriteLine($"Successfully added {temp.icaoId} to airportInfos");

                    }
                }
            }


        }

        private static List<AirportInfo> DeserializeAirports(string jsonContent)
        {
            try
            {
                var airportInfoList = JsonSerializer.Deserialize<List<AirportInfo>>(jsonContent);
                if (airportInfoList == null)
                {
                    throw new Exception("Failed to deserialize airport info");
                }
                return airportInfoList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return new List<AirportInfo>();
            }
        }
    }
}
