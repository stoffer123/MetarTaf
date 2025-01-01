using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend
{
    interface IInfoStation
    {
        AirportInfo GetAirportInfo(string icao);
        Dictionary<DateTime, WrappedMetar> getMetars(string icao);
        Dictionary<DateTime, WrappedTaf> getTafs(string icao);
    }
}
