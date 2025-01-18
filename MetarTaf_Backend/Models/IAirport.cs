using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    internal interface IAirport
    {
        void updateMetars();
        void updateTafs();
        void updateAirportInfo();
    }
}
