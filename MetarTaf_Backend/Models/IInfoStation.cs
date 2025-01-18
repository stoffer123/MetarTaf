using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    internal interface IInfoStation
    {
        void removeObserver(IAirport observer);
        void addObserver(IAirport observer);
        void notifyTafChange();
        void notifyMetarChange();
        void notifyAirportInfoChange();
    }
}
