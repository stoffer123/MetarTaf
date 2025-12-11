namespace GUI.NewFolder
{
    public class AirportReport
    {
        public string AirportName { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty; // "METAR" eller "TAF"
        public DateTime ReportTime { get; set; } //When the report was issued
        public DateTime FetchTime { get; set; } //When the report was fetched
        public string RawReport { get; set; } = string.Empty;

    }
}
