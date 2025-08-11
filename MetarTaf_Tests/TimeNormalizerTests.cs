using MetarTaf_Backend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MetarTaf_Tests
{
    public class TimeNormalizerTests
    {
        [Fact]
        public void ReportTime_Yesterday1920Z_FetchToday1916Z_NormalizesToToday1920Z()
        {
            // Arrange
            // Parser har sat rapport-tid til I GÅR 19:20Z (forkert dag)
            var reportUtc = new DateTime(2025, 8, 9, 19, 20, 0, DateTimeKind.Utc);
            var fetchUtc = new DateTime(2025, 8, 10, 19, 16, 0, DateTimeKind.Utc);
            var nowUtc = new DateTime(2025, 8, 10, 19, 17, 0, DateTimeKind.Utc);

            // Act
            var normalized = TimeNormalizer.NormalizeIssueTimeUtc(reportUtc, fetchUtc, nowUtc);

            // Assert
            // Forvent at dato foldes til nærmeste døgn ift. fetch => I DAG 19:20Z
            var expected = new DateTime(2025, 8, 10, 19, 20, 0, DateTimeKind.Utc);
            Assert.Equal(expected, normalized);
        }

        [Fact]
        public void ReportTime_Tomorrow0005Z_FetchToday2358Z_NormalizesToTomorrow0005Z_WhenWithinTolerance()
        {
            var reportUtc = new DateTime(2025, 8, 11, 0, 5, 0, DateTimeKind.Utc);
            var fetchUtc = new DateTime(2025, 8, 10, 23, 58, 0, DateTimeKind.Utc);
            var nowUtc = new DateTime(2025, 8, 10, 23, 59, 0, DateTimeKind.Utc);

            var normalized = TimeNormalizer.NormalizeIssueTimeUtc(reportUtc, fetchUtc, nowUtc, tolerance: TimeSpan.FromMinutes(10));

            // 7 min efter fetch < tolerance => behold “i morgen”
            var expected = new DateTime(2025, 8, 11, 0, 5, 0, DateTimeKind.Utc);
            Assert.Equal(expected, normalized);
        }

        [Fact]
        public void ReportTime_AfterFetch_BeyondTolerance_ClipsToFetch()
        {
            // Arrange
            var reportUtc = new DateTime(2025, 8, 10, 19, 40, 0, DateTimeKind.Utc); // 24 min efter fetch
            var fetchUtc = new DateTime(2025, 8, 10, 19, 16, 0, DateTimeKind.Utc);
            var nowUtc = new DateTime(2025, 8, 10, 19, 17, 0, DateTimeKind.Utc);

            // Act: tolerance 10 min → 24 min > 10 → klip til fetch
            var normalized = TimeNormalizer.NormalizeIssueTimeUtc(reportUtc, fetchUtc, nowUtc, tolerance: TimeSpan.FromMinutes(10));

            // Assert
            Assert.Equal(fetchUtc, normalized);
        }

        [Fact]
        public void ReportTime_ExactlyAtTolerance_AfterFetch_IsKept()
        {
            var reportUtc = new DateTime(2025, 8, 10, 19, 26, 0, DateTimeKind.Utc); // 10 min efter fetch
            var fetchUtc = new DateTime(2025, 8, 10, 19, 16, 0, DateTimeKind.Utc);

            var normalized = TimeNormalizer.NormalizeIssueTimeUtc(reportUtc, fetchUtc, nowUtc: fetchUtc, tolerance: TimeSpan.FromMinutes(10));
            Assert.Equal(reportUtc, normalized);
        }
    }

}
