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
    public class WeaponFormWriter : FormWriter
    {
        public WeaponFormWriter()
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
                    DisplayFieldName = "Firerate",
                    DataFieldName = "Firerate",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "DamagePerSecond",
                    DataFieldName = "DamagePerSecond",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Three Seconds Damage",
                    DataFieldName = "ThreeSecondDamge",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Ten Seconds Damage",
                    DataFieldName = "TenSecondDamage",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "Sixty Seconds Damage",
                    DataFieldName = "SixtySecondDamage",
                    SortDescending = true
                },
                new FieldInfo
                {
                    DisplayFieldName = "MaxSpread",
                    DataFieldName = "MaxSpread",
                    SortDescending = false
                },
                new FieldInfo
                {
                    DisplayFieldName = "InitialSpread",
                    DataFieldName = "InitialSpread",
                    SortDescending = false
                },
            };
        }

        public override Column[] GetColumns()
        {
            return new Column[] {
                new Column("Id", "Id", false, false, "", false),
                new Column("Name", "Name", false),
                new Column("Size", "Size", false),
                new Column("Alpha Damage", "DamageTotal", true, true),
                new Column("Special Damage", "DamageSpecial", true, true),
                new Column("Damage Per Second", "DamagePerSecond", true, true, "N2"),
                new Column("Special Damage Per Second", "DamagePerSecondSpecial", true, true, "N2"),
                new Column("Firerate", "Firerate", true, true, "N2"),
                new Column("Projectiles Per Shot", "ProjectilesPerShot", true, true),
                new Column("Biochemical Damage", "DamageBiochemical", true, true),
                new Column("Distortion Damage", "DamageDistortion", true, true),
                new Column("Energy Damage", "DamageEnergy", true, true),
                new Column("Physical Damage", "DamagePhysical", true, true),
                new Column("Thermal Damage", "DamageThermal", true, true),
                //new Column("Damage Per Power", "DamagePerPower", true, true, "N2"),
                new Column("Damage Per Heat", "DamagePerHeat", true, true, "N3"),
                //new Column("Power Per Shot", "PowerPerShot", true, false),
                new Column("Heat Per Shot", "HeatPerShot", true, false),
                //new Column("Heat Per Second", "HeatPerSecond", true, false, "N2"),
                //new Column("Heat Uptime", "HeatUptime", true, true, "N2"),
                new Column("Projectile Velocity", "Speed", true, true),
                new Column("Max Range", "MaxRange", true, true),
                new Column("Min Spread", "MinSpread", true, false, "N3"),
                new Column("Max Spread", "MaxSpread", true, false, "N3"),
                new Column("Initial Spread", "InitialSpread", true, false, "N3"),
                new Column("Spread Growth", "SpreadGrowth", true, false, "N3"),
                new Column("Spread Decay", "SpreadDecay", true, true, "N3"),
                new Column("Spread Per Second", "SpreadPerSecond", true, false, "N3"),
                new Column("Time Until Max Spread", "TimeUntilMaxSpread", true, true, "N3"),
                new Column("Score", null, true, true, "N2", false),
            };
        }

        public List<string[]> GetDownloadInfo(string FilePath)
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/WeaponGun", FilePath + "\\weapons" }
            };
        }
    }
}
