using MetarTaf.Components;
using MetarTaf.Components.Factories;
using MetarTaf.Components.Services.Avwx;
using MetarTaf.Components.Services.Awc;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .Services
    .AddSingleton(new HttpClient()) // Register HttpClient as a Singleton
                                    // Services - CHANGE HERE WHEN CHANGING API
    .AddSingleton(sp => new AwcMetarService(sp.GetRequiredService<HttpClient>())) // MetarService
    .AddSingleton(sp => new AvwxTafService(sp.GetRequiredService<HttpClient>(), GetApiKey(builder.Configuration))) // TAFService
    .AddSingleton(sp => new AvwxAirportInfoService(sp.GetRequiredService<HttpClient>(), GetApiKey(builder.Configuration))); // AirportInfoService

var app = builder.Build();

// Initialize AirportFactory with the required services
var metarService = app.Services.GetRequiredService<AwcMetarService>();
var tafService = app.Services.GetRequiredService<AvwxTafService>();
var airportInfoService = app.Services.GetRequiredService<AvwxAirportInfoService>();
AirportFactory.Initialize(metarService, tafService, airportInfoService);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static string GetApiKey(IConfiguration configuration)
{
    // Try to get the API key from environment variables first
    var apiKey = Environment.GetEnvironmentVariable("API_KEY");

    // If not found, fall back to the configuration file
    if (string.IsNullOrEmpty(apiKey))
    {
        apiKey = configuration["ApiSettings:ApiKey"];
    }

    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("API key is not set.");
    }

    return apiKey;
}
