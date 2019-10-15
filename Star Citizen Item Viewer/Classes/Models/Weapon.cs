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
        #region File Path
        public static string Filepath
        {
            get
            {
                return $"{_filePath}\\weapons";
            }
        }
        #endregion

        private decimal _firerate { get; set; }
        [ColumnData("Firerate", 6, true, true, "N2")]
        [RadarField]
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
        [ColumnData("Projectiles per Shot", 7, true, true)]
        [RadarField]
        public int ProjectilesPerShot { get; set; }

        public int MaximumTemperature { get; set; }
        [ColumnData("Overheat Temperature", 15, true, true)]
        [RadarField]
        public int OverheatTemperature { get; set; }
        public int RecoveryTemperature { get; set; }
        public int StartCoolingTemperature { get; set; }
        [ColumnData("Heat per Shot", 13, true, false, "N2")]
        public decimal HeatPerShot { get; set; }
        [ColumnData("Max Cooling Rate", 14, true, true, "N2")]
        [RadarField]
        public decimal MaxCoolingRate { get; set; }
        public decimal HeatPerSecond
        {
            get
            {
                return (HeatPerShot * Firerate) - MaxCoolingRate;
            }
        }
        public decimal HeatUptime
        {
            get
            {
                return HeatPerSecond > 0 ? (MaximumTemperature - OverheatTemperature) / HeatPerSecond : 9999;
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
        [ColumnData("Projectile Velocity", 16, true, true)]
        [RadarField]
        public int Speed { get; set; }
        private decimal _damageBiochemical { get; set; }
        private decimal _damageDistortion { get; set; }
        private decimal _damageEnergy { get; set; }
        private decimal _damagePhysical { get; set; }
        private decimal _damageThermal { get; set; }
        [ColumnData("Biochemical Damage", 8, true, true, "N2")]
        [RadarField]
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
        [ColumnData("Distortion Damage", 9, true, true, "N2")]
        [RadarField]
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
        [ColumnData("Energy Damage", 10, true, true, "N2")]
        [RadarField]
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
        [ColumnData("Physical Damage", 11, true, true, "N2")]
        [RadarField]
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
        [ColumnData("Thermal Damage", 12, true, true, "N2")]
        [RadarField]
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
        [ColumnData("Damage", 3, true, true, "N2")]
        [RadarField]
        public decimal DamageTotal {
            get
            {
                return DamageBiochemical + DamageDistortion + DamageEnergy + DamagePhysical + DamageThermal;
            }
        }
        [ColumnData("Special Damage", 4, true, true, "N2")]
        [RadarField]
        public decimal DamageSpecial
        {
            get
            {
                return DamageTotal * ProjectilesPerShot;
            }
        }
        [ColumnData("Damage per Second", 5, true, true, "N2")]
        [RadarField]
        public decimal DamagePerSecond
        {
            get
            {
                return DamageSpecial * Firerate;
            }
        }
        public decimal DamagePerSecondSpecial
        {
            get
            {
                return DamageSpecial * Firerate;
            }
        }
        [ColumnData("Max Range", 17, true, true)]
        [RadarField]
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
        [ColumnData("Damage per Heat", 12, true, true, "N2")]
        [RadarField]
        public decimal DamagePerHeat
        {
            get
            {
                return DamageTotal / HeatPerShot;
            }
        }

        [ColumnData("Min Spread", 18, true, false, "N3")]
        [RadarField]
        public decimal MinSpread { get; set; }
        [ColumnData("Max Spread", 19, true, false, "N3")]
        [RadarField]
        public decimal MaxSpread { get; set; }
        [ColumnData("Initial Spread", 20, true, false, "N3")]
        [RadarField]
        public decimal InitialSpread { get; set; }
        [ColumnData("Spread Growth", 21, true, false, "N3")]
        [RadarField]
        public decimal SpreadGrowth { get; set; }
        [ColumnData("Spread Decay", 22, true, true, "N3")]
        [RadarField]
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
                return SpreadPerSecond != 0 && (MaxSpread-MinSpread) / SpreadPerSecond >= 0 ? (MaxSpread - MinSpread) / SpreadPerSecond : 999;
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

        [ColumnData("3 Second Damage", 100, true, true, "N2", false)]
        [RadarField]
        public decimal ThreeSecondDamge
        {
            get
            {
                return DamageSpecial + (DamageSpecial * Math.Floor(3 / (1 / Firerate)));
            }
        }
        [ColumnData("10 Second Damage", 100, true, true, "N2", false)]
        [RadarField]
        public decimal TenSecondDamage
        {
            get
            {
                return DamageSpecial + (DamageSpecial * Math.Floor(10 / (1 / Firerate)));
            }
        }
        [ColumnData("60 Second Damage", 100, true, true, "N2", false)]
        [RadarField]
        public decimal SixtySecondDamage
        {
            get
            {
                return DamageSpecial + (DamageSpecial * Math.Floor(60 / (1 / Firerate)));
            }
        }

        public Weapon(dynamic json, string file)
        {
            Id = json.__ref;
            Name = string.IsNullOrEmpty((string)json.name_local) ? file : json.name_local;
            Size = json.size;
            Filename = file;
            Type = Types.Weapon;

            Firerate = json.Components.SCItemWeaponComponentParams.fire.fireRate / 60M;
            ProjectilesPerShot = json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.pelletCount ?? 1;
            HeatPerShot = json.Components.SCItemWeaponComponentParams.fire.heatPerShot;
            

            PowerBase = json.Components.EntityComponentPowerConnection.PowerBase;
            PowerDraw = json.Components.EntityComponentPowerConnection.PowerDraw;

            MinSpread = json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.min;
            MaxSpread = json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.max;
            InitialSpread = json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.firstAttack;
            SpreadGrowth = json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.attack;
            SpreadDecay = json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.decay;

            Lifetime = json.ammo.lifetime;
            Speed = json.ammo.speed;
            DamageBiochemical = json.ammo.bullet.damage.DamageInfo.DamageBiochemical;
            DamageDistortion = json.ammo.bullet.damage.DamageInfo.DamageDistortion;
            DamageEnergy = json.ammo.bullet.damage.DamageInfo.DamageEnergy;
            DamagePhysical = json.ammo.bullet.damage.DamageInfo.DamagePhysical;
            DamageThermal = json.ammo.bullet.damage.DamageInfo.DamageThermal;

            // Explosive ammo
            DamageBiochemical += json.ammo.bullet.detonation != null ? (int)json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageBiochemical : 0;
            DamageDistortion += json.ammo.bullet.detonation != null ? (int)json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageDistortion : 0;
            DamageEnergy += json.ammo.bullet.detonation != null ? (int)json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageEnergy : 0;
            DamagePhysical += json.ammo.bullet.detonation != null ? (int)json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamagePhysical : 0;
            DamageThermal += json.ammo.bullet.detonation != null ? (int)json.ammo.bullet.detonation.explosion.damage.DamageInfo.DamageThermal : 0;

            OverclockFirerateMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.fireRateMultiplier;
            OverclockDamageMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.damageMultiplier;
            OverclockAmmoCostMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.ammoCostMultiplier;
            OverclockHeatMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overclockStats.heatGenerationMultiplier;

            OverpowerFirerateMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.fireRateMultiplier;
            OverpowerDamageMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.damageMultiplier;
            OverpowerAmmoCostMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.ammoCostMultiplier;
            OverpowerHeatMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.overpowerStats.heatGenerationMultiplier;

            HeatFirerateMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.fireRateMultiplier;
            HeatDamageMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.damageMultiplier;
            HeatAmmoCostMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.ammoCostMultiplier;
            HeatHeatMultiplier = json.Components.SCItemWeaponComponentParams.connectionParams.heatStats.heatGenerationMultiplier;

            MaximumTemperature = json.Components.EntityComponentHeatConnection.MaxTemperature;
            MaxCoolingRate = json.Components.EntityComponentHeatConnection.MaxCoolingRate;
            OverheatTemperature = json.Components.EntityComponentHeatConnection.OverheatTemperature;
            RecoveryTemperature = json.Components.EntityComponentHeatConnection.RecoveryTemperature;
            StartCoolingTemperature = json.Components.EntityComponentHeatConnection.StartCoolingTemperature;
        }

        public static Dictionary<string,object> parseAll()
        {
            ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();
            Parallel.ForEach(Directory.GetFiles(Filepath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Weapon w = new Weapon(json, path.Replace(Filepath + "\\", "").Replace(".json", ""));
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

        public static List<Series> CreateLineGraphSeries(List<object> data, int ticks, CancellationToken token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 5, CancellationToken = token };
                Parallel.ForEach(data, options, (item, loopState) =>
                {
                    Weapon w = (Weapon)item;
                    Series s = w.GetNewLineGraphSeries();

                    decimal[] x = new decimal[ticks + 1];
                    decimal[] y = new decimal[ticks + 1];
                    decimal firerate = Convert.ToDecimal(item.Get("Firerate"));
                    for (int i = 0; i <= ticks; i++)
                    {
                        x[i] = i * .05M;
                        y[i] = w.DamageSpecial + (w.DamageSpecial * Math.Floor((i * .05M) / (1 / w.Firerate)));
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
            customLabel1.ForeColor = System.Drawing.Color.White;
            customLabel1.Text = "Projectile Speed";
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
            output.Add(customLabel1);
            output.Add(customLabel2);
            output.Add(customLabel3);
            output.Add(customLabel4);
            output.Add(customLabel5);
            output.Add(customLabel6);
            output.Add(customLabel7);
            return output;
        }
    }
}
