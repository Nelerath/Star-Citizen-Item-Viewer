using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Gun : Item
    {
        public decimal ADSTime { get; set; }

        public decimal SingleFirerate { get; set; }
        public decimal SingleMaxSpread { get; set; }
        public decimal SingleInitialSpread { get; set; }
        public decimal SingleSpreadGrowth { get; set; }
        public decimal SingleSpreadDecay { get; set; }

        public decimal BurstFirerate { get; set; }
        public decimal BurstMaxSpread { get; set; }
        public decimal BurstInitialSpread { get; set; }
        public decimal BurstSpreadGrowth { get; set; }
        public decimal BurstSpreadDecay { get; set; }

        public decimal RapidFirerate { get; set; }
        public decimal RapidMaxSpread { get; set; }
        public decimal RapidInitialSpread { get; set; }
        public decimal RapidSpreadGrowth { get; set; }
        public decimal RapidSpreadDecay { get; set; }

        public int ProjectilesPerShot { get; set; }

        public Magazine Magazine { get; set; }

        public int Speed
        {
            get
            {
                return Magazine.Ammo.Speed;
            }
        }

        public decimal MaxRange
        {
            get
            {
                return Magazine.Ammo.Speed * Magazine.Ammo.Lifetime;
            }
        }

        public decimal DamageBiochemical { get { return Magazine.Ammo.DamageBiochemical * ProjectilesPerShot; } }
        public decimal DamageDistortion { get { return Magazine.Ammo.DamageDistortion * ProjectilesPerShot; } }
        public decimal DamageEnergy { get { return Magazine.Ammo.DamageEnergy * ProjectilesPerShot; } }
        public decimal DamagePhysical { get { return Magazine.Ammo.DamagePhysical * ProjectilesPerShot; } }
        public decimal DamageThermal { get { return Magazine.Ammo.DamageThermal * ProjectilesPerShot; } }
        public decimal DamageTotal { get { return Magazine.Ammo.DamageTotal * ProjectilesPerShot; } }
        public decimal DamageSpecial { get; set; }

        public decimal Weight { get; set; }

        public Gun(dynamic Json, string File, Dictionary<string, object> Magazines)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;
            Type = Types.Gun;

            Magazine = Magazines[Convert.ToString(Json.Components.SCItemWeaponComponentParams.ammoContainerRecord)];
            ProjectilesPerShot = 1;
            //ADSTime = Json.Components.SCItemWeaponComponentParams.aimAction.SWeaponActionAimSimpleParams.zoomInTime;

            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams != null)
            {
                SingleFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.fireRate / 60M;
                SingleMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.max;
                SingleInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                SingleSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.attack;
                SingleSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.decay;
                ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.pelletCount ?? 1;
            }

            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams != null)
            {
                BurstFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.fireRate / 60M;
                BurstMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.max;
                BurstInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                BurstSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.attack;
                BurstSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.decay;
                ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.pelletCount ?? 1;
            }
            
            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams != null)
            {
                RapidFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.fireRate / 60M;
                RapidMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.max;
                RapidInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                RapidSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.attack;
                RapidSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.decay;
                ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.pelletCount ?? 1;
            }

            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams != null)
            {
                DamageSpecial = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.maxChargeModifier.damageMultiplier * DamageTotal;
            }
            else
            {
                DamageSpecial = DamageTotal;
            }

            Weight = Convert.ToDecimal(Json.Components.SEntityPhysicsControllerParams.PhysType.SEntityRigidPhysicsControllerParams.Mass) + Magazine.Weight;
        }

        //public static Dictionary<string, object> parseAll(string FilePath)
        //{
        //    ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();
        //
        //    Parallel.ForEach(Directory.GetFiles(FilePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
        //    {
        //        try
        //        {
        //            string raw = File.ReadAllText(path).Replace("@", "");
        //            dynamic json = JsonConvert.DeserializeObject(raw);
        //            Gun g = new Gun(json, path.Replace(FilePath + "\\", "").Replace(".json", ""));
        //            output.TryAdd(g.Id, g);
        //        }
        //        catch (Exception ex) { }
        //    });
        //
        //    return new Dictionary<string, object>(output);
        //}

        public static Dictionary<string, object> parseAll(string GunFilePath, string AttachmentFilePath, string AmmoFilePath)
        {
            ConcurrentDictionary<string, object> magazines = new ConcurrentDictionary<string, object>();
            ConcurrentDictionary<string, object> ammo = new ConcurrentDictionary<string, object>();

            Parallel.ForEach(Directory.GetFiles(AmmoFilePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path => 
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Ammo a = new Ammo(json, path.Replace(AmmoFilePath + "\\", "").Replace(".json", ""));
                    ammo.TryAdd(a.Id, a);
                }
                catch (Exception)
                {
                    #if !DEBUG
                    File.Delete(path);
                    #endif
                }
            });

            Parallel.ForEach(Directory.GetFiles(AttachmentFilePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path => 
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    if (json.subtype == "Magazine")
                    {
                        Magazine m = new Magazine(json, path.Replace(AttachmentFilePath + "\\", "").Replace(".json", ""), new Dictionary<string, object>(ammo));
                        magazines.TryAdd(m.Id, m);
                    }
                }
                catch (Exception)
                {
                    #if !DEBUG
                    File.Delete(path);
                    #endif
                }
            });

            ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();

            Parallel.ForEach(Directory.GetFiles(GunFilePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Gun g = new Gun(json, path.Replace(GunFilePath + "\\", "").Replace(".json", ""), new Dictionary<string, object>(magazines));
                    output.TryAdd(g.Id, g);
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
                new Column("Total Damage", "DamageTotal", true, true),
                new Column("Special Damage", "DamageSpecial", true, true, "N2"),
                new Column("Projectiles Per Shot", "ProjectilesPerShot", true, true),
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

        public static List<string[]> GetDownloadInfo(string FilePath)
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/WeaponPersonal", FilePath + "\\guns" }
                ,new string[] { "http://starcitizendb.com/api/ammo/energy", FilePath + "\\ammo" }
                ,new string[] { "http://starcitizendb.com/api/ammo/projectile", FilePath + "\\ammo" }
                ,new string[] { "http://starcitizendb.com/api/components/df/WeaponAttachment", FilePath + "\\attachments" }
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


        private static decimal TickSeconds = .01M;
        public static List<Series> Calculator(List<object> Data, int Ticks, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;
                    int dataCount = Data.Count;
                    if (g.SingleFirerate > 0)
                    {
                        Series s = g.GetNewSeries(g.Name + " Single Fire");
                        s.BorderDashStyle = ChartDashStyle.Dot;
                        decimal[] x = new decimal[Ticks + 1];
                        decimal[] y = new decimal[Ticks + 1];
                        for (int i = 0; i <= Ticks; i++)
                        {
                            x[i] = i * TickSeconds;
                            y[i] = g.DamageTotal + (g.DamageTotal * Math.Floor((i * TickSeconds) / (1M / g.SingleFirerate)));
                            if (options.CancellationToken.IsCancellationRequested)
                                loopState.Break();
                        }

                        s.Points.DataBindXY(x, y);
                        list.Enqueue(s);
                    }

                    if (g.RapidFirerate > 0)
                    {
                        Series s = g.GetNewSeries(g.Name + " Rapid Fire");
                        decimal[] x = new decimal[Ticks + 1];
                        decimal[] y = new decimal[Ticks + 1];
                        for (int i = 0; i <= Ticks; i++)
                        {
                            x[i] = i * TickSeconds;
                            y[i] = g.DamageTotal + (g.DamageTotal * Math.Floor((i * TickSeconds) / (1M / g.RapidFirerate)));
                            if (options.CancellationToken.IsCancellationRequested)
                                loopState.Break();
                        }

                        s.Points.DataBindXY(x, y);
                        list.Enqueue(s);
                    }

                    if (g.BurstFirerate > 0)
                    {
                        Series s = g.GetNewSeries(g.Name + " Burst Fire");
                        s.BorderDashStyle = ChartDashStyle.Dash;
                    }
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).Concat(DrawKillLines(Ticks)).ToList();
        }

        private static List<Series> DrawKillLines(int Ticks)
        {
            List<Series> output = new List<Series>();
            foreach (var bodyPart in new string[] { "Head", "Torso" })
            {
                Color color;
                decimal health = 0;
                if (bodyPart == "Head")
                {
                    health = 10M / 1.5M;
                    color = Color.Red;
                }
                else
                {
                    health = 20M;
                    color = Color.Yellow;
                }
                foreach (var armor in new string[] { "No Armor", "Light", "Medium", "Heavy" })
                {
                    switch (armor)
                    {
                        case "Light": health /= .8M; break;
                        case "Medium": health /= .7M; break;
                        case "Heavy": health /= .6M; break;
                        default: break;
                    }

                    decimal[] x = new decimal[Ticks + 1];
                    decimal[] y = new decimal[Ticks + 1];
                    for (int i = 0; i <= Ticks; i++)
                    {
                        x[i] = i * TickSeconds;
                        y[i] = health;
                    }

                    Series s = new Series(armor + " " + bodyPart);
                    s.ChartType = SeriesChartType.Line;
                    s.BorderWidth = 2;
                    s.MarkerStyle = MarkerStyle.Circle;
                    s.BorderDashStyle = ChartDashStyle.Solid;
                    s.IsValueShownAsLabel = false;
                    s.Color = color;
                    s.Points.DataBindXY(x, y);
                    output.Add(s);
                }
            }

            return output;
        }
    }

    public class Magazine : Item
    {
        public Ammo Ammo { get; set; }
        public int AmmoCount { get; set; }
        public decimal Weight { get; set; }

        public Magazine(dynamic Json, string File, Dictionary<string, object> Ammo)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;

            Weight = Convert.ToDecimal(Json.Components.SEntityPhysicsControllerParams.PhysType.SEntityRigidPhysicsControllerParams.Mass);

            this.Ammo = Ammo[Convert.ToString(Json.Components.SAmmoContainerComponentParams.ammoParamsRecord)];
            AmmoCount = (int)Json.Components.SAmmoContainerComponentParams.maxAmmoCount;
        }
    }

    public class Ammo : Item
    {
        public decimal Lifetime { get; set; }
        public int Speed { get; set; }
        public decimal DamageBiochemical { get; set; }
        public decimal DamageDistortion { get; set; }
        public decimal DamageEnergy { get; set; }
        public decimal DamagePhysical { get; set; }
        public decimal DamageThermal { get; set; }
        public decimal DamageTotal
        {
            get
            {
                return DamageBiochemical + DamageDistortion + DamageEnergy + DamagePhysical + DamageThermal;
            }
        }

        public Ammo(dynamic Json, string File)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;

            Speed = (int)Json.speed;
            Lifetime = (decimal)Json.lifetime;

            DamageBiochemical = Json.bullet.damage.DamageInfo.DamageBiochemical;
            DamageDistortion = Json.bullet.damage.DamageInfo.DamageDistortion;
            DamageEnergy = Json.bullet.damage.DamageInfo.DamageEnergy;
            DamagePhysical = Json.bullet.damage.DamageInfo.DamagePhysical;
            DamageThermal = Json.bullet.damage.DamageInfo.DamageThermal;
        }
    }
}
