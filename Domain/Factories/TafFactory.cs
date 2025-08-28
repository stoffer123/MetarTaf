using Domain.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taf.Decoder;
using Taf.Decoder.entity;

namespace Domain.Factories
{
    public class TafFactory
    {
        public TafFactory() { }

        public TafReport createTaf(string tafString)
        {
            DecodedTaf decodedTaf = TafDecoder.ParseWithMode(tafString);

            TafReport tafReport = new(decodedTaf);

            return tafReport;
        }
    }
}
