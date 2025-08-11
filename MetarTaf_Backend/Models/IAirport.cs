using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    public interface IAirport
    {
        void updateMetars();
        void updateTafs();
        void updateAirportInfo();

        void incrementReferenceCount();
        void decrementReferenceCount();
        int getReferenceCount();

        AirportInfo getAirportInfo();
        Dictionary<DateTime, MetarReport> getMetars();
        Dictionary<DateTime, TafReport> getTafs();

    }
}
