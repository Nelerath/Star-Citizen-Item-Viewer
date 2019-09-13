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
        public decimal SingleMinSpread { get; set; }
        public decimal SingleMaxSpread { get; set; }
        public decimal SingleInitialSpread { get; set; }
        public decimal SingleSpreadGrowth { get; set; }
        public decimal SingleSpreadDecay { get; set; }

        public decimal BurstFirerate { get; set; }
        public decimal BurstMinSpread { get; set; }
        public decimal BurstMaxSpread { get; set; }
        public decimal BurstInitialSpread { get; set; }
        public decimal BurstSpreadGrowth { get; set; }
        public decimal BurstSpreadDecay { get; set; }

        public int BurstShotCount { get; set; }
        //public decimal BurstDuration { get; set; }
        public decimal BurstCooldown { get; set; }

        public decimal RapidFirerate { get; set; }
        public decimal RapidMinSpread { get; set; }
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
                SingleMinSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.min;
                SingleMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.max;
                SingleInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                SingleSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.attack;
                SingleSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.decay;
                ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.pelletCount ?? 1;
            }

            // These are completely broken and I hate CIG.
            //if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams != null)
            //{
            //    BurstFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.fireRate / 60M;
            //    BurstMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.max;
            //    BurstInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
            //    BurstSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.attack;
            //    BurstSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.decay;
            //    ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.pelletCount ?? 1;
            //    BurstShotCount = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.shotCount;
            //    BurstCooldown = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.cooldownTime;
            //}
            
            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams != null)
            {
                RapidFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.fireRate / 60M;
                RapidMinSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.min;
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

            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams != null && Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.name == "Burst")
            {
                BurstFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.fireRate / 60M;
                BurstMinSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.min;
                BurstMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.max;
                BurstInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                BurstSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.attack;
                BurstSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.decay;
                ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.pelletCount ?? 1;

                BurstShotCount = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.shotCount;
                //BurstDuration = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.cooldownTime;
                BurstCooldown = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.delay;
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
        public static List<Series> CreateLineGraphSeries(List<object> Data, int Ticks, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;

                    decimal x = 0M;
                    int shotsFired = 1;
                    
                    if (g.RapidFirerate > 0)
                    {
                        List<decimal> rapidX = new List<decimal>();
                        List<decimal> rapidY = new List<decimal>();
                        x = 0;
                        shotsFired = 1;
                        rapidX.Add(x);
                        rapidY.Add(shotsFired * g.DamageTotal);
                        while (x <= Ticks * TickSeconds)
                        {
                            x += (1 / g.RapidFirerate);
                            if (x <= Ticks * TickSeconds)
                            {
                                shotsFired++;
                                rapidX.Add(x);
                            }
                            else
                            {
                                rapidX.Add(Ticks * TickSeconds);
                            }
                            rapidY.Add(shotsFired * g.DamageTotal);
                        }

                        Series s = g.GetNewLineGraphSeries(g.Name + " Rapid");
                        s.Points.DataBindXY(rapidX.ToList(), rapidY.ToList());
                        list.Enqueue(s);
                    }

                    if (g.BurstFirerate > 0)
                    {
                        List<decimal> burstX = new List<decimal>();
                        List<decimal> burstY = new List<decimal>();
                        x = 0;
                        shotsFired = 0;
                        decimal q = 0;
                        for (int i = 0; i < g.BurstShotCount; i++)
                        {
                            q = x + (i * (1 / g.BurstFirerate));
                            if (q <= Ticks * TickSeconds)
                            {
                                shotsFired++;
                                burstX.Add(q);
                                burstY.Add(shotsFired * g.DamageTotal);
                            }
                            else
                                break;
                        }
                        x += g.BurstCooldown;
                        while (x <= Ticks * TickSeconds)
                        {
                            for (int i = 0; i < g.BurstShotCount; i++)
                            {
                                q = x + (i * (1 / g.BurstFirerate));
                                if (q <= Ticks * TickSeconds)
                                {
                                    shotsFired++;
                                    burstX.Add(q);
                                    burstY.Add(shotsFired * g.DamageTotal);
                                }
                                else
                                    break;
                            }
                            x += g.BurstCooldown;
                        }
                        burstX.Add(Ticks * TickSeconds);
                        burstY.Add(shotsFired * g.DamageTotal);

                        Series s = g.GetNewLineGraphSeries(g.Name + " Burst");
                        s.BorderDashStyle = ChartDashStyle.Dash;
                        s.Points.DataBindXY(burstX.ToList(), burstY.ToList());
                        list.Enqueue(s);
                    }

                    if (g.SingleFirerate > 0)
                    {
                        List<decimal> singleX = new List<decimal>();
                        List<decimal> singleY = new List<decimal>();
                        x = 0;
                        shotsFired = 1;
                        singleX.Add(x);
                        singleY.Add(shotsFired * g.DamageTotal);
                        while (x <= Ticks * TickSeconds)
                        {
                            x += (1 / g.SingleFirerate);
                            if (x <= Ticks * TickSeconds)
                            {
                                shotsFired++;
                                singleX.Add(x);
                            }
                            else
                            {
                                singleX.Add(Ticks * TickSeconds);
                            }
                            singleY.Add(shotsFired * g.DamageTotal);
                        }

                        Series s = g.GetNewLineGraphSeries(g.Name + " Single");
                        s.BorderDashStyle = ChartDashStyle.Dot;
                        s.Points.DataBindXY(singleX.ToList(), singleY.ToList());
                        list.Enqueue(s);
                    }
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).Concat(DrawKillLines(Ticks)).ToList();
        }

        public static List<Series> CreateRadarGraphSeries(List<object> Data, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            decimal weight = 99999999;
            decimal damage = 0;

            decimal firerate = 0;
            decimal spreadMin = 99999999;
            decimal spreadMax = 99999999;
            decimal spreadInitial = 99999999;

            decimal TTK = 9999;
            // Weight
            // Damage
            // Firerate
            // SpreadMin
            // SpreadMax
            // InitialSpread
            // TTK


            // Nice to haves
            // Recoil
            // TTK Unarmored Head
            // TTK Light Head
            // TTK Medium Head
            // TTK Heavy Head
            // TTK Unarmored Chest
            // TTK Light Chest
            // TTK Medium Chest
            // TTK Heavy Chest
            foreach (Gun item in Data)
            {
                weight = Math.Min(item.Weight, weight);
                damage = Math.Max(item.DamageTotal, damage);

                if (item.RapidFirerate > 0)
                {
                    firerate = Math.Max(item.RapidFirerate, firerate);

                    spreadMin = Math.Min(item.RapidMinSpread, spreadMin);
                    spreadMax = Math.Min(item.RapidMaxSpread, spreadMax);
                    spreadInitial = Math.Min(item.RapidInitialSpread, spreadInitial);

                    TTK = Math.Min(CalculateAverageTTK(item.DamageTotal, item.RapidFirerate), TTK);
                }

                // Burstfire is fucked
                //if (item.BurstFirerate > 0)
                //{
                //    firerate = Math.Max(item.BurstFirerate, firerate);
                //
                //    spreadMin = Math.Min(item.BurstMinSpread, spreadMin);
                //    spreadMax = Math.Min(item.BurstMaxSpread, spreadMax);
                //    spreadInitial = Math.Min(item.BurstInitialSpread, spreadInitial);
                //
                //    TTK = Math.Min(CalculateAverageTTK(item.DamageTotal, item.BurstFirerate), TTK);
                //}
                
                if (item.SingleFirerate > 0)
                {
                    firerate = Math.Max(item.SingleFirerate, firerate);
                
                    spreadMin = Math.Min(item.SingleMinSpread, spreadMin);
                    spreadMax = Math.Min(item.SingleMaxSpread, spreadMax);
                    spreadInitial = Math.Min(item.SingleInitialSpread, spreadInitial);

                    TTK = Math.Min(CalculateAverageTTK(item.DamageTotal, item.SingleFirerate), TTK);
                }
            }
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 1, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;

                    // Weight
                    // Damage
                    // Firerate
                    // SpreadMin
                    // SpreadMax
                    // InitialSpread
                    if (g.RapidFirerate > 0)
                    {
                        Series s = g.GetNewRadarGraphSeries(g.Name + " Rapid");
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.Weight, weight, false)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.DamageTotal , damage)));

                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.RapidFirerate , firerate)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.RapidMinSpread, spreadMin, false)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.RapidMaxSpread, spreadMax, false)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.RapidInitialSpread , spreadInitial, false)));

                        s.Points.Add(new DataPoint(0, Utility.GetRank(CalculateAverageTTK(g.DamageTotal, g.RapidFirerate), TTK, false)));

                        list.Enqueue(s);
                    }

                    // Burst is fucked
                    //if (g.BurstFirerate > 0)
                    //{
                    //    Series s = g.GetNewRadarGraphSeries(g.Name + " Burst");
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(g.Weight, weight, false)));
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(g.DamageTotal, damage)));
                    //
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(g.BurstFirerate, firerate)));
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(g.BurstMinSpread, spreadMin, false)));
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(g.BurstMaxSpread, spreadMax, false)));
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(g.BurstInitialSpread, spreadInitial, false)));
                    //
                    //    s.Points.Add(new DataPoint(0, Utility.GetRank(CalculateAverageTTK(g.DamageTotal, g.BurstFirerate), TTK, false)));
                    //
                    //    list.Enqueue(s);
                    //}

                    if (g.SingleFirerate > 0)
                    {
                        Series s = g.GetNewRadarGraphSeries(g.Name + " Single");
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.Weight, weight, false)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.DamageTotal, damage)));

                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.SingleFirerate, firerate)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.SingleMinSpread, spreadMin, false)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.SingleMaxSpread, spreadMax, false)));
                        s.Points.Add(new DataPoint(0, Utility.GetRank(g.SingleInitialSpread, spreadInitial, false)));

                        s.Points.Add(new DataPoint(0, Utility.GetRank(CalculateAverageTTK(g.DamageTotal, g.SingleFirerate), TTK, false)));

                        list.Enqueue(s);
                    }
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).ToList();
        }

        private static decimal CalculateAverageTTK(decimal Damage, decimal ShotsPerSecond)
        {
            decimal[] ttks = new decimal[8];
            int i = 0;
            foreach (var bodyPart in new string[] { "Head", "Torso" })
            {
                decimal health = 0;
                decimal armoredHealth = 0;
                if (bodyPart == "Head")
                    health = 10M / 1.5M;
                else
                    health = 20M;

                foreach (var armor in new string[] { "No Armor", "Light", "Medium", "Heavy" })
                {
                    switch (armor)
                    {
                        case "Light": armoredHealth = health / .8M; break;
                        case "Medium": armoredHealth = health / .7M; break;
                        case "Heavy": armoredHealth = health / .6M; break;
                        default: armoredHealth = health; break;
                    }

                    ttks[i] = (Math.Ceiling(armoredHealth / Damage)-1) / ShotsPerSecond;
                    i++;
                }
            }
            return ttks.Average();
        }

        public static List<CustomLabel> RadarLabels()
        {
            List<CustomLabel> output = new List<CustomLabel>();
            CustomLabel customLabel1 = new CustomLabel();
            CustomLabel customLabel2 = new CustomLabel();
            CustomLabel customLabel3 = new CustomLabel();
            CustomLabel customLabel4 = new CustomLabel();
            CustomLabel customLabel5 = new CustomLabel();
            CustomLabel customLabel6 = new CustomLabel();
            CustomLabel customLabel7 = new CustomLabel();
            //CustomLabel customLabel8 = new CustomLabel();
            customLabel1.ForeColor = System.Drawing.Color.White;
            customLabel1.Text = "Weight";
            customLabel2.ForeColor = System.Drawing.Color.White;
            customLabel2.Text = "Damage";
            customLabel3.ForeColor = System.Drawing.Color.White;
            customLabel3.Text = "Firerate";
            customLabel4.ForeColor = System.Drawing.Color.White;
            customLabel4.Text = "Minimum Spread";
            customLabel5.ForeColor = System.Drawing.Color.White;
            customLabel5.Text = "Maximum Spread";
            customLabel6.ForeColor = System.Drawing.Color.White;
            customLabel6.Text = "Initial Spread";
            customLabel7.ForeColor = System.Drawing.Color.White;
            customLabel7.Text = "TTK";
            output.Add(customLabel1);
            output.Add(customLabel2);
            output.Add(customLabel3);
            output.Add(customLabel4);
            output.Add(customLabel5);
            output.Add(customLabel6);
            output.Add(customLabel7);
            return output;
        }

        private static List<Series> DrawKillLines(int Ticks)
        {
            List<Series> output = new List<Series>();
            foreach (var bodyPart in new string[] { "Head", "Torso" })
            {
                Color color;
                decimal health = 0;
                decimal armoredHealth = 0;
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
                        case "Light": armoredHealth = health/.8M; break;
                        case "Medium": armoredHealth = health / .7M; break;
                        case "Heavy": armoredHealth = health / .6M; break;
                        default: armoredHealth = health; break;
                    }

                    decimal[] x = new decimal[2] { 0, Ticks * TickSeconds };
                    decimal[] y = new decimal[2] { armoredHealth, armoredHealth };

                    Series s = new Series(armor + " " + bodyPart);
                    s.ChartType = SeriesChartType.Line;
                    s.BorderWidth = 1;
                    s.MarkerStyle = MarkerStyle.None;
                    s.BorderDashStyle = ChartDashStyle.Solid;
                    s.IsValueShownAsLabel = false;
                    s.IsVisibleInLegend = false;
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
