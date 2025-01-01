using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    internal class Airport
    {
        IInfoStation infoStation;

        AirportInfo airportInfo;
        Dictionary<DateTime, WrappedMetar> metars;
        Dictionary<DateTime, WrappedTaf> tafs;

        public Airport(IInfoStation infoStation)
        {
            this.infoStation = infoStation;
            this.airportInfo = infoStation.GetAirportInfo();
            this.metars = new Dictionary<DateTime, WrappedMetar>();
            this.tafs = new Dictionary<DateTime, WrappedTaf>()
        }
    }
}
