using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Ports;

namespace Application.Tests.TestDoubles
{
    internal sealed class FakeInfoStation : IInfoStation
    {
        private readonly List<IAirport> _observers = new();
        public bool FetchCalled { get; private set; }

        public void addObserver(Airport observer) => _observers.Add(observer);
        public void removeObserver(IAirport observer) => _observers.Remove(observer);

        public IReadOnlyList<IAirport> GetObservers() => _observers.ToList();

        public Task<bool> FetchNewReportsAsync(CancellationToken ct = default)
        {
            FetchCalled = true;
            return Task.FromResult(true);
        }

        // Hvis du har disse i din IInfoStation:
        public Dictionary<DateTime, MetarReport> getMetars(string icao) => new();
        public Dictionary<DateTime, TafReport> getTafs(string icao) => new();
        public void notifyAirportInfoChange() { }
        public void notifyMetarChange() { }
        public void notifyTafChange() { }
    }
}
