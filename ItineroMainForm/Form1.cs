using BenchmarkDotNet.Running;
using Itinero;
using Itinero.LocalGeo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ItineroMainForm
{
    public partial class Form1 : Form
    {
        private string LAST_FILENAME_FILE = "lastfile.txt";
        private string lastFilename = null;
        private string fileContent = null;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            LoadFile();
        }

        private void LoadFile()
        {
            if (!File.Exists(LAST_FILENAME_FILE))
                return;

            lastFilename = File.ReadAllText(LAST_FILENAME_FILE);
            label1.Text = new FileInfo(lastFilename).FullName;
            fileContent = File.ReadAllText(lastFilename);

            textBox2.Text = "";
            var lines = File.ReadAllLines(lastFilename);
            var text = "";
            foreach (var line in lines)
            {
                text += line + Environment.NewLine;
            }
            textBox2.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                Invoke(new Action(() =>
                {
                    textBox1.Text = "Starting RouterCache init" + Environment.NewLine;
                }));
                RouterCache.Init();

                Invoke(new Action(() =>
                {
                    textBox1.Text += "RouterCache init done";
                    button2.Enabled = true;
                }));
            });
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var profile = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();
            // Init
            List<RouterPoint> PossibleCoordinates = new List<RouterPoint>();
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            int failCount = 0;
            foreach (string line in textBox2.Lines)
            {
                try
                {
                    var pairs = line.Split(',');
                    var p1s = pairs.First();
                    var p2s = pairs.Last();

                    float p1 = float.Parse(p1s, CultureInfo.InvariantCulture);
                    float p2 = float.Parse(p2s, CultureInfo.InvariantCulture);
                    try
                    {
                        var a = RouterCache.GetRouter().Resolve(profile, p1, p2, 200);
                        PossibleCoordinates.Add(a);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($" https://maps.google.com/?q={line}");
                        failCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    failCount++;
                }
            };

            RouterCache.GetRouter().Calculate(profile, PossibleCoordinates.First(), PossibleCoordinates.Last()); //init
            
            Random random = new Random(DateTime.Now.Millisecond);
            Stopwatch sw = new Stopwatch();

            int count = 1000;
            long totalElapsed = 0;
            long highest = 0;
            long lowest = int.MaxValue;
             var weight = RouterCache.GetRouter().GetDefaultWeightHandler(profile);

            Stopwatch total = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                var c1 = random.Next(0, PossibleCoordinates.Count());
                var c2 = random.Next(0, PossibleCoordinates.Count());
                var from = PossibleCoordinates.ElementAt(c1);
                var to = PossibleCoordinates.ElementAt(c2);

                try
                {
                    sw.Start();
                    // var something = RouterCache.GetRouter().TryCalculateRaw(profile, weight, from, to);

                    var route = RouterCache.GetRouter().Calculate(profile, from, to);
                    sw.Stop();
                    totalElapsed += sw.ElapsedMilliseconds;
                    if (sw.ElapsedMilliseconds > highest)
                        highest = sw.ElapsedMilliseconds;
                    if (sw.ElapsedMilliseconds < lowest)
                        lowest = sw.ElapsedMilliseconds;
                    sw.Reset();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " | " + from  + " | " + to + " | " + $" https://www.google.com/maps/dir/{from.Latitude.ToInvariantString()},{from.Longitude.ToInvariantString()}/{to.Latitude.ToInvariantString()},{to.Longitude.ToInvariantString()}");
                    sw.Reset();
                }
            }
            total.Stop();
            double avg = totalElapsed / count;
            List<string> asd = new List<string>();

            asd.Add("RESULTS");
            asd.Add($"Avg:\t{avg}");
            asd.Add($"Highest:\t{highest}");
            asd.Add($"Lowest:\t{lowest}");
            asd.Add($"Total: {total.ElapsedMilliseconds}");

            textBox1.Text = string.Join(Environment.NewLine, asd);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.csv";
            openFileDialog.ShowDialog();

            if (!File.Exists(openFileDialog.FileName))
            {
                MessageBox.Show("File does not exist)");
                return;
            }

            File.WriteAllText(LAST_FILENAME_FILE, openFileDialog.FileName);
            LoadFile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadFile();
        }
    }
}
