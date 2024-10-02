using MetarTaf.Components.Models;

namespace MetarTaf.Components.Services
{
    public interface IAirportInfoService
    {
        Task<AirportInfo?> GetAirportInfoAsync(string ident);
    }
}
