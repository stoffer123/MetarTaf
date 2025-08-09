using Metar.Decoder.Entity;
using MetarTaf_Backend.Factories;
using MetarTaf_Backend.Models;

namespace MetarTaf_Backend.Services
{
    internal class MetarService
    {
        private readonly Dictionary<string, Dictionary<DateTime, MetarReport>> metars = new();
        private readonly MetarFactory metarFactory = new();
        private readonly IInfoStation infoStation;
        private readonly AirportController airportController;
        private readonly NorthAviMetFetcher fetcher;

        // NY: injicer fetcher
        public MetarService(IInfoStation infostation, AirportController airportController, NorthAviMetFetcher fetcher)
        {
            this.infoStation = infostation;
            this.airportController = airportController;
            this.fetcher = fetcher;
        }

        public async Task fetchMetars()
        {
            var icaoList = airportController.getAirportIcaoList().ToArray();
            if (icaoList.Length == 0)
            {
                infoStation.notifyMetarChange();
                return;
            }

            try
            {
                var (metarMap, _) = await fetcher.GetLatestPerIcaoAsync(icaoList, windValidTime: 0);

                foreach (var kv in metarMap)
                {
                    // fx "METAR EKCH 091920Z ..."
                    var metarLine = kv.Value;
                    // ryd "AUTO " hvis din parser kræver det
                    metarLine = metarLine.Replace(" AUTO ", " ");

                    try
                    {
                        var metar = metarFactory.createMetar(
                            metarLine.StartsWith("METAR ") ? metarLine : "METAR " + metarLine
                        );

                        var icao = metar.decodedMetar.ICAO;
                        var reportTime = metar.reportTime;

                        if (!metars.ContainsKey(icao))
                            metars[icao] = new Dictionary<DateTime, MetarReport>();

                        if (!metars[icao].ContainsKey(reportTime))
                            metars[icao][reportTime] = metar;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing METAR for {kv.Key}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"fetchMetars failed: {ex.Message}");
            }

            infoStation.notifyMetarChange();
        }

        public Dictionary<DateTime, MetarReport> getMetars(string icao)
        {
            return metars.TryGetValue(icao, out var dict)
                ? dict
                : new Dictionary<DateTime, MetarReport>();
        }
    }
}
