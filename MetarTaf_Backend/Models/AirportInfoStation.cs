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
        MetarService metarService;
        
        public AirportInfoStation()
        {
            observers = new List<IAirport>();
            metarService = new(this);
        }

        public void addObserver(IAirport observer)
        {
            observers.Add(observer);
        }
        public void removeObserver(IAirport observer)
        {
            observers.Remove(observer);
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
            foreach (IAirport observer in observers)
            {
                observer.updateMetars();
            }
        }

        public void notifyTafChange()
        {
            foreach (IAirport observer in observers)
            {
                observer.updateTafs();
            }
        }

        public Dictionary<DateTime, MetarReport> getMetars(string icao)
        {
            Dictionary<DateTime, MetarReport> metars = metarService.getMetars(icao);

            return metars;
        }

    }
}
