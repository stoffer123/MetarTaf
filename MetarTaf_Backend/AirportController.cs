using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend
{
    public static class AirportController
    {
        private static Dictionary<string, IAirport> airports = new();



        public static List<string> getAirportIcaoList()
        {
            List<string> strings = airports.Keys.ToList();

            return strings;
        }
    }
}
