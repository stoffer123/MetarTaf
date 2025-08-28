using Domain.Reports;
using Metar.Decoder;
using Metar.Decoder.Entity;

namespace Domain.Factories
{
    public class MetarFactory
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
