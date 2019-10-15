
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public enum Types
    {
        Weapon,
        Gun,
        Shield,
        Powerplant,
        Helmet,
        Torso,
        Arms,
        Legs,
    }

    public enum ItemGrades
    {
        A,
        B,
        C,
        D,
    }

    public enum ItemClassifications
    {
        Military,
        Civilian,
        Stealth,
        Industrial,
        Competition,
    }

    public class Item
    {
        [ColumnData("Id", 0, sort: false, visible: false)]
        public string Id { get; set; }
        private string _name { get; set; }
        [ColumnData("Name", 1, false)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = Regex.Replace(value, @"[^\u0000-\u007F]+", string.Empty);
            }
        }
        [ColumnData("Size", 2, false)]
        public int Size { get; set; }
        public string Filename { get; set; }

        public Types Type { get; set; }
        public ItemGrades Grade { get; set; }
        public ItemClassifications Classification { get; set; }

        public Color Color { get; set; }

        public Series GetNewLineGraphSeries(string OverrideName = null)
        {
            Series s = new Series(OverrideName ?? Name);
            s.ChartType = SeriesChartType.StepLine;
            s.BorderWidth = 4;
            s.MarkerStyle = MarkerStyle.Circle;
            s.IsValueShownAsLabel = false;
            // I tried to make the colors deterministic by item so that reading the chart is easier.
            // Unfortunately, at time of writing there are 85 distinct ship weapons alone, assigning them all a unique color is basically impossible.
            // So this is commented out until I somehow invent new colors
            //s.Color = Color; 
            return s;
        }

        public Series GetNewRadarGraphSeries(string OverrideName = null)
        {
            Series s = new Series(OverrideName ?? Name);
            s.ChartType = SeriesChartType.Radar;
            s.CustomProperties = "AreaDrawingStyle=Polygon";
            s.BorderWidth = 5;
            s.Color = Color.Transparent;
            s.MarkerStyle = MarkerStyle.Circle;
            s.MarkerSize = 5;
            s.IsValueShownAsLabel = true;
            s.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            return s;
        }

        protected static string _filePath { get; set; }
        public static void SetPath(string path) { _filePath = path; }
    }
}
