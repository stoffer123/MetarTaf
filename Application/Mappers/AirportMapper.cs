using Application.DTO;
using Domain.Ports;
using Domain.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public static class AirportMapper
    {
        public static AirportOverviewDto ToOverviewDto(IAirport airport)
        {
            var latestMetar = airport.getMetars()
                .OrderByDescending(m => m.Key)
                .FirstOrDefault().Value;

            var latestTaf = airport.getTafs()
                .OrderByDescending(t => t.Key)
                .FirstOrDefault().Value;

            return new AirportOverviewDto
            {
                Icao = airport.getAirportInfo().icaoId,
                Country = airport.getAirportInfo().country,
                LatestMetar = latestMetar?.ToDto(),
                LatestTaf = latestTaf?.ToDto()
            };
        }

        public static MetarDTO ToDto(this MetarReport metar)
        {
            return new MetarDTO
            {
                ReportTimeUtc = metar.reportTime,
                FetchTimeUtc = metar.fetchTime,
                Raw = metar.decodedMetar?.RawMetar ?? string.Empty,
                Type = metar.typeString,
                IsNewForUser = false // AckTracker i UI kan stadig styre dette
            };
        }

        public static TafDto ToDto(this TafReport taf)
        {
            return new TafDto
            {
                ReportTimeUtc = taf.reportTime,
                FetchTimeUtc = taf.fetchTime,
                Raw = taf.decodedTaf?.RawTaf ?? string.Empty,
                Type = taf.typeString,
                IsAmd = taf.decodedTaf?.Type == Taf.Decoder.entity.DecodedTaf.TafType.TAFAMD,
                IsNewForUser = false
            };
        }
    }
}
