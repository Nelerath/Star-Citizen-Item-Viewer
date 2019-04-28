using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer
{
    
    public partial class DamageComparison : Form
    {
        private static CancellationTokenSource Source = new CancellationTokenSource();
        private static ConcurrentQueue<List<Series>> DrawQueue = new ConcurrentQueue<List<Series>>();
        private static Func<List<object>, int, CancellationToken, List<Series>> CalculateSeries;

        private static List<object> DataSeries;

        private bool LabelsDisplayed = false;

        public DamageComparison(List<object> Data, Func<List<object>, int, CancellationToken, List<Series>> Calculator)
        {
            InitializeComponent();
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            CalculateSeries = Calculator;
            DataSeries = Data;
            DrawGraph(5, Source.Token);
            chart1.ChartAreas[0].AxisX.Minimum = 0;
        }

        private void DrawGraph(int Ticks, CancellationToken Token)
        {
            List<Series> series = CalculateSeries(DataSeries, Ticks, Token);
            MethodInvoker d = delegate ()
            {
                chart1.Series.Clear();
                foreach (var s in series)
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
    }
}
