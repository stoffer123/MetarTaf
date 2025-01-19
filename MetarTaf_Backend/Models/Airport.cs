using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            updateTafs();
            
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
            Dictionary<DateTime, TafReport> newTafs = infoStation.getTafs(icao);

            foreach (KeyValuePair<DateTime, TafReport> kvp in newTafs)
            {
                TafReport newTaf = kvp.Value;

                if (tafs.TryAdd(kvp.Key, newTaf))
                {
                    tafIsNew = true;
                }
            }
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

        public AirportInfo getAirportInfo()
        {
            return airportInfo;
        }

        public Dictionary<DateTime, MetarReport> getMetars()
        {
            return metars;
        }

        public Dictionary<DateTime, TafReport> getTafs()
        {
            return tafs;
        }

        public void setMetarIsNew(bool isNew)
        {
            metarIsNew = isNew;
        }

        public void setTafIsNew(bool isNew)
        {
            tafIsNew = isNew;
        }

        public bool getIsNewMetar()
        {
            return metarIsNew;
        }

        public bool getIsNewTaf()
        {
            return tafIsNew;
        }
    }
}
