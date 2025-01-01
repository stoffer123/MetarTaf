using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    public class AirportInfo
    {
        string icao { get; }
        string name { get; }
        string wmo { get; }
        string longitude { get; }
        string latitude { get; }
        string altitude { get; }

        public AirportInfo(string icao, string name, string wmo, string longitude, string latitude, string altitude)
        {
            this.icao = icao;
            this.name = name;
            this.wmo = wmo;
            this.longitude = longitude;
            this.latitude = latitude;+

            this.altitude = altitude;
        }
    }
}
