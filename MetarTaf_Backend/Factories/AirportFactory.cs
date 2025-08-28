using Domain.Ports;
using Domain.ValueObjects;
using Domain.Entities;
using Domain.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetarTaf_Backend.Services;

namespace MetarTaf_Backend.Factories
{
    public class AirportFactory
    {
        IInfoStation infoStation;
        
        public AirportFactory(IInfoStation infoStation) 
        {
            this.infoStation = infoStation;
        }


        public IAirport createAirport(string icao)
        {
            AirportInfo airportInfo = AirportInfoService.getAirportInfo(icao);
            if (airportInfo == null)
            {
                return null;
            }

            IAirport airport = new Airport(infoStation, airportInfo);
            
            return airport;
        }
    }
}
