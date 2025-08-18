using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace MetarTaf_Backend.Services
{
    public sealed class NorthAviMetFetcher : IOpmetFetcher
    {
        private readonly HttpClient _http;
        public NorthAviMetFetcher(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetOpmetRawAsync(string query, int windValidTime = 0, CancellationToken ct = default)
        {
            var url = $"https://www.northavimet.com/NamConWS/rest/opmet/command/{windValidTime}/{Uri.EscapeDataString(query)}";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            req.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9,da;q=0.8");
            req.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");
            req.Headers.Referrer = new Uri("https://www.northavimet.com/metar-taf");

            using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            res.EnsureSuccessStatusCode();

            var bytes = await res.Content.ReadAsByteArrayAsync(ct);
            try
            {
                return Encoding.UTF8.GetString(bytes);
            }
            catch 
            { 
                return Encoding.Latin1.GetString(bytes);
            }
        }

        // Første <td> kan være: TAF | METAR | SPECI + valgfri AMD/COR
        static readonly Regex RxRow = new(
            @"<b>\s*(TAF|METAR|SPECI)(?:\s+(AMD|COR))?\s*</b>\s*</td>\s*<td[^>]*><b>\s*([A-Z]{4})\s*</b>\s*</td>\s*<td[^>]*><b>\s*([^<]+?)=\s*</b>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Hvis AMD/COR ligger i starten af body i stedet
        static readonly Regex RxModifier = new(@"^(AMD|COR)\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static string Collapse(string s)
        {
            return Regex.Replace(s.Replace('\u00A0', ' ').Replace('\t', ' '), @"\s+", " ").Trim();
        }

        public async Task<(Dictionary<string, string> Metar, Dictionary<string, string> Taf)>
            GetLatestPerIcaoAsync(IEnumerable<string> icaos, int windValidTime = 0, CancellationToken ct = default)
        {
            var query = string.Join(' ', icaos.Select(x => x.Trim().ToUpperInvariant()));
            var html = await GetOpmetRawAsync(query, windValidTime, ct);

            var metar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var taf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 1) HTML <table>-format (typisk)
            var matches = RxRow.Matches(html);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    var kindRaw = m.Groups[1].Value.ToUpperInvariant();                        // TAF/METAR/SPECI
                    var modCell = m.Groups[2].Success ? m.Groups[2].Value.ToUpperInvariant() : ""; // AMD/COR i 1. celle (valgfri)
                    var icao = m.Groups[3].Value.ToUpperInvariant();
                    var body = Collapse(m.Groups[4].Value);                                 // kan starte med AMD/COR/NIL/CNL/…

                    // Hvis ikke der var modifier i 1. celle, tjek body-start
                    var modifier = modCell;
                    if (string.IsNullOrEmpty(modifier))
                    {
                        var mm = RxModifier.Match(body);
                        if (mm.Success)
                        {
                            modifier = mm.Groups[1].Value.ToUpperInvariant(); // AMD/COR
                            body = body[mm.Length..].Trim();
                        }
                    }

                    // SPECI behandles som METAR-type
                    var kind = (kindRaw == "SPECI") ? "METAR" : kindRaw;

                    // Normaliseret linje
                    var line = string.IsNullOrEmpty(modifier)
                        ? $"{kind} {icao} {body}"
                        : $"{kind} {modifier} {icao} {body}";

                    if (kind == "METAR")
                    {
                        metar[icao] = line;
                    }
                    else
                    {
                        taf[icao] = line;
                    }
                }
                return (metar, taf);
            }

            // 2) Fallback: plain text (tillad valgfri modifier)
            var textNoTags = Regex.Replace(html, "<[^>]+>", " ");
            textNoTags = textNoTags.Replace("\r", ""); // bevar \n

            var lines = textNoTags
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Collapse) // kollaps spaces pr. linje
                .ToList();

            var rxMetar = new Regex(@"^(METAR|SPECI)(?:\s+(COR))?\s+([A-Z]{4})\s+(.+)$",
                                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var rxTaf = new Regex(@"^TAF(?:\s+(AMD|COR))?\s+([A-Z]{4})\s+(.+)$",
                                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (var l in lines)
            {
                var ma = rxMetar.Match(l);
                if (ma.Success)
                {
                    var hasCor = ma.Groups[2].Success;
                    var icao = ma.Groups[3].Value.ToUpperInvariant();
                    var body = ma.Groups[4].Value.Trim().TrimEnd('='); // ← fjern '='
                    metar[icao] = (hasCor ? $"METAR COR {icao} {body}" : $"METAR {icao} {body}")
                                    .Replace("  ", " ").Trim();
                    continue;
                }

                var ta = rxTaf.Match(l);
                if (ta.Success)
                {
                    var mod = ta.Groups[1].Success ? ta.Groups[1].Value.ToUpperInvariant() + " " : "";
                    var icao = ta.Groups[2].Value.ToUpperInvariant();
                    var body = ta.Groups[3].Value.Trim().TrimEnd('='); // ← fjern '='
                    taf[icao] = $"TAF {mod}{icao} {body}".Replace("  ", " ").Trim();
                }
            }


            return (metar, taf);
        }
    }
}
