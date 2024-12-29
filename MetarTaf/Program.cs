using DecoderTesting;
using MetarTaf.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register InfoStation and AirportFactory as singletons
builder.Services.AddSingleton<IInfoStation, InfoStation>();
builder.Services.AddSingleton<AirportFactory>();
builder.Services.AddHostedService<InfoStationUpdater>();

var app = builder.Build();

// Eagerly initialize AirportFactory
Task.Run(async () =>
{
    try
    {
        // Get AirportFactory from the service provider
        var airportFactory = app.Services.GetRequiredService<AirportFactory>();

        // Initialize airports and load METAR/TAF data
        await airportFactory.createAirports();
        await airportFactory.infoStation.loadReports();

        Console.WriteLine("Airports initialized.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing AirportFactory: {ex.Message}");
    }
});

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
