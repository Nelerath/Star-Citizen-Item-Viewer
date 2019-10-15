﻿using System;
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
            Fields = new List<FieldInfo>();
            foreach (var property in _type.GetProperties())
            {
                GetFieldsRecursive(property, string.Empty);
            }
            Fields = Fields.OrderBy(x => x.Priority).ToList();
        }

        protected virtual void GetFieldsRecursive(PropertyInfo property, string path)
        {
            if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(string))
            {
                if (Attribute.GetCustomAttribute(property, typeof(RadarField)) != null)
                {
                    ColumnData columnData = (ColumnData)Attribute.GetCustomAttribute(property, typeof(ColumnData));
                    if (columnData != null)
                    {
                        Fields.Add(new FieldInfo
                        {
                            DisplayFieldName = $"{path}{columnData.DisplayName}",
                            DataFieldName = $"{path.Replace(' ','.')}{property.Name}",
                            SortDescending = columnData.SortDescending,
                            Priority = columnData.Priority
                        });
                    }
                }
            }
            else if (property.PropertyType.ToString().Split('.')[0] != "System")
            {
                foreach (var prop in property.PropertyType.GetProperties())
                {
                    GetFieldsRecursive(prop, $"{path}{property.Name} ");
                }
            }
        }

        public virtual Column[] GetColumns()
        {
            List<Column> columns = new List<Column>();

            foreach (var property in _type.GetProperties())
            {
                columns.AddRange(GetColumnsRecursive(property, string.Empty));
            }

            columns.Add(new Column("Score", null, true, true, "N2", false, 101));
            return columns.OrderBy(x => x.Priority).ToArray();
        }

        protected virtual Column[] GetColumnsRecursive(PropertyInfo property, string path)
        {
            List<Column> columns = new List<Column>();
            if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(string))
            {
                ColumnData data = (ColumnData)Attribute.GetCustomAttribute(property, typeof(ColumnData));
                if (data != null)
                {
                    columns.Add(new Column($"{path}{data.DisplayName}", $"{path.Replace(' ', '.')}{property.Name}", data.Sort, data.SortDescending, data.Format, data.Visible, data.Priority));
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
                    foreach (var field in Fields)
                    {
                        s.Points.Add(new DataPoint(0, GetRank(field.DataFieldName, Convert.ToDouble(Utility.GetValue(item, field.DataFieldName)), field.SortDescending)));
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
            foreach (var field in Fields)
            {
                CustomLabel label = new CustomLabel();
                label.ForeColor = System.Drawing.Color.White;
                label.Text = field.DisplayFieldName;
                output.Add(label);
            }
            return output;
        }

        public virtual List<string[]> GetDownloadInfo()
        {
            return new List<string[]>();
        }
    }
}
