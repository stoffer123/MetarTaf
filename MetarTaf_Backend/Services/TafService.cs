using Metar.Decoder.Entity;
using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Services
{
    internal class TafService
    {
        private Dictionary<string, Dictionary<DateTime, TafReport>> tafs = new();
        private string apiUrl = "https://api.met.no/weatherapi/tafmetar/1.0/taf?extended=true&icao=";
        private TafFactory tafFactory = new();
        private IInfoStation infoStation;
        private AirportController airportController;

        public TafService(IInfoStation infostation, AirportController airportController)
        {
            this.infoStation = infostation;
            this.airportController = airportController;
        }


        public async Task fetchTafs()
        {
            string[] icaoList = airportController.getAirportIcaoList().ToArray();
            string icaoString = string.Empty;

            //Build icaoString for the request URL
            for (int i = 0; i < icaoList.Length; i++)
            {
                icaoString += icaoList[i];
                if (i != icaoList.Length - 1)
                {
                    icaoString += ",";
                }
            }


            // Combine the API URL with the ICAO string
            string requestUrl = apiUrl + icaoString;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Optional: Add headers if required by the API
                    client.DefaultRequestHeaders.Add("User-Agent", "MetarTaf/TEST (contact: christopherMikkelsen@live.dk)");


                    // Send GET request to the API
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response as a string
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Split the response into lines
                        string[] lines = responseData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);



                        // Process each line
                        foreach (string line in lines)
                        {
                            try
                            {
                                string processedLine = line.Replace("AUTO ", "");

                                // Create a MetarReport object
                                string tafLine = "TAF " + processedLine;

                                TafReport taf = tafFactory.createTaf(tafLine); //"METAR" prefix is needed
                                string icao = taf.decodedTaf.Icao;
                                DateTime reportTime = taf.reportTime;

                                // Check if the ICAO key exists in the dictionary
                                if (!tafs.ContainsKey(icao))
                                {
                                    // Add a new entry for the airport
                                    tafs[icao] = new Dictionary<DateTime, TafReport>();
                                }

                                // Check if the reportTime key exists for the airport
                                if (!tafs[icao].ContainsKey(reportTime))
                                {
                                    // Add the MetarReport for the specific report time
                                    tafs[icao][reportTime] = taf;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing line: {line}. Exception: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            infoStation.notifyTafChange();
        }

        public Dictionary<DateTime, TafReport> getTafs(string icao)
        {
            Dictionary<DateTime, TafReport> tafList;
            if (tafs.TryGetValue(icao, out tafList))
            {
                Console.WriteLine($"Successfully found {icao} in dict of metars");
            }
            else
            {
                Console.WriteLine($"Failed to find {icao} in dict of metars, returning empty dict");
                tafList = new Dictionary<DateTime, TafReport>();
            }

            return tafList;
        }

        //For testing
        public void printIcaoList()
        {
            foreach (KeyValuePair<string, Dictionary<DateTime, TafReport>> kvp in tafs)
            {
                Console.WriteLine(kvp.Key);
            }
        }

    }
}
