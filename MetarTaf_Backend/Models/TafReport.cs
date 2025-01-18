using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taf.Decoder.entity;

namespace MetarTaf_Backend.Models
{
    internal class TafReport : Report
    {
        public DecodedTaf decodedTaf { get; set; }
        public TafReport(DecodedTaf decodedTaf)
        {
            this.decodedTaf = decodedTaf;
            base.reportTime = base.createReportTime(decodedTaf.Day.Value, decodedTaf.Time);
        }

    }
}
