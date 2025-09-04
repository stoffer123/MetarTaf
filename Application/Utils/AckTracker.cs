using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public sealed class AckTracker
    {
        private readonly Dictionary<string, DateTime> _metar = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, DateTime> _taf = new(StringComparer.OrdinalIgnoreCase);

        public bool IsMetarNew(string icao, DateTime reportUtc) =>
            !_metar.TryGetValue(icao, out var ack) || reportUtc > ack;

        public bool IsTafNew(string icao, DateTime reportUtc) =>
            !_taf.TryGetValue(icao, out var ack) || reportUtc > ack;

        public void AckMetar(string icao, DateTime reportUtc) => _metar[icao] = reportUtc;
        public void AckTaf(string icao, DateTime reportUtc) => _taf[icao] = reportUtc;

        public IReadOnlyDictionary<string, DateTime> SnapshotMetar() => _metar;
        public IReadOnlyDictionary<string, DateTime> SnapshotTaf() => _taf;

        public void Load(
            IReadOnlyDictionary<string, DateTime>? metar,
            IReadOnlyDictionary<string, DateTime>? taf)
        {
            _metar.Clear(); _taf.Clear();
            if (metar != null) foreach (var kv in metar) _metar[kv.Key] = DateTime.SpecifyKind(kv.Value, DateTimeKind.Utc);
            if (taf != null) foreach (var kv in taf) _taf[kv.Key] = DateTime.SpecifyKind(kv.Value, DateTimeKind.Utc);
        }
    }

}
