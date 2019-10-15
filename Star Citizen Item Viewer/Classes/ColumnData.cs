using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Star_Citizen_Item_Viewer.Classes
{
    class ColumnData : Attribute
    {
        public string DisplayName { get; set; }
        public int Priority { get; set; }
        public bool Sort { get; set; }
        public bool SortDescending { get; set; }
        public string Format { get; set; }
        public bool Visible { get; set; }

        public ColumnData(string displayName, int priority, bool sort = true, bool descending = true, string format = null, bool visible = true)
        {
            DisplayName = displayName;
            Priority = priority;
            Sort = sort;
            Format = format;
            SortDescending = descending;
            Visible = visible;
        }
    }

    public class RadarField : Attribute { }
}
