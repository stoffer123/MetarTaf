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

        private AirportInfo airportInfo;
        public string icao { get; }
        private Dictionary<DateTime, WrappedMetar> metars;
        private Dictionary<DateTime, WrappedTaf> tafs;
        int referenceCount;

        public Airport(IInfoStation infoStation, string icao)
        {
            this.infoStation = infoStation;
            this.icao = icao;
            this.airportInfo = infoStation.GetAirportInfo(this.icao);
            this.metars = new Dictionary<DateTime, WrappedMetar>();
            this.tafs = new Dictionary<DateTime, WrappedTaf>();
            int referenceCount = 0;
        }

        public void IncrementReferenceCount()
        {
            referenceCount++;
        }

        public void DecrementReferenceCount()
        {
            referenceCount--;
        }

        internal bool IsInUse()
        {
            if(referenceCount <= 0) return false;
            else return true;
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
