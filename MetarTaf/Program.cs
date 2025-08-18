using MetarTaf.Components;
using MetarTaf_Backend;
using MetarTaf_Backend.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// 1) Registrér typed HttpClient til fetcheren (før AirportController)
// Program.cs

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

// Test-kilden
builder.Services.AddSingleton<TestOpmetSource>();

// IMPORTANT: Bind IOpmetFetcher til COMPOSITE (ikke NorthAviMetFetcher)
builder.Services.AddSingleton<IOpmetFetcher>(sp =>
    new CompositeOpmetFetcher(
        sp.GetRequiredService<NorthAviMetFetcher>(),
        sp.GetRequiredService<TestOpmetSource>()
    )
);



// 2) AirportController afhænger af fetcher
builder.Services.AddSingleton<AirportController>(sp =>
{
    var fetcher = sp.GetRequiredService<IOpmetFetcher>();
    return new AirportController(fetcher, timerDelayMinutes: 1);
});

// 3) (almindelig Blazor Server opsætning)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// 4) Hvis AirportInfoService skal køre noget init der bruger DI, gør det EFTER Build()
using (var scope = app.Services.CreateScope())
{
    await AirportInfoService.createAirportInfo();

    var controller = scope.ServiceProvider.GetRequiredService<AirportController>();
    // Sikr at TEST er tracket fra start
    controller.getAirport("TEST");
}

// 5) Pipeline
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
