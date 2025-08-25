using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public enum ReportKind { Metar, Taf }
    public enum ReportModifier { None, AMD, COR, SPECI } // SPECI som modifier til METAR
}
