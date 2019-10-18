using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    class Utility
    {
        public static Color AssignColors(int i, int count, int alpha)
        {
            int increment = count == 1 ? 255 : 255 / (count - 1);
            return ColorFromHSV(i * increment, 1, 1, alpha);
            //Series.OrderBy(x => x.Name);
            //int l = Series.Count;
            //
            //for (int i = 0; i < count; i++)
            //{
            //    //Series[i].Color = ColorFromHSV(i * increment, 1, 1);
            //    Series[i].BorderColor = ColorFromHSV(i * increment, 1, 1, 125);
            //    Series[i].LabelForeColor = ColorFromHSV(i * increment, 1, 1, 255);
            //    Series[i].MarkerColor = ColorFromHSV(i * increment, 1, 1, 255);
            //}
        }

        public static Color ColorFromHSV(double hue, double saturation, double value, int alpha)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(alpha, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(alpha, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(alpha, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(alpha, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(alpha, t, p, v);
            else
                return Color.FromArgb(alpha, v, p, q);
        }

        private static Color GetColor(string Id)
        {
            var splitId = Id.Split('-');
            return Color.FromArgb(CalcColorValueFromString(splitId[1]), CalcColorValueFromString(splitId[2]), CalcColorValueFromString(splitId[3]));
        }

        private static int CalcColorValueFromString(string Thing)
        {
            int x = 0;
            foreach (char c in Thing.ToCharArray())
            {
                x += (Convert.ToInt32(c) - 60) * 4;
            }
            return x > 255 ? 255 : x < 0 ? 0 : x;
        }

        public static object GetValue(object obj, string field)
        {
            string[] path = field.Split('.');
            if (path.Count() > 1)
            {
                var parentProp = obj.GetType().GetProperty(path[0])?.GetValue(obj);
                if (parentProp != null)
                    return parentProp.GetType().GetProperty(path[1]).GetValue(parentProp);
                else
                    return null;
            }
            else
            {
                return obj.GetType().GetProperty(field).GetValue(obj);
            }
        }
    }
}
