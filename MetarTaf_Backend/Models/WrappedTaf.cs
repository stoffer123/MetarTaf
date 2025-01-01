using Metar.Decoder.Entity;
using Taf.Decoder.entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MetarTaf_Backend.Models
{
    public class WrappedTaf
    {
        DecodedTaf decodedTaf { get; }
        DateTime issueTime { get; }

        public WrappedTaf(DecodedTaf decodedTaf)
        {
            this.decodedTaf = decodedTaf;
            issueTime = parseIssueTime(decodedTaf);
        }

        public DateTime parseIssueTime(DecodedTaf decodedTaf)
        {
            int date = decodedTaf.Day.Value;
            string time = decodedTaf.Time;
            try
            {
                //Escape the nullable value
                // Remove 'UTC' from the time string
                string cleanedTime = time.Replace("UTC", "").Trim();

                // Parse the time string into a TimeSpan
                TimeSpan parsedTime = TimeSpan.Parse(cleanedTime);

                // Get the current UTC year and month
                int year = DateTime.UtcNow.Year;
                int month = DateTime.UtcNow.Month;

                // Parse the date and time into a DateTime object
                DateTime parsedDateTime = new DateTime(year, month, date, parsedTime.Hours, parsedTime.Minutes, 0, DateTimeKind.Utc);

                return parsedDateTime;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse date/time: {ex.Message}");
                return DateTime.MinValue;
            }
        }
    }
}
