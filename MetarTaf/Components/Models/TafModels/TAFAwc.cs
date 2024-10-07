using MetarTaf.Components.Models.MetarModels;
using System.Text.Json.Serialization;

namespace MetarTaf.Components.Models.TafModels
{
    public class TAFAwc
    {
        [JsonPropertyName("tafId")]
        public object? tafId { get; set; }

        [JsonPropertyName("icaoId")]
        public string? icao { get; set; }

        [JsonPropertyName("dbPopTime")]
        public string? dbPopTime { get; set; }

        [JsonPropertyName("issueTime")]
        public string? issueTime { get; set; }

        [JsonPropertyName("validTimeFrom")]
        public object? validTimeFrom { get; set; }

        [JsonPropertyName("validTimeTo")]
        public object? validTimeTo { get; set; }

        [JsonPropertyName("rawTAF")]
        public string? rawTAF { get; set; }

        [JsonPropertyName("mostRecent")]
        public object? mostRecent {  get; set; }

        [JsonPropertyName("remarks")]
        public string? remarks { get; set; }

        [JsonPropertyName("lat")]
        public double? latitude { get; set; }

        [JsonPropertyName("lon")]
        public double? longitude { get; set; }

        [JsonPropertyName("elev")]
        public double? elevation { get; set; }

        [JsonPropertyName("prior")]
        public object? prior {  get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("fcsts")]
        public List<Forecast> forecasts { get; set; } = new();


        public class Forecast
        {
            [JsonPropertyName("timeGroup")]
            public object? timeGroup { get; set; }

            [JsonPropertyName("timeFrom")]
            public object? timeFrom { get; set; }

            [JsonPropertyName("timeTo")]
            public object? timeTo { get; set; }

            [JsonPropertyName("timeBec")]
            public object? timeBec { get; set; }

            [JsonPropertyName("fcstChange")]
            public string? forecastChange { get; set; }

            [JsonPropertyName("probability")]
            public object? probability { get; set; }

            [JsonPropertyName("wdir")]
            public object? windDirection { get; set; }

            [JsonPropertyName("wspd")]
            public object? windSpeed { get; set; }

            [JsonPropertyName("wgst")]
            public object? windGust { get; set; }

            [JsonPropertyName("wshearHgt")]
            public object? windShearHgt { get; set; }

            [JsonPropertyName("wshearDir")]
            public object? windShearDirection { get; set; }

            [JsonPropertyName("wshearSpd")]
            public object? windShearSpeed { get; set; }

            [JsonPropertyName("visib")]
            public object? visibility { get; set; }

            [JsonPropertyName("altim")]
            public object? altimeter { get; set; }

            [JsonPropertyName("vertVis")]
            public object? vertVis { get; set; }

            [JsonPropertyName("wxString")]
            public string? wxString { get; set; }

            [JsonPropertyName("notDecoded")]
            public object? notDecoded { get; set; }

            [JsonPropertyName("clouds")]
            public List<Cloud?> clouds { get; set; } = new();

            [JsonPropertyName("icgTurb")]
            public List<object?> icgTurb { get; set; } = new();

            [JsonPropertyName("temp")]
            public List<object?> temp { get; set; } = new();



        }

        public class Cloud
        {
            [JsonPropertyName("cover")]
            public string? cover { get; set; }

            [JsonPropertyName("base")]
            public object? baseElevation { get; set; }

            [JsonPropertyName("type")]
            public object? type { get; set; }


        }



        public TAF createTAF()
        {
            TAF taf = new TAF();

            taf.raw = this.rawTAF;
            taf.station = this.icao;

            // Parse receiptTime and assign it to metar.Time.Dt
            if (!string.IsNullOrEmpty(this.issueTime))
            {
                string format = "yyyy-MM-dd HH:mm:ss"; // Specify the format
                taf.time.dt = DateTime.ParseExact(this.issueTime, format, System.Globalization.CultureInfo.InvariantCulture);
            
            }

            Console.WriteLine($"Created TAF with station: {taf.station} time: {taf.time.dt.ToString()} and value: {taf.raw}");
            return taf;
        }


    }
}
