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
            GetLatestPerIcaoAsync(IEnumerable<string> icaos, int windValidTime = 0, CancellationToken ct = default)
        {
            var list = icaos.Select(x => x.Trim().ToUpperInvariant()).ToArray();

            var hasTest = list.Contains("TEST", StringComparer.OrdinalIgnoreCase);
            var nonTest = list.Where(x => !x.Equals("TEST", StringComparison.OrdinalIgnoreCase)).ToArray();

            var accMetar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var accTaf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 1) Hent rigtige meldinger for ikke-TEST ICAO’er
            if (nonTest.Length > 0)
            {
                var (m, t) = await _real.GetLatestPerIcaoAsync(nonTest, windValidTime, ct);
                foreach (var kv in m) accMetar[kv.Key] = kv.Value;
                foreach (var kv in t) accTaf[kv.Key] = kv.Value;
            }

            // 2) Tilføj syntetisk TEST hvis efterspurgt
            if (hasTest)
            {
                var (tm, tt) = _test.BuildFor(new[] { "TEST" });
                foreach (var kv in tm) accMetar[kv.Key] = kv.Value;
                foreach (var kv in tt) accTaf[kv.Key] = kv.Value;
            }

            return (accMetar, accTaf);
        }
    }
}
