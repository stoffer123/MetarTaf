using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>Samlet domænemodel for en OPMET linje (METAR/SPECI/TAF, inkl. COR/AMD).</summary>
    public sealed class OpmetReport
    {
        public string Icao { get; }
        public ReportKind Kind { get; }
        public ReportModifier Modifier { get; }
        public DateTime IssuedAtUtc { get; }   // “report time”
        public DateTime FetchedAtUtc { get; }  // når vi hentede den
        public string RawText { get; }         // hele rå linjen, som vist til bruger

        public OpmetReport(string icao, ReportKind kind, ReportModifier modifier,
                           DateTime issuedAtUtc, DateTime fetchedAtUtc, string rawText)
        {
            Icao = icao.ToUpperInvariant();
            Kind = kind;
            Modifier = modifier;
            IssuedAtUtc = DateTime.SpecifyKind(issuedAtUtc, DateTimeKind.Utc);
            FetchedAtUtc = DateTime.SpecifyKind(fetchedAtUtc, DateTimeKind.Utc);
            RawText = rawText ?? "";
        }
    }
}
