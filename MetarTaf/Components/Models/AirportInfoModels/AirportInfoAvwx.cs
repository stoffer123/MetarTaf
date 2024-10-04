using MetarTaf.Components.Models.AirportInfoModels;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MetarTaf.Components.Models
{
    public class AirportInfoAvwx
    {
        [JsonPropertyName("city")]
        public string city { get; set; }

        [JsonPropertyName("country")]
        public string country { get; set; }

        [JsonPropertyName("elevation_ft")]
        public int elevation_ft { get; set; }

        [JsonPropertyName("elevation_m")]
        public int elevation_m { get; set; }

        [JsonPropertyName("gps")]
        public string gps { get; set; }

        [JsonPropertyName("iata")]
        public string iata { get; set; }

        [JsonPropertyName("icao")]
        public string icao { get; set; }

        [JsonPropertyName("latitude")]
        public double latitude { get; set; }

        [JsonPropertyName("local")]
        public string local { get; set; }

        [JsonPropertyName("longitude")]
        public double longitude { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("note")]
        public string note { get; set; }

        [JsonPropertyName("reporting")]
        public bool reporting { get; set; }

        [JsonPropertyName("runways")]
        public List<RunwayAvwx> runways { get; set; }

        [JsonPropertyName("state")]
        public string state { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("website")]
        public string website { get; set; }

        [JsonPropertyName("wiki")]
        public string wiki { get; set; }
        public class RunwayAvwx
        {
            [JsonPropertyName("length_ft")]
            public int length_ft { get; set; }

            [JsonPropertyName("width_ft")]
            public int width_ft { get; set; }

            [JsonPropertyName("ident1")]
            public string ident1 { get; set; }

            [JsonPropertyName("ident2")]
            public string ident2 { get; set; }
        }


        public AirportInfo createAirportInfo()
        {
            AirportInfo airportInfo = new AirportInfo();
            airportInfo.city = this.city;
            airportInfo.country = this.country;
            airportInfo.elevation_ft = this.elevation_ft;
            airportInfo.elevation_m = this.elevation_m;
            airportInfo.gps = this.gps;
            airportInfo.iata = this.iata;
            airportInfo.icao = this.icao;
            airportInfo.latitude = this.latitude;
            airportInfo.longitude = this.longitude;
            airportInfo.local = this.local;
            airportInfo.name = this.name;
            airportInfo.note = this.note;
            airportInfo.reporting = this.reporting;
            airportInfo.state = this.state;
            airportInfo.type = this.type;
            airportInfo.website = this.website;
            airportInfo.wiki = this.wiki;

            if (runways.Any()) 
            {
                foreach(RunwayAvwx runwayAvwx in runways)
                {
                    Runway runway = new Runway();
                    runway.length_ft = runwayAvwx.length_ft;
                    runway.width_ft = runwayAvwx.width_ft;
                    runway.ident1 = runwayAvwx.ident1;
                    runway.ident2 = runwayAvwx.ident2;
                    airportInfo.runways.Add(runway);
                }
            }


            return airportInfo;
        }

    }

}
