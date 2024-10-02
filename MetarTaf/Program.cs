using MetarTaf.Components;
using MetarTaf.Components.Factories;
using MetarTaf.Components.Services.Avwx;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .Services
    .AddSingleton(new HttpClient())
    //Services - CHANGE HERE WHEN CHANGING API
    .AddSingleton(sp => new AvwxMetarService(sp.GetRequiredService<HttpClient>(), GetApiKey(builder.Configuration))) //MetarService, select which API service to use here
    .AddSingleton(sp => new AvwxTafService(sp.GetRequiredService<HttpClient>(), GetApiKey(builder.Configuration))) //TAFService, select which API service to use here
    .AddSingleton(sp => new AvwxAirportInfoService(sp.GetRequiredService<HttpClient>(), GetApiKey(builder.Configuration))); //AirportInfoService, select which API service to use here

var app = builder.Build();

// Initialize AirportFactory with the required services
var metarService = app.Services.GetRequiredService<AvwxMetarService>();
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
