using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public interface IFormWriter
    {
        Column[] GetColumns();
        TreeNode[] BuildTree(object[] Items);
        List<Series> CreateRadarGraphSeries(List<object> Data, CancellationToken Token);
        List<CustomLabel> RadarLabels();

        void TrackValues(object component);
        double GetRank(string field, double value, bool descending);
        void ClearValues();
    }
}
