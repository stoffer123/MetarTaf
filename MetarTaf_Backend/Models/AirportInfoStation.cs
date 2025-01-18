using MetarTaf_Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    internal class AirportInfoStation : IInfoStation
    {
        List<IAirport> observers;
        MetarService metarService = new();
        
        public void addObserver(IAirport observer)
        {
            observers.Add(observer);
        }
        public void removeObserver(IAirport observer)
        {
            observers.RemoveAll(observer);
        }

        public void notifyAirportInfoChange()
        {
            foreach (IAirport observer in observers)
            {
                observer.updateAirportInfo();
            }
        }

        public void notifyMetarChange()
        {
            throw new NotImplementedException();
        }

        public void notifyTafChange()
        {
            throw new NotImplementedException();
        }

    }
}
