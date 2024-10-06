using MetarTaf.Components.Factories;
using MetarTaf.Components.Models.TafModels;
using MetarTaf.Components.Models;
using System.Text.Json;
using MetarTaf.Components.Models.MetarModels;
using System.Text;

namespace MetarTaf.Components.Services.Awc
{
    public class AwcTafService : ITafService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private List<TAF> tafList;
        private System.Timers.Timer timer;
        private bool isInitialized;

        public AwcTafService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            tafList = new List<TAF>();
            timer = new System.Timers.Timer(2 * 60 * 1000); //2 minutes in milliseconds
            timer.Elapsed += async (sender, e) => await OnTimedEvent();
            timer.AutoReset = true;
            timer.Enabled = true;
            isInitialized = false;
        }


        private async Task OnTimedEvent()
        {
            try
            {
                tafList = await getTafsFromAPI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching TAFSs in OnTimedEvent: {ex.Message}");
            }
        }




        public async Task<TAF?> GetTAFAsync(string icao)
        {
            if (!isInitialized)
            {
                await OnTimedEvent();
                isInitialized = true;
            }


            //see if it Airport contains metars
            Airport airport = AirportFactory.GetAirport(icao);
            if (airport.Tafs.Count == 0)
            {
                tafList = await getTafsFromAPI();
            }
            AirportFactory.ReleaseAirport(icao);

            // Filter the taf list to only include those that match the ICAO code
            List<TAF> matchingTafs = tafList.Where(m => m.station == icao).ToList();

            // Check if there are any METARs matching the ICAO code
            if (!matchingTafs.Any())
            {
                Console.WriteLine($"No TAFs found for ICAO: {icao}");
                return null;
            }

            // Find the latest TAF by the Time property
            TAF latestTaf = matchingTafs.OrderByDescending(m => m.time.dt).FirstOrDefault();

            // Return the latest TAF
            return latestTaf;
        }

        public async Task<List<TAF>> getTafsFromAPI()
        {
            StringBuilder icaoListBuilder = new StringBuilder();
            foreach (Airport airport in AirportFactory.airports.Values)
            {
                icaoListBuilder.Append(airport.Icao);

                if (AirportFactory.airports.Last().Value != airport)
                {
                    icaoListBuilder.Append("%2C");
                }
            }

            string icaoList = icaoListBuilder.ToString();
            string url = $"https://aviationweather.gov/api/data/taf?ids={icaoList}&format=json";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var TafAwcList = JsonSerializer.Deserialize<List<TAFAwc>>(responseBody);
                List<TAF> tafList = new List<TAF>();

                foreach (TAFAwc tafAwc in TafAwcList)
                {
                    TAF taf = tafAwc.createTAF();
                    tafList.Add(taf);

                }

                Console.WriteLine($"Successfully created TAFList from airportString: {icaoList}");

                return new List<TAF>(tafList);
            }

            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                Console.WriteLine(icaoList);
                return null;
            }
        }
    }
}
