using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    public interface IAirport : IDisposable
    {
        event Action? Updated;
        void updateMetars();
        void updateTafs();
        void updateAirportInfo();

        void incrementReferenceCount();
        void decrementReferenceCount();
        int getReferenceCount();

        AirportInfo getAirportInfo();
        IReadOnlyDictionary<DateTime, MetarReport> getMetars();
        IReadOnlyDictionary<DateTime, TafReport> getTafs();
        void Dispose();


    }
}
