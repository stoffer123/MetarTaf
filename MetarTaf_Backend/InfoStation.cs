using MetarTaf_Backend.Models;
using System.Text;

namespace MetarTaf_Backend
{
    public class InfoStation : IInfoStation
    {
        Dictionary<string, AirportInfo> airportInfos = new Dictionary<string, AirportInfo>();
        Dictionary<string, Dictionary<DateTime, WrappedMetar>> metars = new Dictionary<string, Dictionary<DateTime, WrappedMetar>>();
        Dictionary<string, Dictionary<DateTime, WrappedTaf>> tafs = new Dictionary<string, Dictionary<DateTime, WrappedTaf>>();

        // Retrieves airport information by ICAO code
        public AirportInfo GetAirportInfo(string icao)
        {
            if (string.IsNullOrEmpty(icao))
            {
                throw new ArgumentException("ICAO code cannot be null or empty.");
            }

            // Attempt to retrieve airport information, return it if found
            if (airportInfos.TryGetValue(icao.ToUpper(), out AirportInfo info))
            {
                return info;
            }

            // If no data is found, throw an exception or handle it as needed
            throw new KeyNotFoundException($"Airport info not found for ICAO code: {icao}");
        }

        // Retrieves METAR data by ICAO code
        public Dictionary<DateTime, WrappedMetar> getMetars(string icao)
        {
            if (string.IsNullOrEmpty(icao))
            {
                throw new ArgumentException("ICAO code cannot be null or empty.");
            }

            // Attempt to retrieve METAR data, return it if found
            if (metars.TryGetValue(icao.ToUpper(), out Dictionary<DateTime, WrappedMetar> metarData))
            {
                return metarData;
            }

            // Return empty dictionary if no METAR data is found
            return new Dictionary<DateTime, WrappedMetar>();
        }

        // Retrieves TAF data by ICAO code
        public Dictionary<DateTime, WrappedTaf> getTafs(string icao)
        {
            if (string.IsNullOrEmpty(icao))
            {
                throw new ArgumentException("ICAO code cannot be null or empty.");
            }

            // Attempt to retrieve TAF data, return it if found
            if (tafs.TryGetValue(icao.ToUpper(), out Dictionary<DateTime, WrappedTaf> tafData))
            {
                return tafData;
            }

            // Return empty dictionary if no TAF data is found
            return new Dictionary<DateTime, WrappedTaf>();
        }

        private void fetchAirportInfo()
        {

  
        }


    }
}
