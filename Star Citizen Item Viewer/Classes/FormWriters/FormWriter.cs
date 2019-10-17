using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public abstract class FormWriter : RankTracker
    {
        private Type _type { get; set; }

        public FormWriter(Type type)
        {
            _type = type;

            List<Column> columns = new List<Column>();
            foreach (var property in _type.GetProperties())
            {
                columns.AddRange(GetColumnsRecursive(property, string.Empty));
            }
            _columns = columns.OrderBy(x => x.Priority).ToArray();
        }

        protected virtual Column[] GetColumnsRecursive(PropertyInfo property, string path)
        {
            List<Column> columns = new List<Column>();
            if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(string))
            {
                ColumnData data = (ColumnData)Attribute.GetCustomAttribute(property, typeof(ColumnData));
                if (data != null)
                {
                    columns.Add(new Column
                    {
                        Name = $"{path}{data.DisplayName}",
                        DataFieldName = $"{path.Replace(' ', '.')}{property.Name}",
                        Sort = data.Sort,
                        SortDescending = data.SortDescending,
                        Format = data.Format,
                        Visible = data.Visible,
                        Priority = data.Priority,

                        RadarField = (RadarField)Attribute.GetCustomAttribute(property, typeof(RadarField)) != null ? true : false
                    });
                }
            }
            else if (property.PropertyType.ToString().Split('.')[0] != "System")
            {
                foreach (var prop in property.PropertyType.GetProperties())
                {
                    columns.AddRange(GetColumnsRecursive(prop, $"{path}{property.Name} "));
                }
            }
            return columns.ToArray();
        }

        public virtual TreeNode[] BuildTree(object[] Items)
        {
            Dictionary<string, List<TreeNode>> tree = new Dictionary<string, List<TreeNode>>();
            foreach (Item item in Items)
            {
                TreeNode n = new TreeNode();
                n.Name = item.Id;
                n.Text = item.Name;

                string key = item.Size.ToString();
                if (tree.ContainsKey(key))
                    tree[key].Add(n);
                else
                    tree.Add(key, new List<TreeNode>() { n });
            }
            List<TreeNode> output = new List<TreeNode>();
            foreach (var key in tree.Keys.OrderBy(x => Convert.ToInt16(x)))
            {
                output.Add(new TreeNode(key, tree[key].OrderBy(x => x.Text).ToArray()));
            }
            return output.ToArray();
        }

        public virtual List<Series> CreateRadarGraphSeries(List<object> Data, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 2, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Item i = (Item)item;
                    Series s = i.GetNewRadarGraphSeries();
                    foreach (var col in Columns)
                    {
                        if (col.RadarField)
                            s.Points.Add(new DataPoint(0, GetRank(col.DataFieldName, Convert.ToDouble(Utility.GetValue(item, col.DataFieldName)), col.SortDescending)));
                    }
                    list.Enqueue(s);
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).ToList();
        }

        public virtual List<CustomLabel> RadarLabels()
        {
            List<CustomLabel> output = new List<CustomLabel>();
            foreach (var col in Columns)
            {
                if (col.RadarField)
                {
                    CustomLabel label = new CustomLabel();
                    label.ForeColor = System.Drawing.Color.White;
                    label.Text = col.Name;
                    output.Add(label);
                }   
            }
            return output;
        }

        public virtual List<string[]> GetDownloadInfo()
        {
            return new List<string[]>();
        }
    }
}
