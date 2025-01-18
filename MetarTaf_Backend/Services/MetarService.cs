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
    internal class MetarService
    {
        private Dictionary<string, Dictionary<DateTime, MetarReport>> metars = new();
        private string apiUrl = "https://api.met.no/weatherapi/tafmetar/1.0/metar?extended=true&icao=";
        private MetarFactory metarFactory = new();


        public async Task fetchMetars()
        {
            string[] icaoList = AirportController.getAirportIcaoList().ToArray();
            string icaoString = string.Empty;

            //Build icaoString for the request URL
            for (int i = 0; i < icaoList.Length; i++)
            {
                icaoString += icaoList[i];
                if(i != icaoList.Length - 1)
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
                                string metarLine = "METAR " + processedLine;

                                MetarReport metar = metarFactory.createMetar(metarLine); //"METAR" prefix is needed
                                Console.WriteLine(metarLine);
                                string icao = metar.decodedMetar.ICAO;
                                DateTime reportTime = metar.reportTime;

                                // Check if the ICAO key exists in the dictionary
                                if (!metars.ContainsKey(icao))
                                {
                                    // Add a new entry for the airport
                                    metars[icao] = new Dictionary<DateTime, MetarReport>();
                                }

                                // Check if the reportTime key exists for the airport
                                if (!metars[icao].ContainsKey(reportTime))
                                {
                                    // Add the MetarReport for the specific report time
                                    metars[icao][reportTime] = metar;
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


        }

        public Dictionary<DateTime, MetarReport> getMetars(string icao)
        {
            Dictionary<DateTime, MetarReport> metarList;
            if(metars.TryGetValue(icao, out metarList))
            {
                Console.WriteLine($"Successfully found {icao} in dict of metars");
            }
            else
            {
                Console.WriteLine($"Failed to find {icao} in dict of metars, returning empty dict");
                metarList = new Dictionary<DateTime, MetarReport>();
            }

            return metarList;
        }

        //For testing
        public void printIcaoList()
        {
            foreach(KeyValuePair<string, Dictionary<DateTime, MetarReport>> kvp in metars)
            {
                Console.WriteLine(kvp.Key);
            }
        }

    }
}
