using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes.NewFolder1
{
    public class GunFormWriter : FormWriter
    {
        public GunFormWriter(Type type) : base(type)
        {
            Fields = new List<FieldInfo>
            {
                new FieldInfo
                {
                    DisplayFieldName = "Speed",
                    DataFieldName = "Speed",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "DamageTotal",
                    DataFieldName = "DamageTotal",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Single.Firerate",
                    DataFieldName = "Single.Firerate",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Single.AverageTTK",
                    DataFieldName = "Single.AverageTTK",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Rapid.Firerate",
                    DataFieldName = "Rapid.Firerate",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Rapid.AverageTTK",
                    DataFieldName = "Rapid.AverageTTK",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Weight",
                    DataFieldName = "Weight",
                    SortDescending = false
                },
            };
        }

        public override Column[] GetColumns()
        {
            return new Column[] {
                new Column("Id", "Id", false, false, "", false),
                new Column("Name", "Name", false),
                new Column("Total Damage", "DamageTotal", true, true),
                new Column("Special Damage", "DamageSpecial", true, true, "N2"),
                //new Column("Projectiles Per Shot", "ProjectilesPerShot", true, true),
                new Column("Singleshot Firerate", "Single.Firerate", true, true, "N2"),
                new Column("Burst Firerate", "Burst.Firerate", true, true, "N2"),
                new Column("Auto Firerate", "Rapid.Firerate", true, true, "N2"),
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
                new Column("Weight", "Weight", true, false, "N2"),
                //new Column("Max Spread", "MaxSpread", true, false, "N3"),
                //new Column("Initial Spread", "InitialSpread", true, false, "N3"),
                //new Column("Spread Growth", "SpreadGrowth", true, false, "N3"),
                //new Column("Spread Decay", "SpreadDecay", true, true, "N3"),
                //new Column("Spread Per Second", "SpreadPerSecond", true, false, "N3"),
                //new Column("Time Until Max Spread", "TimeUntilMaxSpread", true, true, "N3"),
                new Column("Score", null, true, true, "N2", false),
            };
        }

        public override List<Series> CreateRadarGraphSeries(List<object> Data, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 2, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;
                    Series s = g.GetNewRadarGraphSeries();
                    foreach (var fieldInfo in Fields)
                    {
                        string[] fieldPath = fieldInfo.DataFieldName.Split('.');
                        if (fieldPath.Count() > 1)
                        {
                            s.Points.Add(new DataPoint(0, GetRank(fieldInfo.DataFieldName, Convert.ToDouble(Utility.GetValue(g, fieldInfo.DataFieldName)), fieldInfo.SortDescending)));
                        }
                        else
                            s.Points.Add(new DataPoint(0, GetRank(fieldInfo.DataFieldName, Convert.ToDouble(Utility.GetValue(g, fieldInfo.DataFieldName)), fieldInfo.SortDescending)));
                    }
                    list.Enqueue(s);
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).ToList();
        }

        public override List<CustomLabel> RadarLabels()
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

        public override List<string[]> GetDownloadInfo()
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/WeaponPersonal", Gun.GunPath }
                ,new string[] { "http://starcitizendb.com/api/ammo/energy", Gun.AmmoPath }
                ,new string[] { "http://starcitizendb.com/api/ammo/projectile", Gun.AmmoPath }
                ,new string[] { "http://starcitizendb.com/api/components/df/WeaponAttachment", Gun.AttachmentPath }
            };
        }
    }
}
