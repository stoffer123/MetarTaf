using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    internal class Airport : IAirport
    {
        private IInfoStation infoStation;
        public AirportInfo airportInfo { get; }
        public Dictionary<DateTime, MetarReport> metars { get; }
        public Dictionary<DateTime, TafReport> tafs { get; }
        public string icao { get; }
        private int referenceCount;

        public bool metarIsNew { get; set; }
        public bool tafIsNew { get; set; }


        public Airport(IInfoStation infoStation, AirportInfo airportInfo)
        {
            this.infoStation = infoStation;
            this.airportInfo = airportInfo;
            metars = new Dictionary<DateTime, MetarReport>();
            tafs = new Dictionary<DateTime, TafReport>();
            icao = airportInfo.icaoId;
            referenceCount = 1;
            metarIsNew = false;
            tafIsNew = false;

            updateMetars();
            
        }

        public void updateAirportInfo()
        {
            throw new NotImplementedException();
        }
        
        public void updateMetars()
        {
            Dictionary<DateTime, MetarReport> newMetars = infoStation.getMetars(icao);

            foreach (KeyValuePair<DateTime, MetarReport> kvp in newMetars)
            {
                MetarReport newMetar = kvp.Value;

                if(metars.TryAdd(kvp.Key, newMetar))
                {
                    metarIsNew = true;
                }
            }
        }

        public void updateTafs()
        {
            throw new NotImplementedException();
        }

        public void incrementReferenceCount()
        {
            referenceCount++;
        }

        public void decrementReferenceCount()
        {
            referenceCount--;
        }

        public int getReferenceCount()
        {
            return referenceCount;
        }
    }
}
