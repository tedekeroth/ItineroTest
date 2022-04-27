using BenchmarkDotNet.Attributes;
using Itinero;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [MemoryDiagnoser]
    public class ItineroBenchmarks
    {
        [ParamsSource(nameof(PossibleCoordinates))]
        public Coordinate From { get; set; }

        [ParamsSource(nameof(PossibleCoordinates))]
        public Coordinate To { get; set; }

        public IEnumerable<Coordinate> PossibleCoordinates => new[] { new Coordinate(55.689458f, 13.212871f), new Coordinate(55.712343f, 13.182219f), new Coordinate(55.717278f, 13.197442f), new Coordinate(55.666078f, 13.347477f), new Coordinate(55.634178f, 13.497319f) };
        

        [Benchmark]
        public Route GetRoute()
        {
            // get a profile.
            var profile = Itinero.Osm.Vehicles.Vehicle.Car.Fastest(); // the default OSM car profile.

            // create a routerpoint from a location.
            // snaps the given location to the nearest routable edge.
            //var start = RouterCache.GetRouter().Resolve(profile, From);
            //var end = RouterCache.GetRouter().Resolve(profile, To);

            // calculate a route.
            return RouterCache.GetRouter().Calculate(profile, From, To);
        }
    }
}
