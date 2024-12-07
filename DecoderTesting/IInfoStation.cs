using Metar.Decoder.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taf.Decoder.entity;

namespace DecoderTesting
{
    public interface IInfoStation
    {
        void addAirport(Airport airport);
        void removeAirport(Airport airport);
        void notify();
        DecodedMetar getMetar(Airport airport);
        DecodedTaf getTaf(Airport airport);

        Task loadMetar();

        Task loadTaf();

        Task loadReports();

    }
}
