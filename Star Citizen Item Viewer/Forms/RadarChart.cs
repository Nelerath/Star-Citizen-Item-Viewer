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

        public RadarChart(List<Series> series, List<CustomLabel> Labels)
        {
            InitializeComponent();
            Utility.AssignColors(series);
            foreach (var item in Labels)
            {
                Chart.ChartAreas[0].AxisX.CustomLabels.Add(item);
            }

            foreach (var s in series)
            {
                Chart.Series.Add(s);
            }
        }
    }
}
