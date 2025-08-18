using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Services
{
    public interface IOpmetFetcher
    {
        Task<(Dictionary<string, string> Metar, Dictionary<string, string> Taf)>
            GetLatestPerIcaoAsync(IEnumerable<string> icaos, int windValidTime = 0, CancellationToken ct = default);
    }
}
