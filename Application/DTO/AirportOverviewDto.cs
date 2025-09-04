using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AirportOverviewDto
    {
        public string Icao { get; init; }
        public string Country { get; init; }
        public MetarDTO? LatestMetar { get; set; }
        public TafDto? LatestTaf { get; set; }
    }
}
