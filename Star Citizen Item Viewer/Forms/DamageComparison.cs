using Star_Citizen_Item_Viewer.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer
{
    enum Firemode
    {
        All = 1,
        Single = 2,
        Burst = 3,
        Rapid = 4
    }
    
    public partial class DamageComparison : Form
    {
        private static CancellationTokenSource Source = new CancellationTokenSource();
        private static ConcurrentQueue<List<Series>> DrawQueue = new ConcurrentQueue<List<Series>>();

        private static List<object> Data;

        private bool LabelsDisplayed = false;
        private int FiremodeDisplay { get; set; }

        private FormWriter Writer { get; set; }

        public DamageComparison(List<object> data, FormWriter writer)
        {
            InitializeComponent();
            FiremodeDisplay = (int)Firemode.All;

            Item i = (Item)data.FirstOrDefault();
            if (i != null && i.Type == Types.Gun)
            {
                ShowLabels.Dock = DockStyle.Top;
                cycleFiremode.Dock = DockStyle.Bottom;
                cycleFiremode.Enabled = true;
                cycleFiremode.Visible = true;
            }
            else
            {
                ShowLabels.Dock = DockStyle.Fill;
                cycleFiremode.Dock = DockStyle.None;
                cycleFiremode.Enabled = false;
                cycleFiremode.Visible = false;
            }

            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            Writer = writer;
            Data = data;
            DrawGraph(trackBar1.Value * 10, Source.Token);
            chart1.ChartAreas[0].AxisX.Minimum = 0;
        }

        

        private void DrawGraph(int Ticks, CancellationToken Token)
        {
            List<Series> series = Writer.CreateLineGraphSeries(Data, Ticks, Token).Where(x =>
            {
                switch (FiremodeDisplay)
                {
                    case (int)Firemode.All: return true;
                    case (int)Firemode.Single: return x.Name.EndsWith(" Single");
                    case (int)Firemode.Burst: return x.Name.EndsWith(" Burst");
                    case (int)Firemode.Rapid: return x.Name.EndsWith(" Rapid");
                    default: return true;
                }
            }).ToList();

            List<Series> killLines = Writer.DrawKillLines(Ticks);

            MethodInvoker d = delegate ()
            {
                chart1.Series.Clear();
                int x = series.Count;
                for (int i = 0; i < x; i++)
                {
                    series[i].Color = Utility.AssignColors(i, x, 255);
                    chart1.Series.Add(series[i]);
                }
                foreach (var s in killLines)
                {
                    chart1.Series.Add(s);
                }
            };
            if (this.InvokeRequired)
                this.BeginInvoke(d);
            else
                d();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Source.Cancel();
            LabelsDisplayed = false;
            Source = new CancellationTokenSource();
            int value = trackBar1.Value * 10;
            Task.Run(() => DrawGraph(value, Source.Token), Source.Token);
        }

        private void ShowLabels_Click(object sender, EventArgs e)
        {
            LabelsDisplayed = !LabelsDisplayed;
            foreach (var series in chart1.Series)
            {
                series.IsValueShownAsLabel = LabelsDisplayed;
            }
        }

        private void cycleFiremode_Click(object sender, EventArgs e)
        {
            FiremodeDisplay = (FiremodeDisplay % 4)+1;
            cycleFiremode.Text = Enum.GetName(typeof(Firemode), FiremodeDisplay);
            Source.Cancel();
            Source = new CancellationTokenSource();
            int value = trackBar1.Value * 10;
            Task.Run(() => DrawGraph(value, Source.Token), Source.Token);
        }
    }
}
