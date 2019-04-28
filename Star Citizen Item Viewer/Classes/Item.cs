
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public enum Types
    {
        Weapon,
        Gun,
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
        public string Id { get; set; }
        private string _name { get; set; }
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
        public int Size { get; set; }
        public string Filename { get; set; }

        public Types Type { get; set; }
        public ItemGrades Grade { get; set; }
        public ItemClassifications Classification { get; set; }

        public static Series GetNewSeries(string Name)
        {
            Series s = new Series(Name);
            s.ChartType = SeriesChartType.Line;
            s.BorderWidth = 3;
            s.MarkerStyle = MarkerStyle.Circle;
            s.IsValueShownAsLabel = false;
            return s;
        }
    }
}
