using Metar.Decoder.Entity;
using Taf.Decoder.entity;


namespace DecoderTesting
{
    public class Airport
    {
        public string icaoId { get; set; }
        public string iataId { get; set; }
        public string faaId { get; set; }
        public string wmoId { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public int elev { get; set; }
        public string site { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public int priority { get; set; }

        public IInfoStation infoStation { get; set; }

        // Dictionaries for METARs and TAFs
        public Dictionary<DateTime, DecodedMetar> metars { get; set; }
        public Dictionary<DateTime, DecodedTaf> tafs { get; set; }

        public Airport()
        {
            metars = new Dictionary<DateTime, DecodedMetar>();
            tafs = new Dictionary<DateTime, DecodedTaf>();
        }

        public void update()
        {
            try
            {
                DecodedMetar newMetar = infoStation.getMetar(this);
                if(newMetar != null)
                {
                    DateTime newMetarDateTime = parseDateTime(newMetar.Day.Value, newMetar.Time);
                    // METAR Check if the key (DateTime) already exists in the dictionary
                    if (!metars.ContainsKey(newMetarDateTime))
                    {
                        // Add to dictionary
                        metars.Add(newMetarDateTime, newMetar);
                        Console.WriteLine($"Added METAR for {newMetarDateTime}: {newMetar.RawMetar}");
                    }
                    else
                    {
                        Console.WriteLine($"METAR for {newMetarDateTime} already exists in the dictionary.");
                    }

                }


                // Get the new METAR and TAF data from the infoStation object
                DecodedTaf newTaf = infoStation.getTaf(this);
                if(newTaf != null)
                {
                    // Parse the date and time into DateTime
                    DateTime newTafDateTime = parseDateTime(newTaf.Day.Value, newTaf.Time);
                    // TAF Check if the key (DateTime) already exists in the dictionary
                    if (!tafs.ContainsKey(newTafDateTime))
                    {
                        // Add to dictionary
                        tafs.Add(newTafDateTime, newTaf);
                        Console.WriteLine($"Added TAF for {newTafDateTime}: {newTaf.RawTaf}");
                    }
                    else
                    {
                        Console.WriteLine($"TAF for {newTafDateTime} already exists in the dictionary.");
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during update: {ex.Message}");
            }
        }



        private DateTime parseDateTime(int date, string time)
        {
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
