using static MetarTaf.Components.Models.TafModels.TAF;
using System.Text.Json.Serialization;

namespace MetarTaf.Components.Models.MetarModels
{
    public class MetarAWC
    {
        [JsonPropertyName("metar_id")]
        public long? MetarId { get; set; }

        [JsonPropertyName("icaoId")]
        public string? IcaoId { get; set; }

        [JsonPropertyName("receiptTime")]
        public string? ReceiptTime { get; set; }

        [JsonPropertyName("obsTime")]
        public long? ObsTime { get; set; }

        [JsonPropertyName("reportTime")]
        public string? ReportTime { get; set; }

        [JsonPropertyName("temp")]
        public double? Temp { get; set; }

        [JsonPropertyName("dewp")]
        public double? Dewp { get; set; }

        [JsonPropertyName("wdir")]
        public object? Wdir { get; set; }

        [JsonPropertyName("wspd")]
        public int? Wspd { get; set; }

        [JsonPropertyName("wgst")]
        public object? Wgst { get; set; }

        [JsonPropertyName("visib")]
        public object? Visib { get; set; }

        [JsonPropertyName("altim")]
        public double? Altim { get; set; }

        [JsonPropertyName("slp")]
        public double? Slp { get; set; }

        [JsonPropertyName("qcField")]
        public int? QcField { get; set; }

        [JsonPropertyName("wxString")]
        public object? WxString { get; set; }

        [JsonPropertyName("presTend")]
        public double? PresTend { get; set; }

        [JsonPropertyName("maxT")]
        public object? MaxT { get; set; }

        [JsonPropertyName("minT")]
        public object? MinT { get; set; }

        [JsonPropertyName("maxT24")]
        public object? MaxT24 { get; set; }

        [JsonPropertyName("minT24")]
        public object? MinT24 { get; set; }

        [JsonPropertyName("precip")]
        public object? Precip { get; set; }

        [JsonPropertyName("pcp3hr")]
        public object? Pcp3hr { get; set; }

        [JsonPropertyName("pcp6hr")]
        public object? Pcp6hr { get; set; }

        [JsonPropertyName("pcp24hr")]
        public object? Pcp24hr { get; set; }

        [JsonPropertyName("snow")]
        public object? Snow { get; set; }

        [JsonPropertyName("vertVis")]
        public object? VertVis { get; set; }

        [JsonPropertyName("metarType")]
        public string? MetarType { get; set; }

        [JsonPropertyName("rawOb")]
        public string? RawOb { get; set; }

        [JsonPropertyName("mostRecent")]
        public int? MostRecent { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }

        [JsonPropertyName("elev")]
        public int? Elev { get; set; }

        [JsonPropertyName("prior")]
        public int? Prior { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("clouds")]
        public List<CloudAWC> Clouds { get; set; } = new();

        public class CloudAWC
        {
            [JsonPropertyName("cover")]
            public string? Cover { get; set; }

            [JsonPropertyName("base")]
            public object? Base { get; set; }
        }


        public Metar createMetar()
        {
            Metar metar = new Metar();
            metar.Raw = this.RawOb;
            metar.Station = IcaoId;

            // Parse receiptTime and assign it to metar.Time.Dt
            if (!string.IsNullOrEmpty(this.ReceiptTime))
            {
                string format = "yyyy-MM-dd HH:mm:ss"; // Specify the format
                metar.Time.Dt = DateTime.ParseExact(this.ReceiptTime, format, System.Globalization.CultureInfo.InvariantCulture);
            }

            return metar;
        }

    }
}
