using MetarTaf.Components.Models.MetarModels;

namespace MetarTaf.Components.Services
{
    public interface IMetarService
    {
        Task<Metar?> GetMetarAsync(string icao); //Run async to get a metar.
    }
}
