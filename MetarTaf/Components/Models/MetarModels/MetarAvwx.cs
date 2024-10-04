using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static MetarTaf.Components.Models.MetarModels.Metar;

namespace MetarTaf.Components.Models.MetarModels
{
    public class MetarAvwx
    {
        
        [JsonPropertyName("altimeter")]
        public DataAvwx? Altimeter { get; set; }

        [JsonPropertyName("clouds")]
        public List<CloudAvwx>? Clouds { get; set; }

        [JsonPropertyName("density_altitude")]
        public int DensityAltitude { get; set; }

        [JsonPropertyName("dewpoint")]
        public DataAvwx? Dewpoint { get; set; }

        [JsonPropertyName("flight_rules")]
        public string? FlightRules { get; set; }

        [JsonPropertyName("meta")]
        public MetaAvwx? Meta { get; set; }

        [JsonPropertyName("other")]
        public List<object>? Other { get; set; }

        [JsonPropertyName("pressure_altitude")]
        public int PressureAltitude { get; set; }

        [JsonPropertyName("raw")]
        public string? Raw { get; set; }

        [JsonPropertyName("relative_humidity")]
        public double RelativeHumidity { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }

        [JsonPropertyName("remarks_info")]
        public RemarksInfoAvwx? RemarksInfo { get; set; }

        [JsonPropertyName("runway_visibility")]
        public List<object>? RunwayVisibility { get; set; }

        [JsonPropertyName("sanitized")]
        public string? Sanitized { get; set; }

        [JsonPropertyName("station")]
        public string? Station { get; set; }

        [JsonPropertyName("temperature")]
        public DataAvwx? Temperature { get; set; }

        [JsonPropertyName("time")]
        public TimeAvwx? Time { get; set; }


        [JsonPropertyName("units")]
        public UnitsAvwx? Units { get; set; }

        [JsonPropertyName("visibility")]
        public DataAvwx? Visibility { get; set; }

        [JsonPropertyName("wind_direction")]
        public DataAvwx? WindDirection { get; set; }


        [JsonPropertyName("wind_gust")]
        public object? WindGust { get; set; }


        [JsonPropertyName("wind_speed")]
        public DataAvwx? WindSpeed { get; set; }

        [JsonPropertyName("wind_variable_direction")]
        public List<DataAvwx>? WindVariableDirection { get; set; }

        [JsonPropertyName("wx_codes")]
        public List<object>? WxCodes { get; set; }



        public class DataAvwx
        {
            [JsonPropertyName("repr")]
            public string? Repr { get; set; }

            [JsonPropertyName("spoken")]
            public string? Spoken { get; set; }

            [JsonPropertyName("value")]
            public double Value { get; set; }
        }

        public class CloudAvwx
        {
            [JsonPropertyName("altitude")]
            public int Altitude { get; set; }

            [JsonPropertyName("direction")]
            public object? Direction { get; set; }

            [JsonPropertyName("modifier")]
            public object? Modifier { get; set; }

            [JsonPropertyName("repr")]
            public string? Repr { get; set; }

            [JsonPropertyName("type")]
            public string? Type { get; set; }
        }

        public class TimeAvwx
        {
            [JsonPropertyName("dt")]
            public DateTime? Dt { get; set; }

            [JsonPropertyName("repr")]
            public string? Repr { get; set; }
        }

        public class MetaAvwx
        {
            [JsonPropertyName("stations_updated")]
            public string? StationsUpdated { get; set; }

            [JsonPropertyName("timestamp")]
            public DateTime Timestamp { get; set; }
        }

        public class RemarksInfoAvwx
        {
            public List<object>? Codes { get; set; }
            [JsonPropertyName("dewpoint_decimal")]
            public object? DewpointDecimal { get; set; }
            [JsonPropertyName("maximum_temperature_24")]
            public object? MaximumTemperature24 { get; set; }
            [JsonPropertyName("maximum_temperature_6")]
            public object? MaximumTemperature6 { get; set; }
            [JsonPropertyName("minimum_temperature_24")]
            public object? MinimumTemperature24 { get; set; }
            [JsonPropertyName("minimum_temperature_6")]
            public object? MinimumTemperature6 { get; set; }
            [JsonPropertyName("precip_24_hours")]
            public object? Precip24Hours { get; set; }
            [JsonPropertyName("precip_36_hours")]
            public object? Precip36Hours { get; set; }
            [JsonPropertyName("precip_hourly")]
            public object? PrecipHourly { get; set; }
            [JsonPropertyName("pressure_tendency")]
            public object? PressureTendency { get; set; }
            [JsonPropertyName("sea_level_pressure")]
            public object? SeaLevelPressure { get; set; }
            [JsonPropertyName("snow_depth")]
            public object? SnowDepth { get; set; }
            [JsonPropertyName("sunshine_minutes")]
            public object? SunshineMinutes { get; set; }
            [JsonPropertyName("temperature_decimal")]
            public object? TemperatureDecimal { get; set; }
        }

        public class UnitsAvwx
        {
            public string? Accumulation { get; set; }
            public string? Altimeter { get; set; }
            public string? Altitude { get; set; }
            public string? Temperature { get; set; }
            public string? Visibility { get; set; }
            [JsonPropertyName("wind_speed")]
            public string? WindSpeed { get; set; }
        }

        public Metar createMetar()
        {
            Metar metar = new Metar();

            metar.Raw = this.Raw;
            metar.Time.Dt = this.Time.Dt;

            return metar;
        }

    }

}
