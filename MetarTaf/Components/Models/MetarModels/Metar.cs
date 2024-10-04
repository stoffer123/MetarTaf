using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json.Serialization;
using static MetarTaf.Components.Models.MetarModels.Metar;

namespace MetarTaf.Components.Models.MetarModels
{
    public class Metar
    {
     
        public Data? Altimeter { get; set; }
        public List<Data_Clouds>? Clouds { get; set; }
        public int DensityAltitude { get; set; }
        public Data? Dewpoint { get; set; }
        public string? FlightRules { get; set; }
        public Data_Meta? Meta { get; set; }
        public List<object>? Other { get; set; }
        public int PressureAltitude { get; set; }
        public string? Raw { get; set; }
        public double RelativeHumidity { get; set; }
        public string? Remarks { get; set; }
        public Data_RemarksInfo? RemarksInfo { get; set; }
        public List<object>? RunwayVisibility { get; set; }
        public string? Sanitized { get; set; }
        public string? Station { get; set; }
        public Data? Temperature { get; set; }
        public Data_Time? Time { get; set; }
        public Data_Units? Units { get; set; }
        public Data? Visibility { get; set; }
        public Data? WindDirection { get; set; }
        public object? WindGust { get; set; }
        public Data? WindSpeed { get; set; }
        public List<Data>? WindVariableDirection { get; set; }
        public List<object>? WxCodes { get; set; }

        public class Data
        {
            public string? Repr { get; set; }
            public string? Spoken { get; set; }
            public double Value { get; set; }
        }

        public class Data_Clouds
        {
            public int Altitude { get; set; }
            public object? Direction { get; set; }
            public object? Modifier { get; set; }
            public string? Repr { get; set; }
            public string? Type { get; set; }
        }

        public class Data_Time
        {
            public DateTime Dt { get; set; }
            public string? Repr { get; set; }
        }


        public class Data_Meta
        {
            public string? StationsUpdated { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public class Data_RemarksInfo
        {
            public List<object>? Codes { get; set; }
            public object? DewpointDecimal { get; set; }
            public object? MaximumTemperature24 { get; set; }
            public object? MaximumTemperature6 { get; set; }
            public object? MinimumTemperature24 { get; set; }
            public object? MinimumTemperature6 { get; set; }
            public object? Precip24Hours { get; set; }
            public object? Precip36Hours { get; set; }
            public object? PrecipHourly { get; set; }
            public object? PressureTendency { get; set; }
            public object? SeaLevelPressure { get; set; }
            public object? SnowDepth { get; set; }
            public object? SunshineMinutes { get; set; }
            public object? TemperatureDecimal { get; set; }
        }

        public class Data_Units
        {
            public string? Accumulation { get; set; }
            public string? Altimeter { get; set; }
            public string? Altitude { get; set; }
            public string? Temperature { get; set; }
            public string? Visibility { get; set; }
            public string? WindSpeed { get; set; }
        }





    }
}
