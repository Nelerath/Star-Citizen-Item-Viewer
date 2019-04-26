using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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

        public decimal DamageBiochemical { get { return Magazine.Ammo.DamageBiochemical; } }
        public decimal DamageDistortion { get { return Magazine.Ammo.DamageDistortion; } }
        public decimal DamageEnergy { get { return Magazine.Ammo.DamageEnergy; } }
        public decimal DamagePhysical { get { return Magazine.Ammo.DamagePhysical; } }
        public decimal DamageThermal { get { return Magazine.Ammo.DamageThermal; } }
        public decimal DamageTotal { get { return Magazine.Ammo.DamageTotal; } }

        public Gun(dynamic Json, string File, Dictionary<string, object> Magazines)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;

            ADSTime = Json.Components.SCItemWeaponComponentParams.aimAction.SWeaponActionAimSimpleParams.zoomInTime;

            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams != null)
            {
                SingleFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.fireRate / 60M;
                SingleMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.projectileLaunchParams.spreadParams.max;
                SingleInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.projectileLaunchParams.spreadParams.firstAttack;
                SingleSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.projectileLaunchParams.spreadParams.attack;
                SingleSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireSingleParams.Single.projectileLaunchParams.spreadParams.decay;
            }

            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams != null)
            {
                BurstFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.fireRate / 60M;
                BurstMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.projectileLaunchParams.spreadParams.max;
                BurstInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.projectileLaunchParams.spreadParams.firstAttack;
                BurstSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.projectileLaunchParams.spreadParams.attack;
                BurstSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireBurstParams.projectileLaunchParams.spreadParams.decay;
            }
            
            if (Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams != null)
            {
                RapidFirerate = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.fireRate / 60M;
                RapidMaxSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.projectileLaunchParams.spreadParams.max;
                RapidInitialSpread = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.projectileLaunchParams.spreadParams.firstAttack;
                RapidSpreadGrowth = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.projectileLaunchParams.spreadParams.attack;
                RapidSpreadDecay = Json.Components.SCItemWeaponComponentParams.fireActions.SWeaponActionFireRapidParams.projectileLaunchParams.spreadParams.decay;
            }

            Magazine = Magazines[Convert.ToString(Json.Components.SCItemWeaponComponentParams.ammoContainerRecord)];

            /*
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
            */
        }

        public static Dictionary<string, object> parseAll(string GunFilePath, string AttachmentFilePath, string AmmoFilePath)
        {
            Dictionary<string, object> magazines = new Dictionary<string, object>();
            Dictionary<string, object> ammo = new Dictionary<string, object>();
            foreach (var path in Directory.GetFiles(AmmoFilePath))
            {
                string raw = File.ReadAllText(path).Replace("@", "");
                dynamic json = JsonConvert.DeserializeObject(raw);
                Ammo a = new Ammo(json, path.Replace(AmmoFilePath + "\\", "").Replace(".json", ""));
                ammo.Add(a.Id, a);
            }

            foreach (var path in Directory.GetFiles(AttachmentFilePath))
            {
                string raw = File.ReadAllText(path).Replace("@", "");
                dynamic json = JsonConvert.DeserializeObject(raw);
                if (json.subtype == "Magazine")
                {
                    Magazine m = new Magazine(json, path.Replace(AttachmentFilePath + "\\", "").Replace(".json", ""), ammo);
                    magazines.Add(m.Id, m);
                }
            }
            Dictionary<string, object> output = new Dictionary<string, object>();
            foreach (var path in Directory.GetFiles(GunFilePath))
            {
                //string raw = File.ReadAllText(path).Replace("@", "");
                //dynamic json = JsonConvert.DeserializeObject(raw);
                //Gun g = new Gun(json, path.Replace(GunFilePath + "\\", "").Replace(".json", ""), magazines);
                //output.Add(g.Id, g);

                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Gun g = new Gun(json, path.Replace(GunFilePath + "\\", "").Replace(".json", ""), magazines);
                    output.Add(g.Id, g);
                }
                catch (Exception ex)
                {
                }
            }
            return output;
        }
    }

    public class Magazine : Item
    {
        public Ammo Ammo { get; set; }
        public int AmmoCount { get; set; }

        public Magazine(dynamic Json, string File, Dictionary<string, object> Ammo)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;

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
