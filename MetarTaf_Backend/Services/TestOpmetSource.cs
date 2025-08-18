using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Services
{
    public sealed class TestOpmetSource
    {
        // Roterende eksempler — tilpas bare indholdet
        private static readonly string[] _metars =
        {
        "METAR TEST 101946Z 18010KT 9999 FEW020 21/12 Q1018=",
        "SPECI TEST 101948Z 22015G25KT 8000 -SHRA SCT015 20/12 Q1017=",
        "METAR COR TEST 101950Z 20012KT CAVOK 21/11 Q1017=",
        "SPECI COR TEST 101955Z 18018KT 9999 BKN030 20/11 Q1016="
    };

        private static readonly string[] _tafs =
        {
        "TAF TEST 1019/1024 20010KT CAVOK=",
        "TAF AMD TEST 101915Z 1019/1024 22015G25KT 8000 SHRA SCT015 BKN030=",
        "TAF COR TEST 101920Z 1019/1024 19012KT CAVOK=",
        "TAF TEST 1020/1026 21012KT 9999 FEW020="
    };

        private int _metarIdx = -1;
        private int _tafIdx = -1;

        private string Next(string[] arr, ref int idx)
        {
            var i = Interlocked.Increment(ref idx);
            return arr[Math.Abs(i) % arr.Length];
        }

        public (Dictionary<string, string> Metar, Dictionary<string, string> Taf)
            BuildFor(IEnumerable<string> icaos)
        {
            var metar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var taf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var raw in icaos)
            {
                var icao = raw.Trim().ToUpperInvariant();
                if (icao == "TEST")
                {
                    metar[icao] = Next(_metars, ref _metarIdx);
                    taf[icao] = Next(_tafs, ref _tafIdx);
                }
            }

            return (metar, taf);
        }
    }
}
