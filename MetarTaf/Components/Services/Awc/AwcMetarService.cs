using Castle.Components.DictionaryAdapter;
using MetarTaf.Components.Factories;
using MetarTaf.Components.Models;
using MetarTaf.Components.Models.MetarModels;
using System.Globalization;
using System.Text;
using System.Text.Json;


namespace MetarTaf.Components.Services.Awc
{
    public class AwcMetarService : IMetarService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private List<Metar> metars;
        private System.Timers.Timer timer;
        private bool isInitialized;

        public AwcMetarService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            metars = new List<Metar>();
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
                metars = await getMetars();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching METARs in OnTimedEvent: {ex.Message}");
            }
        }

        public async Task<Metar?> GetMetarAsync(string icao)
        {
            if(!isInitialized)
            {
                await OnTimedEvent();
                isInitialized = true;
            }
            

            //see if it Airport contains metars
            Airport airport = AirportFactory.GetAirport(icao);
            if (airport.Metars.Count == 0)
            {
                metars = await getMetars();
            }

            // Filter the metars list to only include those that match the ICAO code
            List<Metar> matchingMetars = metars.Where(m => m.Station == icao).ToList();

            // Check if there are any METARs matching the ICAO code
            if (!matchingMetars.Any())
            {
                Console.WriteLine($"No METARs found for ICAO: {icao}");
                return null;
            }

            // Find the latest METAR by the Time property
            Metar latestMetar = matchingMetars.OrderByDescending(m => m.Time.Dt).FirstOrDefault();

            // Return the latest METAR
            return latestMetar;


        }

        private async Task<List<Metar>> getMetars()
        {
            StringBuilder icaoListBuilder = new StringBuilder();
            foreach(Airport airport in AirportFactory.airports.Values)
            {
                icaoListBuilder.Append(airport.Icao);

                if (AirportFactory.airports.Last().Value != airport)
                {
                    icaoListBuilder.Append("%2C");
                }
            }

            string icaoList = icaoListBuilder.ToString();
            string url = $"https://aviationweather.gov/api/data/metar?ids={icaoList}&format=json&hours=24";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var MetarAwcList = JsonSerializer.Deserialize<List<MetarAWC>>(responseBody);
                List<Metar> metarList = new List<Metar>();

                foreach(MetarAWC metarAwc in MetarAwcList)
                {
                    Metar metar = metarAwc.createMetar();
                    metarList.Add(metar);

                }

                Console.WriteLine($"Successfully created MetarList from airportString: {icaoList}");

                return new List<Metar>(metarList);
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
