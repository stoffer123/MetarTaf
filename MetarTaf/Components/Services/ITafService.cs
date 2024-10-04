using MetarTaf.Components.Models.TafModels;

namespace MetarTaf.Components.Services
{
    public interface ITafService
    {
        Task<TAF?> GetTAFAsync(string icao);
    }

}
