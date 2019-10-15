using Star_Citizen_Item_Viewer.Classes;
using Star_Citizen_Item_Viewer.Classes.NewFolder1;
using Star_Citizen_Item_Viewer.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer
{
    public partial class MainSheet : Form
    {
        public static Dictionary<string, object> MasterData = new Dictionary<string, object>();
        private static string LastSelection = "";
        private static string Selected = "";

        private static List<string[]> Downloads { get; set; }

        private static List<object> Data = new List<object>();
        private static Column[] Columns = new Column[0];
        private static Func<List<object>, int, CancellationToken, List<Series>> LineGraphSeriesCreator = Weapon.CreateLineGraphSeries;
        private static List<CustomLabel> RadarLabels = Weapon.RadarLabels();

        private static FormWriter Writer = new WeaponFormWriter(typeof(Weapon));

        public static bool Overclocked = false;
        public static bool Overpowered = false;
        public static bool Hot = false;

        public MainSheet()
        {
            InitializeComponent();
            componentSelect.CheckBoxes = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            progressBar1.Visible = false;
            progressBar1.Maximum = 100;
            Item.SetPath(Directory.GetCurrentDirectory());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Columns = Writer.GetColumns();
            Downloads = Writer.GetDownloadInfo();
            listBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LastSelection = Selected;
            Selected = (string)listBox1.SelectedItem;
            if (Selected != LastSelection)
            {
                label1.Text = Selected;
                Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Step = 100 / Downloads.Count;
            progressBar1.Visible = true;

            bool displayGrid = damageOutput.Enabled;
            damageOutput.Enabled = false;
            componentSelect.Enabled = false;
            button1.Enabled = false;
            

            Task.Run(() =>
            {
                List<Task> tasks = new List<Task>();
                foreach (string[] download in Downloads)
                {
                    tasks.Add(Task.Run(() => DownloadEverything(download[0], download[1])));
                }
                Task.WaitAll(tasks.ToArray());
                MethodInvoker d = delegate () 
                {
                    Refresh();
                    progressBar1.Visible = false;
                    damageOutput.Enabled = displayGrid;
                    componentSelect.Enabled = true;
                    button1.Enabled = true;
                };
                this.BeginInvoke(d);
            });
        }

        private void damageOutput_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                DamageComparison view = new DamageComparison(Data, LineGraphSeriesCreator);
                view.ShowDialog();
            });
        }

        private void radarComparison_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                RadarChartOptions view = new RadarChartOptions(MasterData, Writer);
                //RadarChart view = new RadarChart(Data, RadarGraphSeriesCreator, RadarLabels);
                view.ShowDialog();
            });
        }

        private void weaponsSelect_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        if (!node.Checked)
                            node.Checked = true;
                    }
                }
                else
                {
                    Data.Add(MasterData[e.Node.Name]);
                    AddRow(MasterData[e.Node.Name]);
                }
            }
            else
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        if (node.Checked)
                            node.Checked = false;
                    }
                }
                else
                {
                    Data.RemoveAll(x => x == MasterData[e.Node.Name]);
                    RemoveRow(e.Node.Name);
                }
            }

            if (Data.Count > 1)
                Recolor();
        }

        private void OverclockedCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Overclocked = OverclockedCheckbox.Checked;
            ReloadTable();
        }

        private void OverpoweredCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Overpowered = OverpoweredCheckbox.Checked;
            ReloadTable();
        }

        private void HotCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Hot = HotCheckbox.Checked;
            ReloadTable();
        }



        private void ReloadTable()
        {
            dataGridView1.Rows.Clear();
            foreach (TreeNode parentNode in componentSelect.Nodes)
            {
                foreach (TreeNode item in parentNode.Nodes)
                {
                    if (item.Checked)
                        AddRow(MasterData[item.Name]);
                }
            }
            dataGridView1.AutoResizeColumns();
            if (Data.Count > 1)
                Recolor();
        }

        private void Refresh()
        {
            componentSelect.Nodes.Clear();
            Data.Clear();
            Task t;

            radarComparison.Enabled = false;
            damageOutput.Enabled = false;
            OverclockedCheckbox.Enabled = false;
            OverpoweredCheckbox.Enabled = false;
            HotCheckbox.Enabled = false;

            switch (Selected)
            {
                case "Weapons":
                    Writer = new WeaponFormWriter(typeof(Weapon));
                    t = Task.Run(() =>
                    {
                        MasterData = Weapon.parseAll();
                        //Task.Run(() => { Utility.AssignColors(MasterData.Values.ToList()); });
                        var tree = Writer.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Writer.GetColumns();
                    Downloads = Writer.GetDownloadInfo();
                    LineGraphSeriesCreator = Weapon.CreateLineGraphSeries;
                    RadarLabels = Weapon.RadarLabels();

                    t.Wait();

                    radarComparison.Enabled = true;
                    damageOutput.Enabled = true;
                    OverclockedCheckbox.Enabled = true;
                    OverpoweredCheckbox.Enabled = true;
                    HotCheckbox.Enabled = true;
                    break;
                case "Shields":
                    Writer = new ShieldFormWriter(typeof(Shield));
                    t = Task.Run(() =>
                    {
                        MasterData = Shield.parseAll();
                        var tree = Writer.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Writer.GetColumns();
                    Downloads = Writer.GetDownloadInfo();

                    t.Wait();

                    radarComparison.Enabled = true;
                    break;
                case "Power Plants":
                    Writer = new PowerPlantWriter(typeof(PowerPlant));
                    t = Task.Run(() =>
                    {
                        MasterData = PowerPlant.parseAll();
                        var tree = Writer.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Writer.GetColumns();
                    Downloads = Writer.GetDownloadInfo();

                    t.Wait();

                    radarComparison.Enabled = true;
                    break;
                case "Coolers":
                    Writer = new CoolerWriter(typeof(Cooler));
                    t = Task.Run(() =>
                    {
                        MasterData = Cooler.parseAll();
                        var tree = Writer.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Writer.GetColumns();
                    Downloads = Writer.GetDownloadInfo();

                    t.Wait();

                    radarComparison.Enabled = true;
                    break;
                case "Guns":
                    Writer = new GunFormWriter(typeof(Gun));
                    t = Task.Run(() =>
                    {
                        MasterData = Gun.parseAll();
                        //Task.Run(() => { Utility.AssignColors(MasterData.Values.ToList()); });
                        var tree = Writer.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Writer.GetColumns();
                    Downloads = Writer.GetDownloadInfo();
                    LineGraphSeriesCreator = Gun.CreateLineGraphSeries;

                    t.Wait();

                    radarComparison.Enabled = true;
                    damageOutput.Enabled = true;
                    break;
                case "Armor":
                    Writer = new ArmorWriter(typeof(Armor));
                    t = Task.Run(() =>
                    {
                        MasterData = Armor.parseAll();
                        //Task.Run(() => { Utility.AssignColors(MasterData.Values.ToList()); });
                        var tree = Writer.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Writer.GetColumns();
                    Downloads = Writer.GetDownloadInfo();

                    t.Wait();

                    // Nothing to really see yet..
                    //radarComparison.Enabled = true;
                    break;
            }

            dataGridView1.Columns.Clear();
            for (int i = 0; i < Columns.Length; i++)
            {
                dataGridView1.Columns.Add(Columns[i].Name, Columns[i].Name);
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].DefaultCellStyle.Format = Columns[i].Format;
                dataGridView1.Columns[i].Visible = Columns[i].Visible;
            }
            dataGridView1.Columns[dataGridView1.Columns.IndexOf("Name")].Frozen = true;
        }

        private void DownloadEverything(string Url, string FilePath)
        {
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);

            string rawUrls;
            using (WebClient web = new WebClient())
            {
                rawUrls = web.DownloadString(Url);
            }

            Parallel.ForEach(rawUrls.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries), new ParallelOptions { MaxDegreeOfParallelism = 5 }, item =>
            {
                using (WebClient web = new WebClient())
                {
                    int i = item.IndexOf("href=\"") + 6;
                    string url = item.Substring(i, item.IndexOf('"', i + 1) - i);
                    string json = web.DownloadString("http://starcitizendb.com" + url);
                    using (StreamWriter writer = File.CreateText(FilePath + "\\" + url.Split(new char[] { '/' }).Last()))
                    {
                        writer.Write(json);
                    }
                }
            });
            MethodInvoker d = delegate () { progressBar1.PerformStep(); };
            this.BeginInvoke(d);
        }

        private void AddRow(object data)
        {
            dataGridView1.Rows.Add();
            DataGridViewRow dataRow = new DataGridViewRow();
            dataRow = dataGridView1.Rows[dataGridView1.Rows.Count - 1];
            dataRow.ReadOnly = true;
            decimal score = 0M;
            int scoreColumn = dataGridView1.Columns.IndexOf("Score");

            Dictionary<string, decimal> scoreMultipliers = new Dictionary<string, decimal>();
            scoreMultipliers.DefaultIfEmpty(new KeyValuePair<string, decimal>("default", 0));
            if (richTextBox1.Text.Length > 0)
            {
                foreach (var valueSet in richTextBox1.Text.Split(','))
                {
                    string[] values = valueSet.Split('=');
                    scoreMultipliers.Add(values[0].ToLower(), Convert.ToDecimal(values[1]));
                }
                dataGridView1.Columns[scoreColumn].Visible = true;
            }


            foreach (var col in Columns)
            {
                int x = dataGridView1.Columns.IndexOf(col.Name);
                dataRow.Cells[x].Value = data.Get(col.DataName);
                decimal scoreMultiplier;
                if (scoreMultipliers.TryGetValue(col.Name.ToLower(), out scoreMultiplier))
                {
                    score += scoreMultiplier * Convert.ToDecimal(dataRow.Cells[dataGridView1.Columns.IndexOf(col.Name)].Value);
                }
            }
            dataRow.Cells[scoreColumn].Value = score;

            dataGridView1.AutoResizeColumns();
        }

        private void RemoveRow(string Name)
        {
            int x = dataGridView1.Rows.Count;
            for (int i = 0; i < x; i++)
            {
                if (dataGridView1.Rows[i].Cells[dataGridView1.Columns.IndexOf("Id")].Value.ToString() == Name)
                {
                    dataGridView1.Rows.RemoveAt(i);
                    break;
                }
            }

            Recolor();
            dataGridView1.AutoResizeColumns();
        }

        private void Recolor()
        {
            foreach (var col in Columns)
            {
                int i = dataGridView1.Columns.IndexOf(col.Name);
                List<object> values = new List<object>();

                if (col.Sort)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        values.Add(row.Cells[i].Value);
                    }

                    values = values.Distinct().OrderBy(x => Convert.ToDecimal(x)).ToList();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        row.Cells[i].Style.BackColor = GetColor(values.IndexOf(row.Cells[i].Value), values.Count, (bool)col.Invert);
                    }
                }
            }
        }

        private Color GetColor(int i, int l, bool invert)
        {
            int increment = 510 / (l);

            int r = 0;
            int g = 255;

            for (int x = 0; x < i; x++)
            {
                r = r + increment > 255 ? 255 : r + increment;
                if (r == 255)
                    g = g - increment < 0 ? 0 : g - increment;
            }

            if (invert)
                return Color.FromArgb(g, r, 0);
            else
                return Color.FromArgb(r, g, 0);

        }

        
    }

    public class Column
    {
        public string Name { get; set; }
        public bool Sort { get; set; }
        public bool Invert { get; set; }
        public string Format { get; set; }
        public bool Visible { get; set; }
        public string DataName { get; set; }
        public int Priority { get; set; }

        public Column(string name, string dataName, bool sort, bool sortDescending = false, string format = "", bool visible = true, int priority = 100)
        {
            Name = name;
            DataName = dataName;
            Sort = sort;
            Invert = sortDescending;
            Format = format;
            Visible = visible;
            Priority = priority;
        }
    }

    public static class Extensions
    {
        public static int IndexOf(this DataGridViewColumnCollection Collection, string Name)
        {
            int x = Collection.Count;
            for (int i = 0; i < x; i++)
            {
                if (Collection[i].Name == Name) return i;
            }
            return -1;
        }

        public static object Get(this object obj, string field)
        {
            if (string.IsNullOrEmpty(field))
                return null;

            return Utility.GetValue(obj, field);
        }
    }
}
