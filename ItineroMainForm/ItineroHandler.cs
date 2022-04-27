using BenchmarkDotNet.Attributes;
using Itinero;
using Itinero.IO.Osm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItineroMainForm
{
    internal class ItineroHandler
    {
        [Benchmark]
        public Route GetRoute(float fromLat, float fromLon, float toLat, float toLon)
        {
            // get a profile.
            var profile = Itinero.Osm.Vehicles.Vehicle.Car.Fastest(); // the default OSM car profile.

            // create a routerpoint from a location.
            // snaps the given location to the nearest routable edge.
            var start = RouterCache.GetRouter().Resolve(profile, fromLat, fromLon);
            var end = RouterCache.GetRouter().Resolve(profile, toLat, toLon);

            // calculate a route.
            return RouterCache.GetRouter().Calculate(profile, start, end);
        }
    }
}
