using Domain.Reports;
using Metar.Decoder;
using Metar.Decoder.Entity;
using MetarTaf_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Factories
{
    internal class MetarFactory
    {
        public MetarFactory() { }

        public MetarReport createMetar(string metarString)
        {
            DecodedMetar decodedMetar = MetarDecoder.ParseWithMode(metarString);

            MetarReport metarReport = new(decodedMetar);

            return metarReport;
        }
    }
}
