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

            foreach (var col in Columns)
            {
                col.DataSetContainsField = false;
            }
        }

        public void TrackValues(object component, bool displayedField = false)
        {
            Averages = new ConcurrentDictionary<string, double>();
            StandardDeviations = new ConcurrentDictionary<string, double>();
            foreach (var col in Columns)
            {
                if (col.Sort)
                {
                    object x = Utility.GetValue(component, col.DataFieldName);
                    if (x != null)
                    {
                        double val = Convert.ToDouble(x);
                        if (val == 0 && !col.AllowZeroes) continue;

                        if (Values.ContainsKey(col.DataFieldName))
                            Values[col.DataFieldName].Add(val);
                        else
                            Values.TryAdd(col.DataFieldName, new List<double> { val });

                        CompareValues(col.DataFieldName, val, col.SortDescending, TopValues);
                        CompareValues(col.DataFieldName, val, !col.SortDescending, BottomValues);

                        if (displayedField && val > 0) col.DataSetContainsField = true;
                    }
                }
            }
            _tracked++;
        }

        public void CalculateStandardDeviations()
        {
            foreach (var col in Columns)
            {
                if (col.Sort)
                {
                    double stdDev;
                    double avg;
                    if (CalculateStdDev(col.DataFieldName, out stdDev, out avg))
                    {
                        StandardDeviations.TryAdd(col.DataFieldName, stdDev);
                        Averages.TryAdd(col.DataFieldName, avg);
                    }
                }
            }
        }

        private bool CalculateStdDev(string field, out double stdDev, out double avg)
        {
            stdDev = 0;
            avg = 0;
            if (!Values.ContainsKey(field) || Values[field].Count == 0 || !Values[field].Any(x => x > 0)) return false;

            double average = Values[field].Average();
            stdDev = Math.Pow(Values[field].Select(x => Math.Pow(x - average, 2)).Average(), .5);
            avg = average;
            return true;
        }

        public double GetRank(string field, double value, bool descending)
        {
            if (false && Values.First().Value.Count > 2)
            {
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
                return SystemColors.ControlDarkDark;
            else if (StandardDeviations[field] == 0)
                return Color.FromArgb(255, 255, 255);

            double x = ((value - Averages[field]) / StandardDeviations[field]);
            if (!descending)
                x *= -1;


            int y = (int)Math.Min(((decimal)Math.Abs(x) / 6M) * 255M, 255M);
            return x > 0 ? Color.FromArgb(255 - y, 255, 255 - y) : Color.FromArgb(255, 255 - y, 255 - y);
        }
    }
}
