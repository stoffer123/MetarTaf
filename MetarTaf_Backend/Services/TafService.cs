using Domain.Factories;
using Domain.Ports;
using Domain.Reports;

namespace MetarTaf_Backend.Services
{
    internal class TafService
    {
        private readonly Dictionary<string, Dictionary<DateTime, TafReport>> tafs = new();
        private readonly TafFactory tafFactory = new();
        private readonly IInfoStation infoStation;
        private readonly IOpmetFetcher fetcher;

        public TafService(IInfoStation infostation, IOpmetFetcher fetcher)
        {
            this.infoStation = infostation;
            this.fetcher = fetcher;
        }

        public async Task FetchTafs()
        {
            var icaoList = infoStation.GetObserverIcaos().ToArray();
            if (icaoList.Length == 0)
            {
                infoStation.notifyTafChange();
                return;
            }

            try
            {
                var (_, tafMap) = await fetcher.GetLatestPerIcaoRawAsync(icaoList, windValidTime: 0);

                foreach (var kv in tafMap)
                {
                    // fx "TAF EKCH 091714Z 0918/1018 ..."
                    var tafLine = kv.Value;
                    
                    tafLine = tafLine.Replace(" RTD ", " "); //RTD er ikke understøtet af decoderen, der arbejdes på det

                    try
                    {
                        var taf = tafFactory.createTaf(tafLine.StartsWith("TAF ") ?
                            tafLine : "TAF " + tafLine);

                        var icao = taf.decodedTaf.Icao;
                        var reportTime = taf.reportTime;

                        if (!tafs.ContainsKey(icao))
                        {
                            tafs[icao] = new Dictionary<DateTime, TafReport>();
                        }

                        if (!tafs[icao].ContainsKey(reportTime))
                        {
                            tafs[icao][reportTime] = taf;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing TAF for {kv.Key}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"fetchTafs failed: {ex.Message}");
            }

            infoStation.notifyTafChange();
        }

        public Dictionary<DateTime, TafReport> getTafs(string icao)
        {
            return tafs.TryGetValue(icao, out var dict) ?
                dict : new Dictionary<DateTime, TafReport>();
        }
    }
}
