using DecoderTesting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;


namespace MetarTaf.Components.Pages
{
    public partial class OverviewPage
    {
        private DateTime currentTime;
        private Timer? timer;
        private List<Airport> airports = new List<Airport>();
        private bool isInitialized = false;
        private bool showNewTaf = true;
        private bool showNewMetar = true;
        private DateTime lastAcknowledgeTime = DateTime.MinValue;
        [Inject] private IJSRuntime JSRuntime { get; set; }


        private const string AirportsStorageKey = "airports";
        private NewAirportModel newAirportModel = new NewAirportModel();

        protected override void OnInitialized()
        {
            currentTime = DateTime.UtcNow;
            timer = new Timer(UpdateCurrentTime, null, 0, 1000); // Update every second
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !isInitialized)
            {
                await LoadAirportsFromLocalStorage();
                isInitialized = true;
                StateHasChanged();
            }
        }

        private void UpdateCurrentTime(object? state)
        {
            currentTime = DateTime.UtcNow;
            lastAcknowledgeTime = lastAcknowledgeTime.AddSeconds(1);
            InvokeAsync(StateHasChanged);
        }

        private async Task AddAirport()
        {
            if (!string.IsNullOrEmpty(newAirportModel.Icao))
            {
                string icaoToAdd = newAirportModel.Icao;
                newAirportModel.Icao = String.Empty;

                // Check if an airport with the same ICAO code already exists in the list
                var existingAirport = airports.FirstOrDefault(a => a.icaoId == icaoToAdd);
                if (existingAirport != null)
                {
                    Console.WriteLine($"Airport with ICAO {icaoToAdd} is already in the list.");
                    return; // Exit the method if the airport is already in the list
                }


                try
                {
                    var airport = AirportFactory.getAirport(icaoToAdd);
                    
                    // Check if the airport has valid data
                    if (airport != null)
                    {
                        await SaveAirportsToLocalStorage();
                        airports.Add(airport);
                        StateHasChanged();

                    }
                    else
                    {
                        Console.WriteLine($"Invalid airport data for ICAO: {newAirportModel.Icao}");
                        StateHasChanged();
                    }
                }
                catch (KeyNotFoundException ke)
                {
                    Console.WriteLine($"Error fetching data for ICAO {icaoToAdd}: {ke.Message}");
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error for ICAO {icaoToAdd}: {ex.Message}");          
                    StateHasChanged();
                }
            }
        }

        private async Task RemoveAirport(string icao)
        {
            airports.RemoveAll(a => a.icaoId == icao);
            await SaveAirportsToLocalStorage();
            StateHasChanged();
        }

        private async Task ClearAllAirports()
        {
            airports.Clear(); // Clear the in-memory dictionary
            await SaveAirportsToLocalStorage(); // Update the local storage
            StateHasChanged(); // Notify the UI to re-render
        }

        private async Task SaveAirportsToLocalStorage()
        {
            List<string> icaoList = new List<string>();

            foreach (Airport airport in airports)
            {
                icaoList.Add(airport.icaoId);
            }

            await JSRuntime.InvokeVoidAsync("localStorage.setItem", AirportsStorageKey, JsonSerializer.Serialize(icaoList));
        }

        private async Task LoadAirportsFromLocalStorage()
        {
            var icaoListJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", AirportsStorageKey);
            if (!string.IsNullOrEmpty(icaoListJson))
            {
                var icaoList = JsonSerializer.Deserialize<List<string>>(icaoListJson);
                if (icaoList != null)
                {
                    foreach (var icao in icaoList)
                    {
                        var airport = AirportFactory.getAirport(icao);
                        airports.Add(airport);
                    }
                }
            }
        }

        private void NavigateToAirportPage(string icao)
        {
            Navigation.NavigateTo($"/Airport/{icao}");
        }

        private class NewAirportModel
        {
            public string Icao { get; set; } = string.Empty;
        }

        public void ConfirmReports(Airport airport)
        {
            airport.markMetarAsOld();
            airport.markTafAsOld();
            StateHasChanged();
        }

        public void ConfirmAllReports()
        {
            foreach (var airport in airports)
            {
                airport.markMetarAsOld();
                airport.markTafAsOld();
            }
            lastAcknowledgeTime = DateTime.MinValue;
            StateHasChanged();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
