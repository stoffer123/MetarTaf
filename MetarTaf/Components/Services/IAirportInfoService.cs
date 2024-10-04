using MetarTaf.Components.Models.AirportInfoModels;

namespace MetarTaf.Components.Services
{
    public interface IAirportInfoService
    {
        Task<AirportInfo?> GetAirportInfoAsync(string ident);
    }
}
