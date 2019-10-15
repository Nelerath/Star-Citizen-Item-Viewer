using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public abstract class RankTracker
    {
        protected List<FieldInfo> Fields { get; set; }
        protected ConcurrentDictionary<string, double> TopValues = new ConcurrentDictionary<string, double>();
        protected ConcurrentDictionary<string, double> BottomValues = new ConcurrentDictionary<string, double>();

        public void ClearValues()
        {
            TopValues.Clear();
        }

        public void TrackValues(object component)
        {
            foreach (var fieldInfo in Fields)
            {
                CompareValues(fieldInfo.DataFieldName, Convert.ToDouble(Utility.GetValue(component, fieldInfo.DataFieldName)), fieldInfo.SortDescending, TopValues);
                CompareValues(fieldInfo.DataFieldName, Convert.ToDouble(Utility.GetValue(component, fieldInfo.DataFieldName)), !fieldInfo.SortDescending, BottomValues);
            }
        }

        public double GetRank(string field, double value, bool descending)
        {
            if (descending)
            {
                if (TopValues[field] == 0)
                    return 0;
                else
                    return Math.Floor((value / TopValues[field]) * 100);
            }
            else
            {
                if (BottomValues[field] == 0)
                    return 100;
                else
                    return Math.Floor((1-(value / BottomValues[field])) * 100);

                //if (value == 0)
                //    return 100;
                //else
                //    return Math.Floor((TopValues[field] / value) * 100);
            }
        }

        private void CompareValues(string field, double value, bool descending, ConcurrentDictionary<string, double> dict)
        {
            if (dict.ContainsKey(field))
                dict[field] = descending ? Math.Max(dict[field], value) : Math.Min(dict[field], value);
            else
                dict[field] = value;
        }
    }

    public class FieldInfo
    {
        public string DataFieldName { get; set; }
        public string DisplayFieldName { get; set; }
        public bool SortDescending { get; set; }
        public int Priority { get; set; }
    }
}
