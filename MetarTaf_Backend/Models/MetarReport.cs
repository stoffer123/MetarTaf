using Metar.Decoder.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taf.Decoder.entity;

namespace MetarTaf_Backend.Models
{
    public class MetarReport : Report
    {
        public DecodedMetar decodedMetar { get; set; }
        public override string typeString { get; protected set; }
        public MetarReport(DecodedMetar decodedMetar) 
        {
            this.decodedMetar = decodedMetar;
            this.typeString = setTypeString();
            base.reportTime = base.createReportTime(decodedMetar.Day.Value, decodedMetar.Time);
            base.reportClock = base.CreateReportTimeSpan(decodedMetar.Day.Value, decodedMetar.Time);
            base.fetchTime = DateTime.UtcNow;
        }


        private string setTypeString()
        {
            switch (decodedMetar.Type)
            {
                case DecodedMetar.MetarType.NULL:
                    return "NULL";
                case DecodedMetar.MetarType.METAR:
                    return "METAR";
                case DecodedMetar.MetarType.METAR_COR:
                    return "METAR COR";
                case DecodedMetar.MetarType.SPECI:
                    return "SPECI";
                case DecodedMetar.MetarType.SPECI_COR:
                    return "SPECI COR";
                default:
                    return "METAR";
            }
        }
    }
}
