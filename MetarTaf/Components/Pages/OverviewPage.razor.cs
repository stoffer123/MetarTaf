using Domain.Ports;
using MetarTaf_Backend;
using MetarTaf_Backend.Models;
using MetarTaf_Backend.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;



namespace MetarTaf.Components.Pages
{
    public partial class OverviewPage
    {
        private DateTime currentTime;
        private Timer? timer;
        private List<IAirport> airports = new List<IAirport>();
        private bool isInitialized = false;
        private bool showNewTaf = true;
        private bool showNewMetar = true;
        private DateTime lastAcknowledgeTime = DateTime.MinValue;
        private readonly AckTracker ack = new();
        const string AckMetarKey = "ackMetarUtc";
        const string AckTafKey = "ackTafUtc";
        // undgå dobbelt-subscribe (valgfrit men rart)
        private readonly HashSet<string> _subscribed = new(StringComparer.OrdinalIgnoreCase);
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private IAirportController airportController { get; set; }

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
                await LoadAcksAsync();
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
                string icaoToAdd = newAirportModel.Icao?.Trim().ToUpperInvariant();
                newAirportModel.Icao = String.Empty;

                // Check if an airport with the same ICAO code already exists in the list
                var existingAirport = airports.FirstOrDefault(a => a.getAirportInfo().icaoId == icaoToAdd);
                if (existingAirport != null)
                {
                    Console.WriteLine($"Airport with ICAO {icaoToAdd} is already in the list.");
                    return; // Exit the method if the airport is already in the list
                }


                try
                {
                    var airport = await airportController.GetAirportAsync(icaoToAdd);

                    // Check if the airport has valid data
                    if (airport != null)
                    {
                        Attach(airport); // Attach the airport to the controller
                        airports.Add(airport);
                        await SaveAirportsToLocalStorage();
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
            var ap = airports.FirstOrDefault(a => a.getAirportInfo().icaoId == icao);
            if (ap != null)
            {
                Detach(ap);   // <-- NYT
            }

            airportController.releaseAirport(icao);
            airports.RemoveAll(a => a.getAirportInfo().icaoId == icao);
            await SaveAirportsToLocalStorage();
            StateHasChanged();
        }



        private async Task ClearAllAirports()
        {
            foreach (IAirport airport in airports)
            {
                Detach(airport); // Detach the airport from the controller
                airportController.releaseAirport(airport.getAirportInfo().icaoId);
            }

            airports.Clear(); // Clear the in-memory dictionary
            await SaveAirportsToLocalStorage(); // Update the local storage
            StateHasChanged(); // Notify the UI to re-render
        }

        private async Task SaveAirportsToLocalStorage()
        {
            List<string> icaoList = new List<string>();

            foreach (IAirport airport in airports)
            {
                icaoList.Add(airport.getAirportInfo().icaoId);
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
                    foreach (var icao in icaoList.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        var airport = await airportController.GetAirportAsync(icao);
                        Attach(airport);           // <-- NYT
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

        public async Task ConfirmReports(IAirport airport)
        {
            var icao = airport.getAirportInfo().icaoId;

            var latestMetar = airport.getMetars().OrderByDescending(m => m.Key).FirstOrDefault().Value;
            var latestTaf = airport.getTafs().OrderByDescending(t => t.Key).FirstOrDefault().Value;

            var nowUtc = DateTime.UtcNow;

            if (latestMetar != null)
            {
                var norm = TimeNormalizer.NormalizeIssueTimeUtc(
                    DateTime.SpecifyKind(latestMetar.reportTime, DateTimeKind.Utc),
                    DateTime.SpecifyKind(latestMetar.fetchTime, DateTimeKind.Utc),
                    nowUtc,
                    tolerance: TimeSpan.FromMinutes(10));
                ack.AckMetar(icao, norm);
            }

            if (latestTaf != null)
            {
                var norm = TimeNormalizer.NormalizeIssueTimeUtc(
                    DateTime.SpecifyKind(latestTaf.reportTime, DateTimeKind.Utc),
                    DateTime.SpecifyKind(latestTaf.fetchTime, DateTimeKind.Utc),
                    nowUtc,
                    tolerance: TimeSpan.FromMinutes(10));
                ack.AckTaf(icao, norm);
            }

            lastAcknowledgeTime = DateTime.MinValue;
            await SaveAcksAsync();
            StateHasChanged();
        }

        public async Task ConfirmAllReports()
        {
            var nowUtc = DateTime.UtcNow;

            foreach (var airport in airports)
            {
                var icao = airport.getAirportInfo().icaoId;

                var latestMetar = airport.getMetars().OrderByDescending(m => m.Key).FirstOrDefault().Value;
                if (latestMetar != null)
                {
                    var norm = TimeNormalizer.NormalizeIssueTimeUtc(
                        DateTime.SpecifyKind(latestMetar.reportTime, DateTimeKind.Utc),
                        DateTime.SpecifyKind(latestMetar.fetchTime, DateTimeKind.Utc),
                        nowUtc,
                        tolerance: TimeSpan.FromMinutes(10));
                    ack.AckMetar(icao, norm);
                }

                var latestTaf = airport.getTafs().OrderByDescending(t => t.Key).FirstOrDefault().Value;
                if (latestTaf != null)
                {
                    var norm = TimeNormalizer.NormalizeIssueTimeUtc(
                        DateTime.SpecifyKind(latestTaf.reportTime, DateTimeKind.Utc),
                        DateTime.SpecifyKind(latestTaf.fetchTime, DateTimeKind.Utc),
                        nowUtc,
                        tolerance: TimeSpan.FromMinutes(10));
                    ack.AckTaf(icao, norm);
                }
            }

            lastAcknowledgeTime = DateTime.MinValue;
            await SaveAcksAsync();
            StateHasChanged();
        }

        private async Task LoadAcksAsync()
        {
            var mJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", AckMetarKey);
            var tJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", AckTafKey);

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var m = string.IsNullOrEmpty(mJson) ? null : JsonSerializer.Deserialize<Dictionary<string, DateTime>>(mJson, opts);
            var t = string.IsNullOrEmpty(tJson) ? null : JsonSerializer.Deserialize<Dictionary<string, DateTime>>(tJson, opts);

            ack.Load(m, t);
        }

        private Task SaveAcksAsync()
        {
            var m = JsonSerializer.Serialize(ack.SnapshotMetar());
            var t = JsonSerializer.Serialize(ack.SnapshotTaf());
            return Task.WhenAll(
                JSRuntime.InvokeVoidAsync("localStorage.setItem", AckMetarKey, m).AsTask(),
                JSRuntime.InvokeVoidAsync("localStorage.setItem", AckTafKey, t).AsTask()
            );
        }

        // kald denne når en airport melder nyt
        private void OnAirportUpdated()
        {
            InvokeAsync(StateHasChanged);
        }

        private void Attach(IAirport ap)
        {
            var icao = ap.getAirportInfo().icaoId;
            if (_subscribed.Add(icao))
                ap.Updated += OnAirportUpdated;
        }

        private void Detach(IAirport ap)
        {
            var icao = ap.getAirportInfo().icaoId;
            if (_subscribed.Remove(icao))
                ap.Updated -= OnAirportUpdated;
        }

        private static string FormatTimeDiff(TimeSpan ts)
        {
            if (ts < TimeSpan.Zero)
                return "-" + ts.Duration().ToString(@"hh\:mm");
            else
                return ts.ToString(@"hh\:mm");
        }


        public void Dispose()
        {
            foreach (var ap in airports)
                Detach(ap);        // <-- NYT
            timer?.Dispose();
        }

    }
}