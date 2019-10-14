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
        public int OverheatTemperature { get; set; }
        public int RecoveryTemperature { get; set; }
        public int StartCoolingTemperature { get; set; }
        public decimal HeatPerShot { get; set; }
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

        public decimal MinSpread { get; set; }
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

        public decimal ThreeSecondDamge
        {
            get
            {
                return DamageSpecial + (DamageSpecial * Math.Floor(3 / (1 / Firerate)));
            }
        }
        public decimal TenSecondDamage
        {
            get
            {
                return DamageSpecial + (DamageSpecial * Math.Floor(10 / (1 / Firerate)));
            }
        }
        public decimal SixtySecondDamage
        {
            get
            {
                return DamageSpecial + (DamageSpecial * Math.Floor(60 / (1 / Firerate)));
            }
        }

        public Weapon(dynamic Json, string File)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;
            Type = Types.Weapon;

            Firerate = Json.Components.SCItemWeaponComponentParams.fire.fireRate / 60M;
            ProjectilesPerShot = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.pelletCount ?? 1;
            HeatPerShot = Json.Components.SCItemWeaponComponentParams.fire.heatPerShot;
            

            PowerBase = Json.Components.EntityComponentPowerConnection.PowerBase;
            PowerDraw = Json.Components.EntityComponentPowerConnection.PowerDraw;

            MinSpread = Json.Components.SCItemWeaponComponentParams.fire.launchParams.SProjectileLauncher.spreadParams.min;
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

            MaximumTemperature = Json.Components.EntityComponentHeatConnection.MaxTemperature;
            MaxCoolingRate = Json.Components.EntityComponentHeatConnection.MaxCoolingRate;
            OverheatTemperature = Json.Components.EntityComponentHeatConnection.OverheatTemperature;
            RecoveryTemperature = Json.Components.EntityComponentHeatConnection.RecoveryTemperature;
            StartCoolingTemperature = Json.Components.EntityComponentHeatConnection.StartCoolingTemperature;
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

        public static List<Series> CreateLineGraphSeries(List<object> Data, int Ticks, CancellationToken Token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 5, CancellationToken = Token };
                Parallel.ForEach(Data, options, (item, loopState) =>
                {
                    Weapon w = (Weapon)item;
                    Series s = w.GetNewLineGraphSeries();

                    decimal[] x = new decimal[Ticks + 1];
                    decimal[] y = new decimal[Ticks + 1];
                    decimal firerate = Convert.ToDecimal(item.Get("Firerate"));
                    for (int i = 0; i <= Ticks; i++)
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
