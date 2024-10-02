using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace MetarTaf.Components.Models.MetarModels
{
    public class Metar
    {
        //Create New constructor for each API
        public Metar(MetarAvwx metar)  //Constructor for Avwx API
        {
            Altimeter = metar.Altimeter;
            Clouds = metar.Clouds;
            DensityAltitude = metar.DensityAltitude;
            Dewpoint = metar.Dewpoint;
            FlightRules = metar.FlightRules;
            Meta = metar.Meta;
            Other = metar.Other;
            PressureAltitude = metar.PressureAltitude;
            Raw = metar.Raw;
            RelativeHumidity = metar.RelativeHumidity;
            Remarks = metar.Remarks;
            RemarksInfo = metar.RemarksInfo;
            RunwayVisibility = metar.RunwayVisibility;
            Sanitized = metar.Sanitized;
            Station = metar.Station;
            Temperature = metar.Temperature;
            Time = metar.Time;
            Units = metar.Units;
            Visibility = metar.Visibility;
            WindDirection = metar.WindDirection;
            WindGust = metar.WindGust;
            WindSpeed = metar.WindSpeed;
            WindVariableDirection = metar.WindVariableDirection;
            WxCodes = metar.WxCodes;           
        }

        public Altimeter? Altimeter { get; set; }
        public List<Cloud>? Clouds { get; set; }
        public int DensityAltitude { get; set; }
        public Temperature? Dewpoint { get; set; }
        public string? FlightRules { get; set; }
        public Meta? Meta { get; set; }
        public List<object>? Other { get; set; }
        public int PressureAltitude { get; set; }
        public string? Raw { get; set; }
        public double RelativeHumidity { get; set; }
        public string? Remarks { get; set; }
        public RemarksInfo? RemarksInfo { get; set; }
        public List<object>? RunwayVisibility { get; set; }
        public string? Sanitized { get; set; }
        public string? Station { get; set; }
        public Temperature? Temperature { get; set; }
        public Time? Time { get; set; }
        public Units? Units { get; set; }
        public Visibility? Visibility { get; set; }
        public WindDirection? WindDirection { get; set; }
        public object? WindGust { get; set; }
        public WindSpeed? WindSpeed { get; set; }
        public List<WindVariableDirection>? WindVariableDirection { get; set; }
        public List<object>? WxCodes { get; set; }
    }

    public class Altimeter
    {
        public string? Repr { get; set; }
        public string? Spoken { get; set; }
        public double Value { get; set; }
    }

    public class Cloud
    {
        public int Altitude { get; set; }
        public object? Direction { get; set; }
        public object? Modifier { get; set; }
        public string? Repr { get; set; }
        public string? Type { get; set; }
    }

    public class Temperature
    {
        public string? Repr { get; set; }
        public string? Spoken { get; set; }
        public double Value { get; set; }
    }

    public class Time
    {
        public DateTime Dt { get; set; }
        public string? Repr { get; set; }
    }

    public class Visibility
    {
        public string? Repr { get; set; }
        public string? Spoken { get; set; }
        public double Value { get; set; }
    }

    public class WindDirection
    {
        public string? Repr { get; set; }
        public string? Spoken { get; set; }
        public double Value { get; set; }
    }

    public class WindSpeed
    {
        public string? Repr { get; set; }
        public string? Spoken { get; set; }
        public double Value { get; set; }
    }

    public class WindVariableDirection
    {
        public string? Repr { get; set; }
        public string? Spoken { get; set; }
        public double Value { get; set; }
    }

    public class Meta
    {
        public string? StationsUpdated { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RemarksInfo
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

    public class Units
    {
        public string? Accumulation { get; set; }
        public string? Altimeter { get; set; }
        public string? Altitude { get; set; }
        public string? Temperature { get; set; }
        public string? Visibility { get; set; }
        public string? WindSpeed { get; set; }
    }
}
