using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Weapon : Item
    {
        private decimal _firerate { get; set; }
        public decimal Firerate {
            get
            {
                decimal output = _firerate;
                if (MainSheet.Overclocked)
                    output *= OverclockFirerateMultiplier;
                if (MainSheet.Overpowered)
                    output *= OverpowerFirerateMultiplier;
                if (MainSheet.Hot)
                    output *= HeatFirerateMultiplier;
                return output;
            }
            set
            {
                _firerate = value;
            }
        }
        public int ProjectilesPerShot { get; set; }

        public int MaximumTemperature { get; set; }
        public decimal HeatPerShot { get; set; }
        public decimal HeatPerSecond
        {
            get
            {
                return HeatPerShot * Firerate;
            }
        }
        public decimal HeatUptime
        {
            get
            {
                return MaximumTemperature / HeatPerSecond;
            }
        }


        public int PowerBase { get; set; }
        public int PowerDraw { get; set; }
        public int PowerPerShot
        {
            get
            {
                return PowerBase + PowerDraw;
            }
        }
        

        // Ammo
        public decimal Lifetime { get; set; }
        public int Speed { get; set; }
        private decimal _damageBiochemical { get; set; }
        private decimal _damageDistortion { get; set; }
        private decimal _damageEnergy { get; set; }
        private decimal _damagePhysical { get; set; }
        private decimal _damageThermal { get; set; }
        public decimal DamageBiochemical
        {
            get
            {
                decimal output = _damageBiochemical;
                if (MainSheet.Overclocked)
                    output *= OverclockDamageMultiplier;
                if (MainSheet.Overpowered)
                    output *= OverpowerDamageMultiplier;
                if (MainSheet.Hot)
                    output *= HeatDamageMultiplier;
                return output;
            }
            set
            {
                _damageBiochemical = value;
            }
        }
        public decimal DamageDistortion
        {
            get
            {
                decimal output = _damageDistortion;
                if (MainSheet.Overclocked)
                    output *= OverclockDamageMultiplier;
                if (MainSheet.Overpowered)
                    output *= OverpowerDamageMultiplier;
                if (MainSheet.Hot)
                    output *= HeatDamageMultiplier;
                return output;
            }
            set
            {
                _damageDistortion = value;
            }
        }
        public decimal DamageEnergy
        {
            get
            {
                decimal output = _damageEnergy;
                if (MainSheet.Overclocked)
                    output *= OverclockDamageMultiplier;
                if (MainSheet.Overpowered)
                    output *= OverpowerDamageMultiplier;
                if (MainSheet.Hot)
                    output *= HeatDamageMultiplier;
                return output;
            }
            set
            {
                _damageEnergy = value;
            }
        }
        public decimal DamagePhysical
        {
            get
            {
                decimal output = _damagePhysical;
                if (MainSheet.Overclocked)
                    output *= OverclockDamageMultiplier;
                if (MainSheet.Overpowered)
                    output *= OverpowerDamageMultiplier;
                if (MainSheet.Hot)
                    output *= HeatDamageMultiplier;
                return output;
            }
            set
            {
                _damagePhysical = value;
            }
        }
        public decimal DamageThermal
        {
            get
            {
                decimal output = _damageThermal;
                if (MainSheet.Overclocked)
                    output *= OverclockDamageMultiplier;
                if (MainSheet.Overpowered)
                    output *= OverpowerDamageMultiplier;
                if (MainSheet.Hot)
                    output *= HeatDamageMultiplier;
                return output;
            }
            set
            {
                _damageThermal = value;
            }
        }
        public decimal DamageTotal {
            get
            {
                return DamageBiochemical + DamageDistortion + DamageEnergy + DamagePhysical + DamageThermal;
            }
        }
        public decimal DamageSpecial
        {
            get
            {
                return DamageTotal * ProjectilesPerShot;
            }
        }
        public decimal DamagePerSecond
        {
            get
            {
                return DamageTotal * Firerate;
            }
        }
        public decimal DamagePerSecondSpecial
        {
            get
            {
                return DamageSpecial * Firerate;
            }
        }
        public int MaxRange
        {
            get
            {
                return Convert.ToInt32(Speed * Lifetime);
            }
        }
        public decimal DamagePerPower
        {
            get
            {
                return DamagePerSecond / (PowerBase + PowerDraw);
            }
        }
        public decimal DamagePerHeat
        {
            get
            {
                return DamageTotal / HeatPerShot;
            }
        }

        public decimal MaxSpread { get; set; }
        public decimal InitialSpread { get; set; }
        public decimal SpreadGrowth { get; set; }
        public decimal SpreadDecay { get; set; }
        public decimal SpreadPerSecond
        {
            get
            {
                return (Firerate * SpreadGrowth) - SpreadDecay > 0 ? (Firerate * SpreadGrowth) - SpreadDecay : 0;
            }
        }
        public decimal TimeUntilMaxSpread
        {
            get
            {
                return SpreadPerSecond != 0 && (MaxSpread-InitialSpread) / SpreadPerSecond >= 0 ? (MaxSpread - InitialSpread) / SpreadPerSecond : 999;
            }
        }

        public decimal OverpowerFirerateMultiplier = 1;
        public decimal OverpowerDamageMultiplier = 1;
        public decimal OverpowerAmmoCostMultiplier = 1;
        public decimal OverpowerHeatMultiplier = 1;

        public decimal OverclockFirerateMultiplier = 1;
        public decimal OverclockDamageMultiplier = 1;
        public decimal OverclockAmmoCostMultiplier = 1;
        public decimal OverclockHeatMultiplier = 1;

        public decimal HeatFirerateMultiplier = 1;
        public decimal HeatDamageMultiplier = 1;
        public decimal HeatAmmoCostMultiplier = 1;
        public decimal HeatHeatMultiplier = 1;

        public Weapon(dynamic Json, string File)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;
            Type = Types.Weapon;

            Firerate = Json.Components.SCItemWeaponComponentParams.fire.fireRate / 60M;
            ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.pelletCount ?? 1;
            //HeatPerShot = Json.Components.SCItemWeaponComponentParams.fire.heatPerShot;
            //MaximumTemperature = Json.Components.EntityComponentHeatConnection.MaximumTemperature;

            PowerBase = Json.Components.EntityComponentPowerConnection.PowerBase;
            PowerDraw = Json.Components.EntityComponentPowerConnection.PowerDraw;

            MaxSpread = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.max;
            InitialSpread = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.firstAttack;
            SpreadGrowth = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.attack;
            SpreadDecay = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.decay;

            Lifetime = Json.ammo.lifetime;
            Speed = Json.ammo.speed;
            DamageBiochemical = Json.ammo.bullet.damage.DamageInfo.DamageBiochemical;
            DamageDistortion = Json.ammo.bullet.damage.DamageInfo.DamageDistortion;
            DamageEnergy = Json.ammo.bullet.damage.DamageInfo.DamageEnergy;
            DamagePhysical = Json.ammo.bullet.damage.DamageInfo.DamagePhysical;
            DamageThermal = Json.ammo.bullet.damage.DamageInfo.DamageThermal;

            // Explosive ammo
            DamageBiochemical += Json.ammo.bullet.detonation != null ? (int)Json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageBiochemical : 0;
            DamageDistortion += Json.ammo.bullet.detonation != null ? (int)Json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageDistortion : 0;
            DamageEnergy += Json.ammo.bullet.detonation != null ? (int)Json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageEnergy : 0;
            DamagePhysical += Json.ammo.bullet.detonation != null ? (int)Json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamagePhysical : 0;
            DamageThermal += Json.ammo.bullet.detonation != null ? (int)Json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageThermal : 0;

            OverclockFirerateMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.fireRateMultiplier;
            OverclockDamageMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.damageMultiplier;
            OverclockAmmoCostMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.ammoCostMultiplier;
            OverclockHeatMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.heatGenerationMultiplier;

            OverpowerFirerateMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.fireRateMultiplier;
            OverpowerDamageMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.damageMultiplier;
            OverpowerAmmoCostMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.ammoCostMultiplier;
            OverpowerHeatMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.heatGenerationMultiplier;

            HeatFirerateMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.fireRateMultiplier;
            HeatDamageMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.damageMultiplier;
            HeatAmmoCostMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.ammoCostMultiplier;
            HeatHeatMultiplier = Json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.heatGenerationMultiplier;
        }

        public static Dictionary<string,object> parseAll(string filePath)
        {
            ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();
            Parallel.ForEach(Directory.GetFiles(filePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Weapon w = new Weapon(json, path.Replace(filePath + "\\", "").Replace(".json", ""));
                    output.TryAdd(w.Id, w);
                }
                catch (Exception)
                {
                    #if !DEBUG
                    File.Delete(path);
                    #endif
                }
            });
            return new Dictionary<string, object>(output);
        }

        public static Column[] GetColumns()
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
                //new Column("Damage Per Heat", "DamagePerHeat", true, true, "N2"),
                //new Column("Power Per Shot", "PowerPerShot", true, false),
                //new Column("Heat Per Shot", "HeatPerShot", true, false),
                //new Column("Heat Per Second", "HeatPerSecond", true, false, "N2"),
                //new Column("Heat Uptime", "HeatUptime", true, true, "N2"),
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
        }

        public static List<string[]> GetDownloadInfo(string FilePath)
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/WeaponGun", FilePath + "\\weapons" }
                //,new string[] {"http://starcitizendb.com/api/components/df/PowerPlant", FilePath + "\\power plants"}
                //,new string[] {"http://starcitizendb.com/api/components/df/Cooler", FilePath + "\\coolers"}
                //,new string[] {"http://starcitizendb.com/api/components/df/Shield", FilePath + "\\shields"}
            };
        }

        public static TreeNode[] BuildTree(object[] Items)
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

        public static List<Series> Calculator(List<object> Data, int Ticks, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 5, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Weapon w = (Weapon)item;
                    Series s = w.GetNewSeries();

                    decimal[] x = new decimal[Ticks + 1];
                    decimal[] y = new decimal[Ticks + 1];
                    decimal firerate = Convert.ToDecimal(item.Get("Firerate"));
                    for (int i = 0; i <= Ticks; i++)
                    {
                        x[i] = i * .05M;
                        y[i] = w.DamageTotal + (w.DamageTotal * Math.Floor((i * .05M) / (1 / w.Firerate)));
                        if (options.CancellationToken.IsCancellationRequested)
                            break;
                    }

                    if (options.CancellationToken.IsCancellationRequested)
                        loopState.Break();

                    s.Points.DataBindXY(x, y);
                    list.Enqueue(s);
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).ToList();
        }
    }
}
