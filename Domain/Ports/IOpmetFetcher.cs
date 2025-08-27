using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ports
{
    /// <summary>
    /// Port for at hente seneste OPMET pr. ICAO. Adapter i Infrastructure implementerer denne.
    /// </summary>
    public interface IOpmetFetcher
    {
        /// <returns>
        /// Tuple af (metarMap, tafMap) hvor værdi er komplette rå linjer,
        /// eller – alternativt – returnér allerede mappet til OpmetReport (se nedenfor).
        /// </returns>
        Task<(Dictionary<string, string> Metar, Dictionary<string, string> Taf)>
            GetLatestPerIcaoRawAsync(IEnumerable<string> icaos, int windValidTime = 0, CancellationToken ct = default);

        /// <summary>Valgfrit: en port der returnerer domæneobjekter direkte.</summary>
        //Task<IReadOnlyList<OpmetReport>> GetLatestPerIcaoAsync(IEnumerable<string> icaos, int windValidTime = 0, CancellationToken ct = default);
    }
}
