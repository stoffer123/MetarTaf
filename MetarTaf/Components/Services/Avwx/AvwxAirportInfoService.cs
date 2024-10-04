using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MetarTaf.Components.Factories;
using MetarTaf.Components.Models;
using MetarTaf.Components.Models.AirportInfoModels;

namespace MetarTaf.Components.Services.Avwx
{
    public class AvwxAirportInfoService : IAirportInfoService
    {
        private readonly HttpClient httpClient;

        public AvwxAirportInfoService(HttpClient httpClient, string token)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://avwx.rest/api/");
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Token {token}");
        }

        public async Task<AirportInfo?> GetAirportInfoAsync(string ident)
        {
            var response = await httpClient.GetAsync($"station/{ident}?format=json");
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();

            AirportInfoAvwx airportInfoAvwx = JsonSerializer.Deserialize<AirportInfoAvwx>(responseData);

            AirportInfo airportInfo = airportInfoAvwx.createAirportInfo();

            return airportInfo;
        }
    }
}
