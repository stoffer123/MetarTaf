using MetarTaf.Components;
using MetarTaf_Backend;
using MetarTaf_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

await AirportInfoService.createAirportInfo();


builder.Services.AddSingleton<AirportController>();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); ;

var app = builder.Build();

await AirportInfoService.createAirportInfo();

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

