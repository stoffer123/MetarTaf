using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class Airport
    {
        public string Icao { get; }
        public string? Name { get; }
        public string? Country { get; }

        public Airport(string icao, string? name = null, string? country = null)
        {
            if (string.IsNullOrWhiteSpace(icao) || icao.Length != 4)
                throw new ArgumentException("ICAO must be 4 letters", nameof(icao));

            Icao = icao.ToUpperInvariant();
            Name = name;
            Country = country;
        }
    }
}
