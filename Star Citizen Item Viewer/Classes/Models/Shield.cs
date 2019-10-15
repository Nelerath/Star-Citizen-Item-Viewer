﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Shield : Item
    {
        #region File Path
        public static string Filepath
        {
            get
            {
                return $"{_filePath}\\shields";
            }
        }
        #endregion

        [ColumnData("Damage Regen Delay", 5, true, false, "N2")]
        [RadarField]
        public decimal DamagedRegenDelay { get; set; }
        public decimal DecayRatio { get; set; }
        [ColumnData("Downed Regen Delay", 6, true, false, "N2")]
        [RadarField]
        public decimal DownedRegenDelay { get; set; }
        public decimal MaxReallocation { get; set; }
        [ColumnData("Max Shield Health", 3, true, true)]
        [RadarField]
        public int MaxShieldHealth { get; set; }
        [ColumnData("Shield Regen", 4, true, true, "N2")]
        [RadarField]
        public decimal MaxShieldRegen { get; set; }
        public decimal ReallocationRate { get; set; }

        [ColumnData("Hardening Cooldown", 8, true, false, "N2")]
        public decimal Cooldown { get; set; }
        [ColumnData("Hardening Duration", 7, true, true, "N2")]
        public decimal Duration { get; set; }
        [ColumnData("Hardening Factor", 9, true, true, "N2")]
        public decimal Factor { get; set; }

        [ColumnData("Power Base", 10, true, false, "N2")]
        [RadarField]
        public decimal PowerBase { get; set; }
        [ColumnData("Power Draw", 11, true, false, "N2")]
        [RadarField]
        public decimal PowerDraw { get; set; }
        [ColumnData("Power to EM", 12, true, false, "N2")]
        [RadarField]
        public decimal PowerToEM { get; set; }
        [ColumnData("Decay Rate of EM", 14, true, true, "N2")]
        [RadarField]
        public decimal DecayRateOfEM { get; set; }
        [ColumnData("EM Signature", 13, true, false, "N2")]
        [RadarField]
        public decimal EM
        {
            get { return (PowerBase + PowerDraw) * PowerToEM; }
        }

        public decimal ThermalEnergyBase { get; set; }
        public decimal ThermalEnergyDraw { get; set; }
        [ColumnData("Temperature to IR", 15, true, false, "N2")]
        [RadarField]
        public decimal TemperatureToIR { get; set; }
        [ColumnData("Max Cooling Rate", 16, true, true, "N2")]
        [RadarField]
        public decimal MaxCoolingRate { get; set; }



        [ColumnData("Lifetime Hours", 17, true, true, "N2")]
        [RadarField]
        public decimal MaxLifetimeHours { get; set; }

        public Shield(dynamic json, string file)
        {
            Id = json.__ref;
            Name = string.IsNullOrEmpty((string)json.name_local) ? file : json.name_local;
            Size = json.size;
            Filename = file;
            Type = Types.Shield;

            DamagedRegenDelay = json.Components.SCItemShieldGeneratorParams.DamagedRegenDelay;
            DecayRatio = json.Components.SCItemShieldGeneratorParams.DecayRatio;
            DownedRegenDelay = json.Components.SCItemShieldGeneratorParams.DownedRegenDelay;
            MaxReallocation = json.Components.SCItemShieldGeneratorParams.MaxReallocation;
            MaxShieldHealth = json.Components.SCItemShieldGeneratorParams.MaxShieldHealth;
            MaxShieldRegen = json.Components.SCItemShieldGeneratorParams.MaxShieldRegen;
            ReallocationRate = json.Components.SCItemShieldGeneratorParams.ReallocationRate;

            Cooldown = json.Components.SCItemShieldGeneratorParams.ShieldHardening.Cooldown;
            Duration = json.Components.SCItemShieldGeneratorParams.ShieldHardening.Duration;
            Factor = json.Components.SCItemShieldGeneratorParams.ShieldHardening.Factor;

            PowerBase = json.Components.EntityComponentPowerConnection.PowerBase;
            PowerDraw = json.Components.EntityComponentPowerConnection.PowerDraw;
            PowerToEM = json.Components.EntityComponentPowerConnection.PowerToEM;
            DecayRateOfEM = json.Components.EntityComponentPowerConnection.DecayRateOfEM;

            ThermalEnergyBase = json.Components.EntityComponentHeatConnection.ThermalEnergyBase;
            ThermalEnergyDraw = json.Components.EntityComponentHeatConnection.ThermalEnergyDraw;
            TemperatureToIR = json.Components.EntityComponentHeatConnection.TemperatureToIR;
            MaxCoolingRate = json.Components.EntityComponentHeatConnection.MaxCoolingRate;

            MaxLifetimeHours = json.Components.SDegradationParams.MaxLifetimeHours;
        }

        public static Dictionary<string, object> parseAll()
        {
            ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();
            Parallel.ForEach(Directory.GetFiles(Filepath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Shield s = new Shield(json, path.Replace(Filepath + "\\", "").Replace(".json", ""));
                    output.TryAdd(s.Id, s);
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
}
