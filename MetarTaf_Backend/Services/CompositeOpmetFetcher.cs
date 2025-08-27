using Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Services
{
    public sealed class CompositeOpmetFetcher : IOpmetFetcher
    {   
        private readonly IOpmetFetcher _real;     // NorthAviMetFetcher
        private readonly TestOpmetSource _test;

        public CompositeOpmetFetcher(IOpmetFetcher real, TestOpmetSource test)
        {
            _real = real;
            _test = test;
        }

        public async Task<(Dictionary<string, string> Metar, Dictionary<string, string> Taf)>
            GetLatestPerIcaoRawAsync(IEnumerable<string> icaos, int windValidTime = 0, CancellationToken ct = default)
        {
            var list = icaos.Select(s => s.Trim().ToUpperInvariant()).Distinct().ToArray();

            // 1) Hent fra real for alle (inkl. TEST, det er fint; vi overskriver TEST bagefter)
            var (metar, taf) = await _real.GetLatestPerIcaoRawAsync(list, windValidTime, ct);

            // 2) Merge TEST fra test-kilden (baseret på NU)
            var now = DateTime.UtcNow;
            var (metarTest, tafTest) = _test.GetFor(now, list);

            foreach (var kv in metarTest) metar[kv.Key] = kv.Value;
            foreach (var kv in tafTest) taf[kv.Key] = kv.Value;

            return (metar, taf);
        }

    }
}
