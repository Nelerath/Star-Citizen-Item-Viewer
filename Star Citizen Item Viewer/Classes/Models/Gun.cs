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

        [ColumnData("Projectile Velocity", 22, true, true)]
        [RadarField]
        public int Speed
        {
            get
            {
                return Magazine.Ammo.Speed;
            }
        }
        [ColumnData("Max Range", 23, true, true, "N2")]
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
        public decimal DamageSpecial { get; set; }


        [ColumnData("Magazine Capacity", 10, true, true)]
        [RadarField]
        public int MagazineCapacity { get { return Magazine.AmmoCount; } }
        [ColumnData("Damage per Magazine", 11, true, true, "N2")]
        [RadarField]
        public decimal DamagePerMagazine { get { return DamageSpecial > 0 ? MagazineCapacity * DamageSpecial : MagazineCapacity * DamageTotal; } }

        [ColumnData("Weight", 24, true, false, "N2")]
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
        [ColumnData("Firerate", 12, true, true, "N2")]
        [RadarField]
        public decimal Firerate { get; set; }
        [ColumnData("Min Spread", 16, true, false, "N2")]
        [RadarField]
        public decimal MinSpread { get; set; }
        [ColumnData("Max Spread", 17, true, false, "N2")]
        [RadarField]
        public decimal MaxSpread { get; set; }
        [ColumnData("Initial Spread", 18, true, false, "N2")]
        [RadarField]
        public decimal InitialSpread { get; set; }
        [ColumnData("Spread Growth", 19, true, false, "N2")]
        [RadarField]
        public decimal SpreadGrowth { get; set; }
        [ColumnData("Spread Decay", 20, true, true, "N2")]
        [RadarField]
        public decimal SpreadDecay { get; set; }
        [ColumnData("Projectiles per Shot", 15, true, true)]
        [RadarField]
        public int ProjectilesPerShot { get; set; }

        public FireModeStats(decimal damage) { _damage = damage; }
        protected decimal _damage { get; set; }
        protected decimal Damage
        {
            get { return _damage * ProjectilesPerShot; }
        }

        [ColumnData("Damage per Second", 13, true, true, "N2")]
        [RadarField]
        public decimal DamagePerSecond { get { return Damage * Firerate; } }

        [ColumnData("Average Head TTK", 15, true, false, "N3")]
        [RadarField]
        public decimal AverageHeadTTK
        {
            get
            {
                decimal[] ttks = new decimal[4];
                int i = 0;
                decimal health = 10M;// / 1.5M;
                decimal armoredHealth = 0;
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
                return ttks.Average();
            }
        }
        [ColumnData("Average Body TTK", 16, true, false, "N3")]
        [RadarField]
        public decimal AverageBodyTTK
        {
            get
            {
                decimal[] ttks = new decimal[4];
                int i = 0;
                decimal health = 20;
                decimal armoredHealth = 0;
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
