using System.Globalization;

public sealed class TestOpmetSource
{
    // Roter over 4 scenarier for at skabe variation
    private static readonly (string wind, string vis, string clouds, string temp)[] MetarSlots = new[]
    {
        ("21012KT", "9999", "FEW020", "18/12"),
        ("18008KT", "8000", "SCT025", "17/11"),
        ("24015G25KT", "9999", "BKN030", "16/10"),
        ("VRB03KT", "CAVOK", "", "19/12"),
    };

    private static readonly string[] TafBodies = new[]
    {
        // rolig
        "9999 SCT025",
        // lidt byger
        "8000 -SHRA SCT020 BKN030",
        // blæst
        "9999 24015G25KT SCT030",
        // CAVOK
        "CAVOK"
    };

    /// <summary>
    /// Returnér dynamisk METAR/TAF for TEST baseret på nuværende minut.
    /// </summary>
    public (Dictionary<string, string> Metar, Dictionary<string, string> Taf) GetFor(DateTime nowUtc, IEnumerable<string> icaos)
    {
        var wantTest = icaos.Any(x => string.Equals(x, "TEST", StringComparison.OrdinalIgnoreCase));
        var metar = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var taf = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!wantTest)
            return (metar, taf);

        // Brug “minut-rotor” til at vælge slot og AMD/COR
        var minute = (int)Math.Floor((nowUtc - DateTime.UnixEpoch).TotalMinutes);
        var slot = minute % MetarSlots.Length;

        // METAR tid: DDHHMMZ (UTC)
        var dd = nowUtc.Day.ToString("00", CultureInfo.InvariantCulture);
        var hhmm = nowUtc.ToString("HHmm", CultureInfo.InvariantCulture);
        var metarTimeToken = $"{dd}{hhmm}Z";

        var (wind, vis, clouds, temp) = MetarSlots[slot];
        var cloudsPart = string.IsNullOrWhiteSpace(clouds) ? "" : $" {clouds}";

        // Hver 2. minut laver vi en COR for at vise variation
        var metarPrefix = (minute % 2 == 0) ? "METAR" : "METAR COR";

        var qnh = "Q1015"; // fast for test
        var metarLine = $"{metarPrefix} TEST {metarTimeToken} {wind} {vis}{cloudsPart} {temp} {qnh}=";
        metar["TEST"] = metarLine.Trim();

        // TAF: issue + gyldighed (24 timer)
        var issue = metarTimeToken; // samme som metar-issue for simpelt showcase
        var from = nowUtc.ToString("ddHH", CultureInfo.InvariantCulture);
        var to = nowUtc.AddHours(24).ToString("ddHH", CultureInfo.InvariantCulture);
        var tafCore = TafBodies[slot];

        // Hver 3. minut laver vi AMD for at teste blink/lyd
        var tafPrefix = (minute % 3 == 0) ? "TAF AMD" : "TAF";
        var tafLine = $"{tafPrefix} TEST {issue} {from}/{to} {tafCore}=";
        taf["TEST"] = tafLine.Trim();

        return (metar, taf);
    }
}
