using Application;
using Application.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
using Application.DTO;    // AirportOverviewDto, MetarDTO, TafDto



namespace MetarTaf.Components.Pages
{
    public partial class OverviewPage
    {
        private DateTime currentTime;
        private Timer? timer;
        private List<AirportOverviewDto> airports = new();
        private bool isInitialized = false;
        private bool showNewTaf = true;
        private bool showNewMetar = true;
        private DateTime lastAcknowledgeTime = DateTime.MinValue;
        private readonly AckTracker ack = new();
        const string AckMetarKey = "ackMetarUtc";
        const string AckTafKey = "ackTafUtc";
        private readonly SemaphoreSlim _refreshGate = new(1, 1);
        private int _tick;
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private AirportController airportController { get; set; }

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

            // Hver 10. sekund: hent friske DTO’er
            var t = Interlocked.Increment(ref _tick);
            if (t % 10 == 0)
                _ = RefreshOverviewsAsync();  // fire-and-forget

            InvokeAsync(StateHasChanged);
        }


        private async Task AddAirport()
        {
            if (!string.IsNullOrEmpty(newAirportModel.Icao))
            {
                string icaoToAdd = newAirportModel.Icao?.Trim().ToUpperInvariant();
                newAirportModel.Icao = String.Empty;

                // Check if an airport with the same ICAO code already exists in the list
                var existingAirport = airports.FirstOrDefault(a => a.Icao == icaoToAdd);
                if (existingAirport != null)
                {
                    Console.WriteLine($"Airport with ICAO {icaoToAdd} is already in the list.");
                    return; // Exit the method if the airport is already in the list
                }


                try
                {
                    var airport = await airportController.GetOverviewAsync(icaoToAdd);

                    // Check if the airport has valid data
                    if (airport != null)
                    {
                        airports.Add(airport);
                        await SaveAirportsToLocalStorage();
                        await airportController.ForceFetchAsync();
                        await RefreshOverviewsAsync();
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
            airports.RemoveAll(a => a.Icao.Equals(icao, StringComparison.OrdinalIgnoreCase));
            await SaveAirportsToLocalStorage();
            StateHasChanged();
        }




        private async Task ClearAllAirports()
        {
            airports.Clear();
            await SaveAirportsToLocalStorage();
            StateHasChanged();
        }


        private async Task SaveAirportsToLocalStorage()
        {
            var icaos = airports.Select(a => a.Icao).ToList();
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", AirportsStorageKey, JsonSerializer.Serialize(icaos));
        }


        private async Task LoadAirportsFromLocalStorage()
        {
            var icaoListJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", AirportsStorageKey);
            if (string.IsNullOrEmpty(icaoListJson)) return;

            var icaos = JsonSerializer.Deserialize<List<string>>(icaoListJson) ?? new List<string>();
            foreach (var icao in icaos.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var dto = await airportController.GetOverviewAsync(icao); // <-- ny use case i Application
                if (dto != null) airports.Add(dto);
            }
            await airportController.ForceFetchAsync();
            await RefreshOverviewsAsync();
        }

        private void NavigateToAirportPage(string icao)
        {
            Navigation.NavigateTo($"/Airport/{icao}");
        }

        private class NewAirportModel
        {
            public string Icao { get; set; } = string.Empty;
        }

        public async Task ConfirmReports(AirportOverviewDto ap)
        {
            var nowUtc = DateTime.UtcNow;

            if (ap.LatestMetar != null)
                ack.AckMetar(ap.Icao, ap.LatestMetar.ReportTimeUtc);

            if (ap.LatestTaf != null)
                ack.AckTaf(ap.Icao, ap.LatestTaf.ReportTimeUtc);

            lastAcknowledgeTime = nowUtc;   // <-- ikke MinValue
            await SaveAcksAsync();
            StateHasChanged();
        }


        public async Task ConfirmAllReports()
        {
            var nowUtc = DateTime.UtcNow;

            foreach (var ap in airports)
            {
                if (ap.LatestMetar != null)
                    ack.AckMetar(ap.Icao, ap.LatestMetar.ReportTimeUtc);
                if (ap.LatestTaf != null)
                    ack.AckTaf(ap.Icao, ap.LatestTaf.ReportTimeUtc);
            }

            lastAcknowledgeTime = nowUtc;   // <-- ikke MinValue
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

        private static string FormatTimeDiff(TimeSpan ts)
        {
            if (ts < TimeSpan.Zero)
                return "-" + ts.Duration().ToString(@"hh\:mm");
            else
                return ts.ToString(@"hh\:mm");
        }

        private async Task RefreshOverviewsAsync()
        {
            if (!await _refreshGate.WaitAsync(0)) return;
            try
            {
                if (airports.Count == 0) return;

                var icaos = airports.Select(a => a.Icao).ToArray();
                var tasks = icaos.Select(icao => airportController.GetOverviewAsync(icao));
                var dtos = await Task.WhenAll(tasks);

                airports.Clear();
                airports.AddRange(dtos.Where(d => d != null)!);

                await InvokeAsync(StateHasChanged);
            }
            finally
            {
                _refreshGate.Release();
            }
        }



        public void Dispose()
        {
            timer?.Dispose();
        }

    }
}