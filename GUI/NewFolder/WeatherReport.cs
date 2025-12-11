namespace GUI.NewFolder
{
    public class WeatherReport
    {
        public DateTime TimeStamp { get; set; }
        public string RawReport { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty; //METAR, TAF, TAF AMD, SPECI osv
    }
}
