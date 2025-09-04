using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taf.Decoder.entity;

namespace Domain.Reports
{
    public class TafReport : Report
    {
        public DecodedTaf decodedTaf { get; set; }
        public override string typeString { get; protected set; }
        public TafReport(DecodedTaf decodedTaf)
        {
            this.decodedTaf = decodedTaf;
            this.typeString = setTypeString();
            base.reportTime = base.createReportTime(decodedTaf.Day.Value, decodedTaf.Time);
            base.fetchTime = DateTime.UtcNow;
        }

        private string setTypeString()
        {
            switch(decodedTaf.Type)
            {
                case DecodedTaf.TafType.NULL:
                    return "NULL";
                case DecodedTaf.TafType.TAF:
                    return "TAF";
                case DecodedTaf.TafType.TAFAMD:
                    return "TAF AMD";
                case DecodedTaf.TafType.TAFCOR:
                    return "TAF COR";
                default:
                    return "TAF";
            }
        }

    }
}
