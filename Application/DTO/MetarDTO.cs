using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class MetarDTO
    {
        public DateTime ReportTimeUtc { get; init; } //When the report was issued at the station.
        public DateTime FetchTimeUtc { get; init; } //When the report was fetched from the source.
        public string Raw { get; init; } //Raw METAR string.
        public string Type { get; init; } // "METAR" / "SPECI" / "COR" as tekst.
        public bool IsNewForUser { get; set; } //Whether this report is new for the user (not yet acknowledged).
    }
}
