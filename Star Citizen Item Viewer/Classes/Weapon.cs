using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Weapon : Item
    {
        public decimal Firerate { get; set; }

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
        public int DamageBiochemical {get;set;}
        public int DamageDistortion { get; set; }
        public int DamageEnergy { get; set; }
        public int DamagePhysical { get; set; }
        public int DamageThermal { get; set; }
        public int DamageTotal {
            get
            {
                return DamageBiochemical + DamageDistortion + DamageEnergy + DamagePhysical + DamageThermal;
            }
        }
        public decimal DamagePerSecond
        {
            get
            {
                return DamageTotal * Firerate;
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

        public Weapon(dynamic Json, string File)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
                
            Size = Json.size;
            Filename = File;

            Firerate = Json.Components.SCItemWeaponComponentParams.fire.fireRate / 60M;
            HeatPerShot = Json.Components.SCItemWeaponComponentParams.fire.heatPerShot;
            MaximumTemperature = Json.Components.EntityComponentHeatConnection.MaximumTemperature;

            PowerBase = Json.Components.EntityComponentPowerConnection.PowerBase;
            PowerDraw = Json.Components.EntityComponentPowerConnection.PowerDraw;

            MaxSpread = Json.Components.SCItemWeaponComponentParams.fire.projectileLaunchParams.spreadParams.max;
            InitialSpread = Json.Components.SCItemWeaponComponentParams.fire.projectileLaunchParams.spreadParams.firstAttack;
            SpreadGrowth = Json.Components.SCItemWeaponComponentParams.fire.projectileLaunchParams.spreadParams.attack;
            SpreadDecay = Json.Components.SCItemWeaponComponentParams.fire.projectileLaunchParams.spreadParams.decay;

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
        }

        public static Dictionary<string,object> parseAll(string filePath)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            foreach (var path in Directory.GetFiles(filePath))
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Weapon w = new Weapon(json, path.Replace(filePath + "\\", "").Replace(".json", ""));
                    output.Add(w.Id, w);
                }
                catch (Exception ex)
                {
                }
            }
            return output;
        }
    }
}
