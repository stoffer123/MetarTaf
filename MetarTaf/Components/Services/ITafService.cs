using MetarTaf.Components.Models;

namespace MetarTaf.Components.Services
{
    public interface ITafService
    {
        Task<TAF?> GetTAFAsync(string icao);
    }

}
