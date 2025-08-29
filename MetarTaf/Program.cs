using System.Net;
using Application;               // AirportController
using Domain.Factories;          // AirportFactory
using Domain.Ports;              // IOpmetFetcher, IInfoStation, IAirportInfoProvider
using MetarTaf.Components;       // App (Razor root)
using MetarTaf_Backend;          // AirportInfoService (infra loader)
using MetarTaf_Backend.Models;   // AirportInfoStation (IInfoStation-impl)
using MetarTaf_Backend.Services; // NorthAviMetFetcher, TestOpmetSource, CompositeOpmetFetcher, AirportInfoProvider
                                 // (AirportInfoProvider = adapter der wrapper AirportInfoService)

var builder = WebApplication.CreateBuilder(args);

// ---------- Infrastruktur: typed HttpClient til NorthAviMetFetcher ----------
builder.Services.AddHttpClient<NorthAviMetFetcher>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(20);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip |
                             DecompressionMethods.Deflate |
                             DecompressionMethods.Brotli
});

// Test-kilde (til “TEST”-lufthavn mm.)
builder.Services.AddSingleton<TestOpmetSource>();

// Bind IOpmetFetcher til en composite (prod + test)
builder.Services.AddSingleton<IOpmetFetcher>(sp =>
    new CompositeOpmetFetcher(
        sp.GetRequiredService<NorthAviMetFetcher>(),
        sp.GetRequiredService<TestOpmetSource>()
    )
);

// ---------- Domain-porte/adaptere + services ----------

// IInfoStation (⚠️ VIGTIGT: INGEN AirportController i ctor → undgå cirkel!)
builder.Services.AddSingleton<IInfoStation>(sp =>
    new AirportInfoStation(sp.GetRequiredService<IOpmetFetcher>()));

// AirportInfoProvider (adapter over AirportInfoService) til domænets port
builder.Services.AddSingleton<IAirportInfoProvider, AirportInfoProvider>();

// AirportFactory (domæne) – forventer IAirportInfoProvider + IInfoStation
builder.Services.AddSingleton<AirportFactory>();

// ---------- Application-layer ----------
builder.Services.AddSingleton<IAirportController>(sp =>
    new AirportController(
        sp.GetRequiredService<IInfoStation>(),
        sp.GetRequiredService<AirportFactory>(),
        timerDelayMinutes: 1
    )
);

// (valgfrit) eksponer også via interface hvis du har IAirportController
// builder.Services.AddSingleton<IAirportController>(sp => sp.GetRequiredService<AirportController>());

// ---------- Blazor Server ----------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ---------- Init og bootstrap efter Build() ----------
using (var scope = app.Services.CreateScope())
{
    // Loader airports-data til AirportInfoService (infra)
    await AirportInfoService.createAirportInfo();

    // Sørg for at TEST altid er aktiv
    var controller = scope.ServiceProvider.GetRequiredService<IAirportController>();
    await controller.GetAirportAsync("TEST");
}

// ---------- Pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/healthz", () => Results.Ok("ok"));

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
