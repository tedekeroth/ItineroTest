using BenchmarkDotNet.Running;
using Itinero;
using Itinero.LocalGeo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ItineroMainForm
{
    public partial class Form1 : Form
    {
        public IEnumerable<Coordinate> PossibleCoordinates => new[] { new Coordinate(55.689458f, 13.212871f), new Coordinate(55.712343f, 13.182219f), new Coordinate(55.717278f, 13.197442f), new Coordinate(55.666078f, 13.347477f), new Coordinate(55.634178f, 13.497319f) };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                RouterCache.Init();
                Debug.WriteLine("Init done");
                Invoke(new Action(() => { button2.Enabled = true; }));
            });
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Init
            RouterCache.GetRouter().Calculate(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), PossibleCoordinates.First(), PossibleCoordinates.Last());
            Random random = new Random(DateTime.Now.Millisecond);
            Stopwatch sw = new Stopwatch();

            int count = 1000;
            long totalElapsed = 0;
            long highest = 0;
            long lowest = int.MaxValue;
            for (int i = 0; i < count; i++)
            {
                var from = PossibleCoordinates.ElementAt(random.Next(0, PossibleCoordinates.Count()));
                var to = PossibleCoordinates.ElementAt(random.Next(0, PossibleCoordinates.Count()));

                sw.Start();
                var route = RouterCache.GetRouter().Calculate(Itinero.Osm.Vehicles.Vehicle.Car.Fastest(), from, to);
                sw.Stop();
                totalElapsed += sw.ElapsedMilliseconds;
                if (sw.ElapsedMilliseconds > highest)
                    highest = sw.ElapsedMilliseconds;
                if (sw.ElapsedMilliseconds < lowest)
                    lowest = sw.ElapsedMilliseconds;
                sw.Restart();
            }
            double avg = totalElapsed / count;
            List<string> asd = new List<string>();
            asd.Add("RESULTS");
            asd.Add("RESULTS");
            asd.Add($"Avg:\t{avg}");
            asd.Add($"Highest:\t{highest}");
            asd.Add($"Lowest:\t{lowest}");

            textBox1.Text = string.Join(Environment.NewLine, asd);
        }
    }
}
