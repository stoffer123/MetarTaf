using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Reports
{
    public abstract class Report
    {
        public DateTime reportTime { get; protected set; }
        public DateTime fetchTime { get; protected set; }
        public TimeSpan reportClock { get; protected set; }
        public abstract string typeString { get; protected set; }

        protected DateTime createReportTime(int day, string time)
        {

            string dateTimeString = (day <= 9 ? "0" + day : day ) + time;
            string format = "ddHH:mm 'UTC'";
            CultureInfo culture = CultureInfo.InvariantCulture;

            if (!DateTime.TryParseExact(dateTimeString, format, culture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime parsedDateTime))
            {
                Console.WriteLine($"Failed to parse the date time string. Day: {day} time: {time}");
            }



            return parsedDateTime;
        }

        protected TimeSpan CreateReportTimeSpan(int day, string time)
        {
            // Fjern evt. Z og kolon
            var clean = time.Trim().TrimEnd('Z');
            if (clean.Contains(":") == false && clean.Length == 4)
                clean = clean.Insert(2, ":"); // fx "1230" -> "12:30"

            if (TimeSpan.TryParseExact(clean, "hh\\:mm", CultureInfo.InvariantCulture, out var ts))
                return ts;

            return TimeSpan.Zero;
        }

    }
}
