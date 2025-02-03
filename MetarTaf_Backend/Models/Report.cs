using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarTaf_Backend.Models
{
    public abstract class Report
    {
        public DateTime reportTime { get; set; }
        public DateTime fetchTime { get; set; }

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
    }
}
