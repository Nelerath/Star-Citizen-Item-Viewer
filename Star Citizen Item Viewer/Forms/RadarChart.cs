using Star_Citizen_Item_Viewer.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Forms
{
    public partial class RadarChart : Form
    {
        private static CancellationTokenSource Source = new CancellationTokenSource();

        public RadarChart(List<Series> series, List<CustomLabel> labels, bool stdDev)
        {
            InitializeComponent();

            int x = series.Count;
            for (int i = 0; i < x; i++)
            {   
                series[i].BorderColor = Utility.AssignColors(i, x, 125);
                series[i].LabelForeColor = Utility.AssignColors(i, x, 255);
                series[i].MarkerColor = Utility.AssignColors(i, x, 255);
            }

            if (stdDev)
            {
                Chart.ChartAreas[0].AxisY.Minimum = -3;
                Chart.ChartAreas[0].AxisY.Maximum = 3;

                Series average = new Series();
                average.ChartType = SeriesChartType.Radar;
                average.Color = Color.Transparent;
                average.BorderColor = Color.White;
                average.BorderWidth = 2;
                average.IsVisibleInLegend = false;
                average.BorderDashStyle = ChartDashStyle.Dash;
                foreach (var item in labels)
                {
                    average.Points.AddXY(0, 0);
                    Chart.ChartAreas[0].AxisX.CustomLabels.Add(item);
                }
                Chart.Series.Add(average);
            }
            else
            {
                Chart.ChartAreas[0].AxisY.Minimum = 0;
                Chart.ChartAreas[0].AxisY.Maximum = 100;

                foreach (var item in labels)
                {
                    Chart.ChartAreas[0].AxisX.CustomLabels.Add(item);
                }
            }

            foreach (var s in series)
            {
                Chart.Series.Add(s);
            }
        }
    }
}
