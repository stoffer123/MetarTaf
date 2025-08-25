using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public static class TimeService
    {
        public static string FormatSignedHm(TimeSpan ts)
        {
            var sign = ts < TimeSpan.Zero ? "-" : "";
            var d = ts.Duration();
            var hours = (int)Math.Floor(d.TotalHours);
            var minutes = d.Minutes;
            return $"{sign}{hours:D2}:{minutes:D2}";
        }
    }
}
