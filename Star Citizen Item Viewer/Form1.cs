using Star_Citizen_Item_Viewer.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Star_Citizen_Item_Viewer
{
    public partial class Form1 : Form
    {
        public static string FilePath = Directory.GetCurrentDirectory();
        public static Column[] WeaponColumns =
        {
            new Column("Name", "Name", false),
            new Column("Size", "Size", false),
            new Column("Alpha Damage", "DamageTotal", true, true),
            new Column("Damage Per Second", "DamagePerSecond", true, true, "N2"),
            new Column("Firerate", "Firerate", true, true, "N2"),
            new Column("Biochemical Damage", "DamageBiochemical", true, true),
            new Column("Distortion Damage", "DamageDistortion", true, true),
            new Column("Energy Damage", "DamageEnergy", true, true),
            new Column("Physical Damage", "DamagePhysical", true, true),
            new Column("Thermal Damage", "DamageThermal", true, true),
            new Column("Damage Per Power", "DamagePerPower", true, true, "N2"),
            new Column("Damage Per Heat", "DamagePerHeat", true, true, "N2"),
            new Column("Power Per Shot", "PowerPerShot", true, false),
            new Column("Heat Per Shot", "HeatPerShot", true, false),
            new Column("Heat Per Second", "HeatPerSecond", true, false, "N2"),
            new Column("Heat Uptime", "HeatUptime", true, true, "N2"),
            new Column("Projectile Velocity", "Speed", true, true),
            new Column("Max Range", "MaxRange", true, true),
            new Column("Max Spread", "MaxSpread", true, false, "N3"),
            new Column("Initial Spread", "InitialSpread", true, false, "N3"),
            new Column("Spread Growth", "SpreadGrowth", true, false, "N3"),
            new Column("Spread Decay", "SpreadDecay", true, true, "N3"),
            new Column("Spread Per Second", "SpreadPerSecond", true, false, "N3"),
            new Column("Time Until Max Spread", "TimeUntilMaxSpread", true, true, "N3"),
            new Column("Score", null, true, true, "N2", false),
        };
        public static Column[] GunColumns =
                {
            new Column("Name", "Name", false),
            new Column("Total Damage", "DamageTotal", true, true),
            new Column("Singleshot Firerate", "SingleFirerate", true, true, "N2"),
            new Column("Burst Firerate", "BurstFirerate", true, true, "N2"),
            new Column("Auto Firerate", "RapidFirerate", true, true, "N2"),
            new Column("Biochemical Damage", "DamageBiochemical", true, true),
            new Column("Distortion Damage", "DamageDistortion", true, true),
            new Column("Energy Damage", "DamageEnergy", true, true),
            new Column("Physical Damage", "DamagePhysical", true, true),
            new Column("Thermal Damage", "DamageThermal", true, true),
            //new Column("Damage Per Power", "DamagePerPower", true, true, "N2"),
            //new Column("Damage Per Heat", "DamagePerHeat", true, true, "N2"),
            //new Column("Power Per Shot", "PowerPerShot", true, false),
            //new Column("Heat Per Shot", "HeatPerShot", true, false),
            //new Column("Heat Per Second", "HeatPerSecond", true, false, "N2"),
            //new Column("Heat Uptime", "HeatUptime", true, true, "N2"),
            new Column("Projectile Velocity", "Speed", true, true),
            new Column("Max Range", "MaxRange", true, true),
            //new Column("Max Spread", "MaxSpread", true, false, "N3"),
            //new Column("Initial Spread", "InitialSpread", true, false, "N3"),
            //new Column("Spread Growth", "SpreadGrowth", true, false, "N3"),
            //new Column("Spread Decay", "SpreadDecay", true, true, "N3"),
            //new Column("Spread Per Second", "SpreadPerSecond", true, false, "N3"),
            //new Column("Time Until Max Spread", "TimeUntilMaxSpread", true, true, "N3"),
            new Column("Score", null, true, true, "N2", false),
        };

        public static Dictionary<string, object> MasterData = new Dictionary<string, object>();
        private static string LastSelection = "";
        private static string Selected = "Weapons";

        public Form1()
        {
            InitializeComponent();
            this.Text = "Star Citizen Item Viewer";
            weaponsSelect.CheckBoxes = true;
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
            if (!Directory.Exists(FilePath + "\\magazines"))
                Directory.CreateDirectory(FilePath + "\\magazines");
            if (!Directory.Exists(FilePath + "\\ammo"))
                Directory.CreateDirectory(FilePath + "\\ammo");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.SelectedIndex = 0;
            Refresh();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected = (string)listBox1.SelectedItem;
            label1.Text = Selected;
            Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string[]> downloads = new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/WeaponGun", FilePath + "\\weapons" }
                ,new string[] { "http://starcitizendb.com/api/components/df/WeaponPersonal", FilePath + "\\guns" }
                ,new string[] { "http://starcitizendb.com/api/ammo/energy", FilePath + "\\ammo" }
                ,new string[] { "http://starcitizendb.com/api/ammo/projectile", FilePath + "\\ammo" }
                ,new string[] { "http://starcitizendb.com/api/components/df/WeaponAttachment", FilePath + "\\attachments" }
                //,new string[] {"http://starcitizendb.com/api/components/df/PowerPlant", FilePath + "\\power plants"}
                //,new string[] {"http://starcitizendb.com/api/components/df/Cooler", FilePath + "\\coolers"}
                //,new string[] {"http://starcitizendb.com/api/components/df/Shield", FilePath + "\\shields"}
            };
            progressBar1.Value = 0;
            progressBar1.Step = 100 / downloads.Count;
            progressBar1.Visible = true;

            foreach (string[] download in downloads)
            {
                downloadEverything(download[0], download[1]);
                progressBar1.PerformStep();
            }
            progressBar1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            List<object> data = new List<object>();
            Column[] columns;

            switch (Selected)
            {
                case "Weapons":
                    columns = WeaponColumns;
                    break;
                case "Guns":
                    columns = GunColumns;
                    break;
                default: columns = WeaponColumns; break;
            }

            foreach (TreeNode group in weaponsSelect.Nodes)
            {
                foreach (TreeNode node in group.Nodes)
                {
                    if (node.Checked || group.Checked)
                        data.Add(MasterData[node.Name]);
                }
            }

            DataFill(data, columns);

            dataGridView1.Columns[dataGridView1.Columns.IndexOf("Name")].Frozen = true;
            dataGridView1.AutoResizeColumns();
            LastSelection = Selected;
        }


        private void Refresh()
        {
            weaponsSelect.Nodes.Clear();
            switch (Selected)
            {
                case "Weapons":
                    MasterData = Weapon.parseAll(FilePath + "\\weapons");
                    break;
                case "Power Plants":
                    MasterData = new Dictionary<string, object>();
                    break;
                case "Coolers":
                    MasterData = new Dictionary<string, object>();
                    break;
                case "Guns":
                    MasterData = Gun.parseAll(FilePath + "\\guns", FilePath + "\\attachments", FilePath + "\\ammo");
                    break;
            }
            Dictionary<int, TreeNode> sizes = new Dictionary<int, TreeNode>();
            foreach (Item weapon in MasterData.Values)
            {
                TreeNode n = new TreeNode();
                n.Name = weapon.Id;
                n.Text = weapon.Name;

                if (sizes.ContainsKey(weapon.Size))
                    sizes[weapon.Size].Nodes.Add(n);
                else
                    sizes.Add(weapon.Size, new TreeNode("Size " + weapon.Size.ToString(), new TreeNode[] { n }));
            }
            foreach (var key in sizes.Keys.OrderBy(x => x))
            {
                weaponsSelect.Nodes.Add(sizes[key]);
            }
        }

        private void downloadEverything(string Url, string FilePath)
        {
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
            using (WebClient web = new WebClient())
            {
                string rawUrls = web.DownloadString(Url);
                foreach (var item in rawUrls.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int i = item.IndexOf("href=\"") + 6;
                    string url = item.Substring(i, item.IndexOf('"', i + 1) - i);
                    string json = web.DownloadString("http://starcitizendb.com"+url);
                    using (StreamWriter writer = File.CreateText(FilePath+"\\"+url.Split(new char[] { '/' }).Last()))
                    {
                        writer.Write(json);
                    }
                }
            }
            Refresh();
        }

        private void DataFill(List<object> Data, Column[] Columns)
        {
            if (Selected != LastSelection)
            {
                dataGridView1.Columns.Clear();
                for (int i = 0; i < Columns.Length; i++)
                {
                    dataGridView1.Columns.Add(Columns[i].Name, Columns[i].Name);
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.Format = Columns[i].Format;
                    dataGridView1.Columns[i].Visible = Columns[i].Visible;
                }
            }

            int scoreColumn = dataGridView1.Columns.IndexOf("Score");

            for (int i = 0; i < Data.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].ReadOnly = true;
                foreach (var col in Columns)
                {
                    dataGridView1.Rows[i].Cells[dataGridView1.Columns.IndexOf(col.Name)].Value = Data[i].Get(col.DataName);
                }
            }

            bool colorCode = Data.Count > 1;
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
                int i = dataGridView1.Columns.IndexOf(col.Name);
                List<object> values = new List<object>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    values.Add(row.Cells[i].Value);
                    try
                    {
                        decimal scoreMultiplier;
                        scoreMultipliers.TryGetValue(col.Name.ToLower(), out scoreMultiplier);
                        row.Cells[scoreColumn].Value = Convert.ToDecimal(row.Cells[scoreColumn].Value) + Convert.ToDecimal(row.Cells[i].Value) * scoreMultiplier;
                    }
                    catch (Exception ex)
                    {
                    }

                }
                if (colorCode && col.Sort)
                {
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
                if ((float)x / (float)l < .5)
                    r = r + increment > 255 ? 255 : r + increment;
                else
                    g = g - increment < 0 ? 0 : g - increment;
            }

            if (invert)
                return Color.FromArgb(g, r, 0);
            else
                return Color.FromArgb(r,g,0);
                
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
