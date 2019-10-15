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
        #region File Path
        public static string GunPath
        {
            get
            {
                return $"{_filePath}\\guns";
            }
        }
        public static string AmmoPath
        {
            get
            {
                return $"{_filePath}\\ammo";
            }
        }
        public static string AttachmentPath
        {
            get
            {
                return $"{_filePath}\\attachments";
            }
        }
        #endregion

        public decimal ADSTime { get; set; }

        public FireModeStats Rapid { get; set; }
        public FireModeStats Single { get; set; }
        public BurstFireModeStats Burst { get; set; }

        

        private Magazine Magazine { get; set; }

        [ColumnData("Projectile Velocity", 19, true, true)]
        [RadarField]
        public int Speed
        {
            get
            {
                return Magazine.Ammo.Speed;
            }
        }
        [ColumnData("Max Range", 20, true, true, "N2")]
        [RadarField]
        public decimal MaxRange
        {
            get
            {
                return Magazine.Ammo.Speed * Magazine.Ammo.Lifetime;
            }
        }

        [ColumnData("Damage Biochemical", 5, true, true, "N2")]
        [RadarField]
        public decimal DamageBiochemical { get { return Magazine.Ammo.DamageBiochemical; } }
        [ColumnData("Damage Distortion", 6, true, true, "N2")]
        [RadarField]
        public decimal DamageDistortion { get { return Magazine.Ammo.DamageDistortion; } }
        [ColumnData("Damage Energy", 7, true, true, "N2")]
        [RadarField]
        public decimal DamageEnergy { get { return Magazine.Ammo.DamageEnergy; } }
        [ColumnData("Damage Physical", 8, true, true, "N2")]
        [RadarField]
        public decimal DamagePhysical { get { return Magazine.Ammo.DamagePhysical; } }
        [ColumnData("Damage Thermal", 9, true, true, "N2")]
        [RadarField]
        public decimal DamageThermal { get { return Magazine.Ammo.DamageThermal; } }
        [ColumnData("Damage", 3, true, true, "N2")]
        [RadarField]
        public decimal DamageTotal { get { return Magazine.Ammo.DamageTotal; } }
        [ColumnData("Damage Special", 4, true, true, "N2")]
        [RadarField]
        public decimal? DamageSpecial { get; set; }

        [ColumnData("Weight", 21, true, false, "N2")]
        public decimal Weight { get; set; }

        public Gun(dynamic json, string file, Dictionary<string, object> magazines)
        {
            Id = json.__ref;
            Name = string.IsNullOrEmpty((string)json.name_local) ? file : json.name_local;
            Size = json.size;
            Filename = file;
            Type = Types.Gun;

            Rapid = null;
            Single = null;
            Burst = null;

            Magazine = magazines[Convert.ToString(json.Components.SCItemWeaponComponentParams.ammoContainerRecord)];
            //ADSTime = Json.Components.SCItemWeaponComponentParams.aimAction.SWeaponActionAimSimpleParams.zoomInTime;

            if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams != null)
            {
                Single = new FireModeStats(DamageTotal);
                
                if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single != null)
                {
                    Single.Firerate = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.fireRate / 60M;
                    Single.MinSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.min;
                    Single.MaxSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.max;
                    Single.InitialSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                    Single.SpreadGrowth = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.attack;
                    Single.SpreadDecay = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.spreadParams.decay;
                    Single.ProjectilesPerShot = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.launchParams.SProjectileLauncher.pelletCount ?? 1;
                }
            }
            else if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams != null) // Retarded energy shotgun
            {
                if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams != null)
                {
                    Single = new FireModeStats(DamageTotal);
                    Single.Firerate = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.fireRate / 60M;
                    Single.MinSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.launchParams.SProjectileLauncher.spreadParams.min;
                    Single.MaxSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.launchParams.SProjectileLauncher.spreadParams.max;
                    Single.InitialSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                    Single.SpreadGrowth = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.launchParams.SProjectileLauncher.spreadParams.attack;
                    Single.SpreadDecay = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.launchParams.SProjectileLauncher.spreadParams.decay;
                    Single.ProjectilesPerShot = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.weaponAction.SWeaponActionFireSingleParams.launchParams.SProjectileLauncher.pelletCount ?? 1;
                }
            }
            
            if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams != null)
            {
                Rapid = new FireModeStats(DamageTotal);
                Rapid.Firerate = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.fireRate / 60M;
                Rapid.MinSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.min;
                Rapid.MaxSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.max;
                Rapid.InitialSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                Rapid.SpreadGrowth = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.attack;
                Rapid.SpreadDecay = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.spreadParams.decay;
                Rapid.ProjectilesPerShot = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.launchParams.SProjectileLauncher.pelletCount ?? 1;
            }

            if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams != null)
            {
                DamageSpecial = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireChargedParams.maxChargeModifier.damageMultiplier * DamageTotal;
            }
            else
            {
                DamageSpecial = null;
            }

            if (json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams != null && json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.name == "Burst")
            {
                Burst = new BurstFireModeStats(DamageTotal);
                Burst.Firerate = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.fireRate / 60M;
                Burst.MinSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.min;
                Burst.MaxSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.max;
                Burst.InitialSpread = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.firstAttack;
                Burst.SpreadGrowth = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.attack;
                Burst.SpreadDecay = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.spreadParams.decay;
                Burst.ProjectilesPerShot = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.launchParams.SProjectileLauncher.pelletCount ?? 1;

                Burst.ShotCount = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.shotCount;
                //Burst.BurstDuration = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.weaponAction.SWeaponActionFireBurstParams.cooldownTime;
                Burst.BurstCooldown = json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionSequenceParams.sequenceEntries.SWeaponSequenceEntryParams.delay;
            }

            Weight = Convert.ToDecimal(json.Components.SEntityPhysicsControllerParams.PhysType.SEntityRigidPhysicsControllerParams.Mass) + Magazine.Weight;
        }

        public static Dictionary<string, object> parseAll()
        {
            ConcurrentDictionary<string, object> magazines = new ConcurrentDictionary<string, object>();
            ConcurrentDictionary<string, object> ammo = new ConcurrentDictionary<string, object>();

            Parallel.ForEach(Directory.GetFiles(AmmoPath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path => 
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Ammo a = new Ammo(json, path.Replace(AmmoPath + "\\", "").Replace(".json", ""));
                    ammo.TryAdd(a.Id, a);
                }
                catch (Exception)
                {
                    #if !DEBUG
                    File.Delete(path);
                    #endif
                }
            });

            Parallel.ForEach(Directory.GetFiles(AttachmentPath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path => 
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    if (json.subtype == "Magazine")
                    {
                        Magazine m = new Magazine(json, path.Replace(AttachmentPath + "\\", "").Replace(".json", ""), new Dictionary<string, object>(ammo));
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

            Parallel.ForEach(Directory.GetFiles(GunPath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Gun g = new Gun(json, path.Replace(GunPath + "\\", "").Replace(".json", ""), new Dictionary<string, object>(magazines));
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

        private static decimal TickSeconds = .01M;
        public static List<Series> CreateLineGraphSeries(List<object> data, int ticks, CancellationToken token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 2, CancellationToken = token };
                Parallel.ForEach(data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;

                    decimal x = 0M;
                    int shotsFired = 1;
                    
                    if (g.Rapid != null)
                    {
                        List<decimal> rapidX = new List<decimal>();
                        List<decimal> rapidY = new List<decimal>();
                        x = 0;
                        shotsFired = 1;
                        rapidX.Add(x);
                        rapidY.Add(shotsFired * g.DamageTotal);
                        while (x <= ticks * TickSeconds)
                        {
                            x += (1 / g.Rapid.Firerate);
                            if (x <= ticks * TickSeconds)
                            {
                                shotsFired++;
                                rapidX.Add(x);
                            }
                            else
                            {
                                rapidX.Add(ticks * TickSeconds);
                            }
                            rapidY.Add(shotsFired * g.DamageTotal);
                        }

                        Series s = g.GetNewLineGraphSeries(g.Name + " Rapid");
                        s.Points.DataBindXY(rapidX.ToList(), rapidY.ToList());
                        list.Enqueue(s);
                    }

                    if (g.Burst != null)
                    {
                        List<decimal> burstX = new List<decimal>();
                        List<decimal> burstY = new List<decimal>();
                        x = 0;
                        shotsFired = 0;
                        decimal q = 0;
                        for (int i = 0; i < g.Burst.ShotCount; i++)
                        {
                            q = x + (i * (1 / g.Burst.Firerate));
                            if (q <= ticks * TickSeconds)
                            {
                                shotsFired++;
                                burstX.Add(q);
                                burstY.Add(shotsFired * g.DamageTotal);
                            }
                            else
                                break;
                        }
                        x += g.Burst.BurstCooldown;
                        while (x <= ticks * TickSeconds)
                        {
                            for (int i = 0; i < g.Burst.ShotCount; i++)
                            {
                                q = x + (i * (1 / g.Burst.Firerate));
                                if (q <= ticks * TickSeconds)
                                {
                                    shotsFired++;
                                    burstX.Add(q);
                                    burstY.Add(shotsFired * g.DamageTotal);
                                }
                                else
                                    break;
                            }
                            x += g.Burst.BurstCooldown;
                        }
                        burstX.Add(ticks * TickSeconds);
                        burstY.Add(shotsFired * g.DamageTotal);

                        Series s = g.GetNewLineGraphSeries(g.Name + " Burst");
                        s.BorderDashStyle = ChartDashStyle.Dash;
                        s.Points.DataBindXY(burstX.ToList(), burstY.ToList());
                        list.Enqueue(s);
                    }

                    if (g.Single != null)
                    {
                        List<decimal> singleX = new List<decimal>();
                        List<decimal> singleY = new List<decimal>();
                        x = 0;
                        shotsFired = 1;
                        singleX.Add(x);
                        singleY.Add(shotsFired * g.DamageTotal);
                        while (x <= ticks * TickSeconds)
                        {
                            x += (1 / g.Single.Firerate);
                            if (x <= ticks * TickSeconds)
                            {
                                shotsFired++;
                                singleX.Add(x);
                            }
                            else
                            {
                                singleX.Add(ticks * TickSeconds);
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
            return new List<Series>(list).OrderBy(x => x.Name).Concat(DrawKillLines(ticks)).ToList();
        }

        private static List<Series> DrawKillLines(int ticks)
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

                    decimal[] x = new decimal[2] { 0, ticks * TickSeconds };
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

    public class BurstFireModeStats : FireModeStats
    {
        [ColumnData("Shot Count", 10, true, true)]
        [RadarField]
        public int ShotCount { get; set; }
        //public decimal BurstDuration { get; set; }
        public decimal BurstCooldown { get; set; }
        public BurstFireModeStats(decimal damage) : base(damage) {  }
    }

    public class FireModeStats
    {
        [ColumnData("Firerate", 10, true, true, "N2")]
        [RadarField]
        public decimal Firerate { get; set; }
        [ColumnData("Min Spread", 14, true, false, "N2")]
        [RadarField]
        public decimal MinSpread { get; set; }
        [ColumnData("Max Spread", 15, true, false, "N2")]
        [RadarField]
        public decimal MaxSpread { get; set; }
        [ColumnData("Initial Spread", 16, true, false, "N2")]
        [RadarField]
        public decimal InitialSpread { get; set; }
        [ColumnData("Spread Growth", 17, true, false, "N2")]
        [RadarField]
        public decimal SpreadGrowth { get; set; }
        [ColumnData("Spread Decay", 18, true, true, "N2")]
        [RadarField]
        public decimal SpreadDecay { get; set; }
        [ColumnData("Projectiles per Shot", 13, true, true)]
        [RadarField]
        public int ProjectilesPerShot { get; set; }

        public FireModeStats(decimal damage) { _damage = damage; }
        protected decimal _damage { get; set; }
        protected decimal Damage
        {
            get { return _damage * ProjectilesPerShot; }
        }

        [ColumnData("Damage per Second", 11, true, true, "N2")]
        [RadarField]
        public decimal DamagePerSecond { get { return Damage * Firerate; } }

        [ColumnData("Average TTK", 12, true, true, "N2")]
        [RadarField]
        public decimal AverageTTK
        {
            get
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

                        ttks[i] = (Math.Ceiling(armoredHealth / Damage) - 1) / Firerate;
                        i++;
                    }
                }
                return ttks.Average();
            }
        }
    }

    class Magazine : Item
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

    class Ammo : Item
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
