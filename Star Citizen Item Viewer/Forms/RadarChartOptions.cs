using Star_Citizen_Item_Viewer.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Star_Citizen_Item_Viewer.Forms
{
    public partial class RadarChartOptions : Form
    {
        private static FormWriter Writer { get; set; }
        private static Dictionary<string, object> Data { get; set; }
        private static CancellationTokenSource Source = new CancellationTokenSource();

        public RadarChartOptions(Dictionary<string, object> data, FormWriter writer)
        {
            InitializeComponent();
            Writer = writer;
            Data = data;
            compareValues.Nodes.AddRange(writer.BuildTree(data.Values.ToArray()));
            aggregateValues.Nodes.AddRange(writer.BuildTree(data.Values.ToArray()));
        }

        private void submit_Click(object sender, EventArgs e)
        {
            Writer.ClearValues();
            List<object> compareComponents = new List<object>();
            foreach (TreeNode rootNode in compareValues.Nodes)
            {
                GetSelected(rootNode, compareComponents);
            }
            foreach (TreeNode rootNode in aggregateValues.Nodes)
            {
                GetSelected(rootNode);
            }

            Writer.CalculateStandardDeviations();
            var series = Writer.CreateRadarGraphSeries(compareComponents, Source.Token);
            Task.Run(() =>
            {
                RadarChart view = new RadarChart(series, Writer.RadarLabels(), false);
                view.ShowDialog();
            });
        }

        private void GetSelected(TreeNode node, List<object> selected)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    GetSelected(n, selected);
                }
            }
            else if (node.Checked)
            {
                Writer.TrackValues(Data[node.Name], true);
                selected.Add(Data[node.Name]);
            }
        }

        private void GetSelected(TreeNode node)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode n in node.Nodes)
                {
                    GetSelected(n);
                }
            }
            else if (node.Checked)
            {
                Writer.TrackValues(Data[node.Name], false);
            }
        }

        private void compareValues_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = e.Node.Checked;
            }
        }

        private void aggregateValues_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = e.Node.Checked;
            }
        }
    }
}
