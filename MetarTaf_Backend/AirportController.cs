using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend
{
    public class AirportController
    {
        private Dictionary<string, IAirport> airports = new();
        private IInfoStation infoStation;
        private AirportFactory airportFactory;
        private readonly object lockObject;


        public AirportController()
        {
            infoStation = new AirportInfoStation(this);
            airportFactory = new AirportFactory(infoStation);
            lockObject = new object();
        }

        public IAirport getAirport(string icao)
        {
            lock (lockObject)
            {
                IAirport airport = null;
                if (airports.TryGetValue(icao, out airport))
                {
                    airport.incrementReferenceCount();
                }
                else
                {
                    airport = airportFactory.createAirport(icao);
                }
            return airport;
            }
        }

        public void releaseAirport(string icao)
        {
            lock (lockObject)
            {
                IAirport airport = null;

                if (airports.TryGetValue(icao, out airport))
                {
                    airport.decrementReferenceCount();

                    if (airport.getReferenceCount() < 1)
                    {
                        airports.Remove(icao);
                    }
                }
            }

        }
        

        public List<string> getAirportIcaoList()
        {
            List<string> strings = airports.Keys.ToList();

            ////Test list
            //List<string> strings = new List<string>();
            //strings.Add("EKCH");
            //strings.Add("EKEB");
            return strings;
        }


    }
}
