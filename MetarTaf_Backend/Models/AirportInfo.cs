using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    public class AirportInfo
    {
        public string icaoId { get; set; }
        public string iataId { get; set; }
        public string faaId { get; set; }
        public string wmoId { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public int elev { get; set; }
        public string site { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public int priority { get; set; }




    }
}
