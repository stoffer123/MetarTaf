using Domain.Ports;
using Domain.ValueObjects;
using Domain.Entities;

namespace Domain.Factories
{
    public sealed class AirportFactory
    {
        private readonly IAirportInfoProvider _infoProvider;
        private readonly IInfoStation _infoStation;

        public AirportFactory(IAirportInfoProvider infoProvider, IInfoStation infoStation)
        {
            _infoProvider = infoProvider;
            _infoStation = infoStation;
        }

        public async Task<IAirport?> CreateAsync(string icao, CancellationToken ct = default)
        {
            var info = await _infoProvider.GetByIcaoAsync(icao, ct);
            if (info is null) return null;

            return new Airport(_infoStation, info);
        }
    }
}
