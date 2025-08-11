using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Utils
{
        public static class TimeNormalizer
        {
            /// <summary>
            /// Fold reportUtc til nærmeste døgn omkring fetchUtc (±12h) og undgå, at den ender efter fetch/nu.
            /// </summary>
            public static DateTime NormalizeIssueTimeUtc(
                DateTime reportUtc,
                DateTime fetchUtc,
                DateTime nowUtc,
                TimeSpan? tolerance = null)
            {
                // Antag alt er UTC; hvis ikke, force
                reportUtc = DateTime.SpecifyKind(reportUtc, DateTimeKind.Utc);
                fetchUtc = DateTime.SpecifyKind(fetchUtc, DateTimeKind.Utc);
                nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

                // 1) Fold til nærmeste døgn ift. fetch (±12 timer)
                var diff = reportUtc - fetchUtc;
                if (diff.TotalHours > 12)
                {
                    reportUtc = reportUtc.AddDays(-1);
                }
                else if (diff.TotalHours < -12)
                {
                    reportUtc = reportUtc.AddDays(+1);
                }

                // 2) Tolerér at rapporttid kan ligge en smule fremme ift. fetch (fx planlagt 19:20, fetched 19:16)
                var tol = tolerance ?? TimeSpan.FromMinutes(10);
                if (reportUtc - fetchUtc > tol)
                {
                    reportUtc = fetchUtc;
                }

                return reportUtc;
            }
        }
}
