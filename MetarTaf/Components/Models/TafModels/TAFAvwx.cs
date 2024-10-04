using System.Text.Json.Serialization;

namespace MetarTaf.Components.Models
{
    public class TAFAvwx
    {
        [JsonPropertyName("meta")]
        public MetaAvwx? meta { get; set; }

        [JsonPropertyName("raw")]
        public string? raw { get; set; }

        [JsonPropertyName("station")]
        public string? station { get; set; }

        [JsonPropertyName("time")]
        public TimeAvwx? time { get; set; }

        [JsonPropertyName("remarks")]
        public string? remarks { get; set; }

        [JsonPropertyName("forecast")]
        public ForecastAvwx[]? forecast { get; set; }

        [JsonPropertyName("start_time")]
        public Start_TimeAvwx? start_time { get; set; }

        [JsonPropertyName("end_time")]
        public End_TimeAvwx? end_time { get; set; }

        [JsonPropertyName("max_temp")]
        public string? max_temp { get; set; }

        [JsonPropertyName("min_temp")]
        public string? min_temp { get; set; }

        [JsonPropertyName("alts")]
        public object? alts { get; set; }

        [JsonPropertyName("temps")]
        public object? temps { get; set; }

        [JsonPropertyName("units")]
        public UnitsAvwx? units { get; set; }


        public class MetaAvwx
        {
            [JsonPropertyName("timestamp")]
            public string? timestamp { get; set; }
        }

        public class TimeAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("dt")]
            public DateTime? dt { get; set; }
        }

        public class Start_TimeAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("dt")]
            public DateTime? dt { get; set; }
        }

        public class End_TimeAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("dt")]
            public DateTime? dt { get; set; }
        }

        public class UnitsAvwx
        {
            [JsonPropertyName("altimeter")]
            public string? altimeter { get; set; }

            [JsonPropertyName("altitude")]
            public string? altitude { get; set; }

            [JsonPropertyName("temperature")]
            public string? temperature { get; set; }

            [JsonPropertyName("visibility")]
            public string? visibility { get; set; }

            [JsonPropertyName("wind_speed")]
            public string? wind_speed { get; set; }
        }

        public class ForecastAvwx
        {
            [JsonPropertyName("altimeter")]
            public string? altimeter { get; set; }

            [JsonPropertyName("clouds")]
            public CloudAvwx[]? clouds { get; set; }

            [JsonPropertyName("flight_rules")]
            public string? flight_rules { get; set; }

            [JsonPropertyName("other")]
            public object[]? other { get; set; }

            [JsonPropertyName("sanitized")]
            public string? sanitized { get; set; }

            [JsonPropertyName("visibility")]
            public VisibilityAvwx? visibility { get; set; }

            [JsonPropertyName("wind_direction")]
            public Wind_DirectionAvwx? wind_direction { get; set; }

            [JsonPropertyName("wind_gust")]
            public Wind_GustAvwx? wind_gust { get; set; }

            [JsonPropertyName("wind_speed")]
            public Wind_SpeedAvwx? wind_speed { get; set; }

            [JsonPropertyName("wx_codes")]
            public Wx_CodesAvwx[]? wx_codes { get; set; }

            [JsonPropertyName("end_time")]
            public End_Time1Avwx? end_time { get; set; }

            [JsonPropertyName("icing")]
            public object[]? icing { get; set; }

            [JsonPropertyName("probability")]
            public object? probability { get; set; }

            [JsonPropertyName("raw")]
            public string? raw { get; set; }

            [JsonPropertyName("start_time")]
            public Start_Time1Avwx? start_time { get; set; }

            [JsonPropertyName("turbulence")]
            public object[]? turbulence { get; set; }

            [JsonPropertyName("type")]
            public string? type { get; set; }

            [JsonPropertyName("wind_shear")]
            public object? wind_shear { get; set; }

            [JsonPropertyName("summary")]
            public string? summary { get; set; }
        }

        public class VisibilityAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("value")]
            public object? value { get; set; }

            [JsonPropertyName("spoken")]
            public string? spoken { get; set; }
        }

        public class Wind_DirectionAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("value")]
            public int? value { get; set; }

            [JsonPropertyName("spoken")]
            public string? spoken { get; set; }
        }

        public class Wind_GustAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("value")]
            public int? value { get; set; }

            [JsonPropertyName("spoken")]
            public string? spoken { get; set; }
        }

        public class Wind_SpeedAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("value")]
            public int? value { get; set; }

            [JsonPropertyName("spoken")]
            public string? spoken { get; set; }
        }

        public class End_Time1Avwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("dt")]
            public DateTime? dt { get; set; }
        }

        public class Start_Time1Avwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("dt")]
            public string? dt { get; set; }
        }

        public class CloudAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("type")]
            public string? type { get; set; }

            [JsonPropertyName("altitude")]
            public int? altitude { get; set; }

            [JsonPropertyName("modifier")]
            public object? modifier { get; set; }

            [JsonPropertyName("direction")]
            public object? direction { get; set; }
        }

        public class Wx_CodesAvwx
        {
            [JsonPropertyName("repr")]
            public string? repr { get; set; }

            [JsonPropertyName("value")]
            public string? value { get; set; }
        }



    }
}
