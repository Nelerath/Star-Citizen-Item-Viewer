using Star_Citizen_Item_Viewer.Classes;
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
        public static string FilePath = Directory.GetCurrentDirectory();

        public static Dictionary<string, object> MasterData = new Dictionary<string, object>();
        private static string LastSelection = "";
        private static string Selected = "";

        private static List<string[]> Downloads = Weapon.GetDownloadInfo(FilePath);

        private static List<object> Data = new List<object>();
        private static Column[] Columns = Weapon.GetColumns();
        private static Func<List<object>, int, CancellationToken, List<Series>> GraphCalculator = Weapon.Calculator;

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

            if (!Directory.Exists(FilePath + "\\weapons"))
                Directory.CreateDirectory(FilePath + "\\weapons");
            if (!Directory.Exists(FilePath + "\\power plants"))
                Directory.CreateDirectory(FilePath + "\\power plants");
            if (!Directory.Exists(FilePath + "\\coolers"))
                Directory.CreateDirectory(FilePath + "\\coolers");
            if (!Directory.Exists(FilePath + "\\shields"))
                Directory.CreateDirectory(FilePath + "\\shields");
            if (!Directory.Exists(FilePath + "\\guns"))
                Directory.CreateDirectory(FilePath + "\\guns");
            if (!Directory.Exists(FilePath + "\\attachments"))
                Directory.CreateDirectory(FilePath + "\\attachments");
            if (!Directory.Exists(FilePath + "\\ammo"))
                Directory.CreateDirectory(FilePath + "\\ammo");
            if (!Directory.Exists(FilePath + "\\armor"))
                Directory.CreateDirectory(FilePath + "\\armor");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

            bool displayGrid = DisplayGrid.Enabled;
            DisplayGrid.Enabled = false;
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
                    DisplayGrid.Enabled = displayGrid;
                    componentSelect.Enabled = true;
                    button1.Enabled = true;
                };
                this.BeginInvoke(d);
            });
        }

        private void DisplayGrid_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                DamageComparison view = new DamageComparison(Data, GraphCalculator);
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
            switch (Selected)
            {
                case "Weapons":
                    Task.Run(() =>
                    {
                        MasterData = Weapon.parseAll(FilePath + "\\weapons");
                        //Task.Run(() => { Utility.AssignColors(MasterData.Values.ToList()); });
                        var tree = Weapon.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Weapon.GetColumns();
                    Downloads = Weapon.GetDownloadInfo(FilePath);
                    GraphCalculator = Weapon.Calculator;
                    DisplayGrid.Enabled = true;
                    OverclockedCheckbox.Enabled = true;
                    OverpoweredCheckbox.Enabled = true;
                    HotCheckbox.Enabled = true;
                    break;
                case "Power Plants":
                    MasterData = new Dictionary<string, object>();
                    DisplayGrid.Enabled = false;
                    break;
                case "Coolers":
                    MasterData = new Dictionary<string, object>();
                    DisplayGrid.Enabled = false;
                    break;
                case "Guns":
                    Task.Run(() =>
                    {
                        MasterData = Gun.parseAll(FilePath + "\\guns", FilePath + "\\attachments", FilePath + "\\ammo");
                        //Task.Run(() => { Utility.AssignColors(MasterData.Values.ToList()); });
                        var tree = Gun.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Gun.GetColumns();
                    Downloads = Gun.GetDownloadInfo(FilePath);
                    GraphCalculator = Gun.Calculator;
                    DisplayGrid.Enabled = true;
                    OverclockedCheckbox.Enabled = false;
                    OverpoweredCheckbox.Enabled = false;
                    HotCheckbox.Enabled = false;
                    break;
                case "Armor":
                    Task.Run(() =>
                    {
                        MasterData = Armor.parseAll(FilePath + "\\Armor");
                        //Task.Run(() => { Utility.AssignColors(MasterData.Values.ToList()); });
                        var tree = Armor.BuildTree(MasterData.Values.ToArray());
                        MethodInvoker d = delegate ()
                        {
                            componentSelect.Nodes.AddRange(tree);
                        };
                        componentSelect.BeginInvoke(d);
                    });
                    Columns = Armor.GetColumns();
                    Downloads = Armor.GetDownloadInfo(FilePath);
                    DisplayGrid.Enabled = false;
                    OverclockedCheckbox.Enabled = false;
                    OverpoweredCheckbox.Enabled = false;
                    HotCheckbox.Enabled = false;
                    break;
            }
            
            //Dictionary<int, TreeNode> sizes = new Dictionary<int, TreeNode>();
            //foreach (Item weapon in MasterData.Values)
            //{
            //    TreeNode n = new TreeNode();
            //    n.Name = weapon.Id;
            //    n.Text = weapon.Name;
            //
            //    if (sizes.ContainsKey(weapon.Size))
            //        sizes[weapon.Size].Nodes.Add(n);
            //    else
            //        sizes.Add(weapon.Size, new TreeNode("Size " + weapon.Size.ToString(), new TreeNode[] { n }));
            //}
            //foreach (var key in sizes.Keys.OrderBy(x => x))
            //{
            //    componentSelect.Nodes.Add(sizes[key]);
            //}

            

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

            //List<Task> tasks = new List<Task>();
            //foreach (var item in rawUrls.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    tasks.Add(Task.Run(() =>
            //    {
            //        using (WebClient web = new WebClient())
            //        {
            //            int i = item.IndexOf("href=\"") + 6;
            //            string url = item.Substring(i, item.IndexOf('"', i + 1) - i);
            //            string json = web.DownloadString("http://starcitizendb.com" + url);
            //            using (StreamWriter writer = File.CreateText(FilePath + "\\" + url.Split(new char[] { '/' }).Last()))
            //            {
            //                writer.Write(json);
            //            }
            //        }
            //
            //    }));
            //}
            //Task.WaitAll(tasks.ToArray());
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

        public Column(string n, string d, bool s, bool i = false, string f = "", bool v = true)
        {
            Name = n;
            DataName = d;
            Sort = s;
            Invert = i;
            Format = f;
            Visible = v;
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

        public static object Get(this object Obj, string Name)
        {
            if (string.IsNullOrEmpty(Name))
                return null;
            else
                return Obj.GetType().GetProperty(Name).GetValue(Obj);
        }
    }
}
