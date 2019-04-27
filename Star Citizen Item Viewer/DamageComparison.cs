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

        private static List<object> DataSeries;
        public DamageComparison(List<object> Data)
        {
            InitializeComponent();
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            DataSeries = Data;
            DrawGraph(5, Source.Token);
            chart1.ChartAreas[0].AxisX.Minimum = 0;
        }

        private void DrawGraph(int TotalTicks, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 5, CancellationToken = Token };
                Parallel.ForEach(DataSeries, options, (item, loopState) =>
                {
                    string name = item.Get("Name").ToString();
                    Series s = new Series(name);
                    s.ChartType = SeriesChartType.Line;
                    s.BorderWidth = 2;
                    s.MarkerStyle = MarkerStyle.Circle;
                    s.IsValueShownAsLabel = TotalTicks <= 100;

                    decimal[] x = new decimal[TotalTicks + 1];
                    int[] y = new int[TotalTicks + 1];
                    int damage = Convert.ToInt32(item.Get("DamageTotal"));
                    decimal firerate = Convert.ToDecimal(item.Get("Firerate"));
                    for (int i = 0; i <= TotalTicks; i++)
                    {
                        x[i] = (decimal)(i * .1);
                        y[i] = damage + (int)(damage * Math.Floor(i / firerate));
                        if (options.CancellationToken.IsCancellationRequested)
                            break;
                    }

                    if (options.CancellationToken.IsCancellationRequested)
                        loopState.Break();

                    s.Points.DataBindXY(x, y);
                    list.Enqueue(s);

                });
                MethodInvoker d = delegate ()
                {
                    chart1.Series.Clear();
                    Series s;
                    while (list.TryDequeue(out s))
                    {
                        chart1.Series.Add(s);
                    }
                };
                if (this.InvokeRequired)
                    this.BeginInvoke(d);
                else
                    d();
            }
            catch (OperationCanceledException){}
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Source.Cancel();
            Source = new CancellationTokenSource();
            int value = trackBar1.Value * 10;
            Task.Run(() => DrawGraph(value, Source.Token), Source.Token);
        }
    }
}
