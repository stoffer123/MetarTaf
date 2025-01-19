using Metar.Decoder.Chunkdecoder;
using MetarTaf_Backend.Models;
using MetarTaf_Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Factories
{
    internal class AirportFactory
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
