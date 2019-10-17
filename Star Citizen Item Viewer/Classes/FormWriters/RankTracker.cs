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
        protected Column[] _columns { get; set; }
        public Column[] Columns { get { return _columns; } }

        protected ConcurrentDictionary<string, double> TopValues = new ConcurrentDictionary<string, double>();
        protected ConcurrentDictionary<string, double> BottomValues = new ConcurrentDictionary<string, double>();
        protected ConcurrentDictionary<string, List<double>> Values = new ConcurrentDictionary<string, List<double>>();
        protected ConcurrentDictionary<string, double> StandardDeviations = new ConcurrentDictionary<string, double>();
        protected ConcurrentDictionary<string, double> Averages = new ConcurrentDictionary<string, double>();

        private int _tracked { get; set; }
        public int ComponentsTracked { get { return _tracked; } }

        public void ClearValues()
        {
            TopValues.Clear();
            Values.Clear();
            StandardDeviations.Clear();
            Averages.Clear();
            _tracked = 0;
        }

        public void TrackValues(object component)
        {
            Averages = new ConcurrentDictionary<string, double>();
            StandardDeviations = new ConcurrentDictionary<string, double>();
            foreach (var col in Columns)
            {
                if (col.Sort)
                {
                    double val = Convert.ToDouble(Utility.GetValue(component, col.DataFieldName));
                    if (Values.ContainsKey(col.DataFieldName))
                        Values[col.DataFieldName].Add(val);
                    else
                        Values.TryAdd(col.DataFieldName, new List<double> { val });

                    CompareValues(col.DataFieldName, val, col.SortDescending, TopValues);
                    CompareValues(col.DataFieldName, val, !col.SortDescending, BottomValues);
                }
            }
            _tracked++;
        }

        public void CalculateStdDev(string field)
        {
            double avg = Values[field].Average();
            Averages.TryAdd(field, avg);
            double var = Values[field].Select(x => Math.Pow(x - avg, 2)).Average();
            StandardDeviations.TryAdd(field, Math.Pow(var, .5));
        }

        public double GetRank(string field, double value, bool descending)
        {
            if (Values.First().Value.Count > 2)
            {
                if (!StandardDeviations.ContainsKey(field))
                    CalculateStdDev(field);

                if (StandardDeviations[field] == 0)
                    return 0;

                double x = ((value - Averages[field]) / StandardDeviations[field]);
                if (!descending)
                    x *= -1;

                return Math.Round(x, 2);
            }
            else
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
                        return Math.Floor((1 - (value / BottomValues[field])) * 100);
                }
            }
        }

        private void CompareValues(string field, double value, bool descending, ConcurrentDictionary<string, double> dict)
        {
            if (dict.ContainsKey(field))
                dict[field] = descending ? Math.Max(dict[field], value) : Math.Min(dict[field], value);
            else
                dict[field] = value;
        }

        public Color GetColor(string field, double value, bool descending)
        {
            if (!StandardDeviations.ContainsKey(field))
                CalculateStdDev(field);

            if (StandardDeviations[field] == 0)
                return Color.FromArgb(255, 255, 255);

            double x = ((value - Averages[field]) / StandardDeviations[field]);
            if (!descending)
                x *= -1;


            int y = (int)Math.Min(((decimal)Math.Abs(x) / 6M) * 255M, 255M);
            return x > 0 ? Color.FromArgb(255 - y, 255, 255 - y) : Color.FromArgb(255, 255 - y, 255 - y);
            int red = x < 0 ? (int)Math.Min(((decimal)Math.Abs(x) / 6M) * 55M, 55M) : 0;
            int green = x > 0 ? (int)Math.Min(((decimal)Math.Abs(x) / 6M) * 55M, 55M) : 0;

            return Color.FromArgb(red + 200, green + 200, red+200);
        }
    }
}
