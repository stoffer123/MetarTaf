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

        public event Action? Updated;


        public Airport(IInfoStation infoStation, AirportInfo airportInfo)
        {
            this.infoStation = infoStation;
            this.airportInfo = airportInfo;
            metars = new Dictionary<DateTime, MetarReport>();
            tafs = new Dictionary<DateTime, TafReport>();
            icao = airportInfo.icaoId;
            referenceCount = 1;

            updateMetars();
            updateTafs();
            
        }

        public void updateAirportInfo()
        {
            throw new NotImplementedException();
        }

        public void updateMetars()
        {
            var newMetars = infoStation.getMetars(icao);
            var added = false;
            foreach (var kvp in newMetars)
                if (metars.TryAdd(kvp.Key, kvp.Value))
                    added = true;

            if (added) Updated?.Invoke();
        }

        public void updateTafs()
        {
            var newTafs = infoStation.getTafs(icao);
            var added = false;
            foreach (var kvp in newTafs)
                if (tafs.TryAdd(kvp.Key, kvp.Value))
                    added = true;

            if (added) Updated?.Invoke();
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


    }
}
