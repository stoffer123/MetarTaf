using System.Collections.Concurrent;
using System.IO.Compression;
using Metar.Decoder;
using Metar.Decoder.Entity;
using Taf.Decoder;
using Taf.Decoder.entity;
using static Metar.Decoder.Entity.DecodedMetar;
using static Taf.Decoder.entity.DecodedTaf;

namespace DecoderTesting
{
    public class InfoStation : IInfoStation
    {
        private List<Airport> airports = new List<Airport>();
        private ConcurrentDictionary<string, DecodedMetar> metars = new ConcurrentDictionary<string, DecodedMetar>();
        private ConcurrentDictionary<string, DecodedTaf> tafs = new ConcurrentDictionary<string, DecodedTaf>();

        private readonly string infoDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Info"); //Set info directiory


        public void addAirport(Airport airport)
        {
        airports.Add(airport);
        }

        public void removeAirport(Airport airport)
        {
            airports.Remove(airport);
        }

        public async Task notifyAsync()
        {
            var updateTasks = new List<Task>();

            foreach (Airport airport in airports)
            {
                try
                {
                    if (metars.TryGetValue(airport.icaoId, out var metar))
                    {
                        DateTime metarTime = airport.parseDateTime(metar.Day.Value, metar.Time);

                        if (!airport.metars.ContainsKey(metarTime))
                        {
                            updateTasks.Add(Task.Run(() => airport.update()));
                        }
                    }else if(tafs.TryGetValue(airport.icaoId, out var taf))
                    {
                        DateTime tafTime = airport.parseDateTime(taf.Day.Value, taf.Time);
                        if (!airport.tafs.ContainsKey(tafTime))
                        {
                            updateTasks.Add(Task.Run(() => airport.update()));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No METAR or TAF data found for airport {airport.icaoId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing airport {airport.icaoId}: {ex.Message}");
                }
            }

            await Task.WhenAll(updateTasks);
        }

        public DecodedMetar getMetar(Airport airport)
        {
            if (metars.ContainsKey(airport.icaoId))
            {
                return metars[airport.icaoId];
            }

            Console.WriteLine("METAR entry missing, preventing redundant fetch.");
            return null;
        }

        public async Task loadReports()
        {
            Task metarTask = loadMetar();
            Task tafTask = loadTaf();

            await Task.WhenAll(metarTask, tafTask);
            Console.WriteLine("Both METAR and TAF reports have been loaded.");

            await notifyAsync();
        }

        public DecodedTaf getTaf(Airport airport) 
        {
            if (tafs.ContainsKey(airport.icaoId))
            {
                return tafs[airport.icaoId];
            }

            Console.WriteLine("TAF entry missing, preventing redundant fetch.");
            return null;
        }

        public async Task loadMetar()
        {
            string url = "https://aviationweather.gov/data/cache/metars.cache.csv.gz";
            string gzipFilePath = Path.Combine(infoDirectory, "metars.cache.csv.gz");
            string extractedFilePath = Path.Combine(infoDirectory, "metars.cache.csv");

            try
            {
                // Ensure the /Info directory exists
                Directory.CreateDirectory(infoDirectory);

                // Step 1: Download the .gz file
                Console.WriteLine("Downloading the METAR GZIP file...");
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    await using (var fileStream = new FileStream(gzipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
                Console.WriteLine($"File downloaded to: {gzipFilePath}");

                // Step 2: Unzip the .gz file
                Console.WriteLine("Extracting the GZIP file...");
                using (var gzipStream = new GZipStream(new FileStream(gzipFilePath, FileMode.Open), CompressionMode.Decompress))
                using (var outputFileStream = new FileStream(extractedFilePath, FileMode.Create))
                {
                    gzipStream.CopyTo(outputFileStream);
                }
                Console.WriteLine($"File extracted to: {extractedFilePath}");

                // Step 3: Read through all the lines of the unpacked CSV file
                Console.WriteLine("Reading and processing the CSV file...");
                using (var reader = new StreamReader(extractedFilePath))
                {
                    // Clear the metars dictionary
                    metars.Clear();
                    int currentLine = 0;

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        // Process each line
                        string[] splitArr = line.Split(',');

                        if (splitArr.Length <= 42) // Ensure there are enough fields
                        {
                            Console.WriteLine($"Line {currentLine} skipped due to insufficient data.");
                            continue;
                        }
                        if (splitArr[0] == "raw_text")
                        {
                            continue;
                        }

                        string rawMetar = splitArr[42] + " " + splitArr[0];

                        // Decode the METAR using the MetarDecoder
                        try
                        {
                            Console.WriteLine(rawMetar);
                            DecodedMetar metar = MetarDecoder.ParseWithMode(rawMetar);

                            // Log the decoded METAR's ICAO code for debugging purposes
                            Console.WriteLine($"Decoded METAR ICAO: '{metar.ICAO}'");



                            // Attempt to match METAR to the corresponding airport
                            Airport matchingAirport = airports.FirstOrDefault(a =>
                                a.icaoId?.Trim().ToUpper() == metar.ICAO?.Trim().ToUpper()
                            );

                            if (matchingAirport != null)
                            {
                                // Log the match for debugging
                                Console.WriteLine($"Matched airport: '{matchingAirport.icaoId}'");

                                // Store the decoded METAR in the dictionary
                                metars[matchingAirport.icaoId] = metar;
                                Console.WriteLine($"Stored METAR for {matchingAirport.icaoId}: {rawMetar}");
                            }
                            else
                            {
                                Console.WriteLine($"No matching airport found for ICAO: '{metar.ICAO}'");
                            }
                            currentLine++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error decoding METAR on line {currentLine}: {ex.Message}");
                        }
                    }
                }
                Console.WriteLine("Finished reading the CSV file.");
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task loadTaf() 
        {
            string url = "https://aviationweather.gov/data/cache/tafs.cache.csv.gz";
            string gzipFilePath = Path.Combine(infoDirectory, "tafs.cache.csv.gz");
            string extractedFilePath = Path.Combine(infoDirectory, "tafs.cache.csv");

            try
            {
                // Ensure the /Info directory exists
                Directory.CreateDirectory(infoDirectory);

                // Step 1: Download the .gz file
                Console.WriteLine("Downloading the TAF GZIP file...");
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    await using (var fileStream = new FileStream(gzipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
                Console.WriteLine($"File downloaded to: {gzipFilePath}");

                // Step 2: Unzip the .gz file
                Console.WriteLine("Extracting the TAF GZIP file...");
                using (var gzipStream = new GZipStream(new FileStream(gzipFilePath, FileMode.Open), CompressionMode.Decompress))
                using (var outputFileStream = new FileStream(extractedFilePath, FileMode.Create))
                {
                    gzipStream.CopyTo(outputFileStream);
                }
                Console.WriteLine($"File extracted to: {extractedFilePath}");

                // Step 3: Read through all the lines of the unpacked CSV file
                Console.WriteLine("Reading and processing the TAF CSV file...");
                using (var reader = new StreamReader(extractedFilePath))
                {
                    // Clear the tafs dictionary
                    tafs.Clear();
                    int currentLine = 0;

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        // Process each line
                        string[] splitArr = line.Split(',');

                        if (splitArr.Length <= 20) // Ensure there are enough fields
                        {
                            Console.WriteLine($"Line {currentLine} skipped due to insufficient data.");
                            continue;
                        }
                        if (splitArr[0] == "raw_text")
                        {
                            continue;
                        }

                        string rawTaf = splitArr[0];
                        

                        // Decode the METAR using the MetarDecoder
                        try
                        {
                            Console.WriteLine(rawTaf);
                            DecodedTaf taf = TafDecoder.ParseWithMode(rawTaf);

                            // Log the decoded METAR's ICAO code for debugging purposes
                            Console.WriteLine($"Decoded TAF ICAO: '{taf.Icao}'");



                            // Attempt to match METAR to the corresponding airport
                            Airport matchingAirport = airports.FirstOrDefault(a =>
                                a.icaoId?.Trim().ToUpper() == taf.Icao?.Trim().ToUpper()
                            );

                            if (matchingAirport != null)
                            {
                                // Log the match for debugging
                                Console.WriteLine($"Matched airport: '{matchingAirport.icaoId}'");

                                // Store the decoded METAR in the dictionary
                                tafs[matchingAirport.icaoId] = taf;
                                Console.WriteLine($"Stored TAF for {matchingAirport.icaoId}: {rawTaf}");
                            }
                            else
                            {
                                Console.WriteLine($"No matching airport found for ICAO: '{taf.Icao}'");
                            }
                            currentLine++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error decoding TAF on line {currentLine}: {ex.Message}");
                        }
                    }
                }
                Console.WriteLine("Finished reading the CSV file.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


    }
}
