using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class TafDto
    {
        public DateTime ReportTimeUtc { get; init; } //When the report was issued at the station.
        public DateTime FetchTimeUtc { get; init; } //When the report was fetched from the source.
        public string Raw { get; init; } //Raw TAF string.
        public string Type { get; init; } // "TAF" / "TAF AMD" / "TAF COR" as tekst.
        public bool IsAmd { get; init; } //Whether this report is an amendment "TAF AMD".
        public bool IsNewForUser { get; set; } //Whether this report is new for the user (not yet acknowledged).
    }
}
