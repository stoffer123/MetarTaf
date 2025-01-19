using Metar.Decoder.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    public class MetarReport : Report
    {
        public DecodedMetar decodedMetar { get; set; }

        public MetarReport(DecodedMetar decodedMetar) 
        {
            this.decodedMetar = decodedMetar;
            base.reportTime = base.createReportTime(decodedMetar.Day.Value, decodedMetar.Time);
        }
    }
}
