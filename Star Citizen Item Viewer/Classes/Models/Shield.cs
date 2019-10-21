using Newtonsoft.Json;
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
        [ColumnData("Physical Absorption Min", 6, true, true, "N2")]
        [RadarField]
        public decimal PhysicalAbsorptionMin { get; set; }
        [ColumnData("Physical Absorption Max", 7, true, true, "N2")]
        [RadarField]
        public decimal PhysicalAbsorptionMax { get; set; }
        [ColumnData("Energy Absorption Min", 8, true, true, "N2")]
        [RadarField]
        public decimal EnergyAbsorptionMin { get; set; }
        [ColumnData("Energy Absorption Max", 9, true, true, "N2")]
        [RadarField]
        public decimal EnergyAbsorptionMax { get; set; }
        [ColumnData("Distortion Absorption Min", 10, true, true, "N2")]
        [RadarField]
        public decimal DistortionAbsorptionMin { get; set; }
        [ColumnData("Distortion Absorption Max", 11, true, true, "N2")]
        [RadarField]
        public decimal DistortionAbsorptionMax { get; set; }
        [ColumnData("Thermal Absorption Min", 12, true, true, "N2")]
        [RadarField]
        public decimal ThermalAbsorptionMin { get; set; }
        [ColumnData("Thermal Absorption Max", 13, true, true, "N2")]
        [RadarField]
        public decimal ThermalAbsorptionMax { get; set; }
        [ColumnData("Biochemical Absorption Min", 14, true, true, "N2")]
        [RadarField]
        public decimal BiochemicalAbsorptionMin { get; set; }
        [ColumnData("Biochemical Absorption Max", 15, true, true, "N2")]
        [RadarField]
        public decimal BiochemicalAbsorptionMax { get; set; }
        [ColumnData("Stun Absorption Min", 16, true, true, "N2")]
        [RadarField]
        public decimal StunAbsorptionMin { get; set; }
        [ColumnData("Stun Absorption Max", 17, true, true, "N2")]
        [RadarField]
        public decimal StunAbsorptionMax { get; set; }

        [ColumnData("Hardening Cooldown", 19, true, false, "N2")]
        public decimal Cooldown { get; set; }
        [ColumnData("Hardening Duration", 18, true, true, "N2")]
        public decimal Duration { get; set; }
        [ColumnData("Hardening Factor", 20, true, true, "N2")]
        public decimal Factor { get; set; }

        [ColumnData("Power Base", 21, true, false, "N2")]
        [RadarField]
        public decimal PowerBase { get; set; }
        [ColumnData("Power Draw", 22, true, false, "N2")]
        [RadarField]
        public decimal PowerDraw { get; set; }
        [ColumnData("Power to EM", 23, true, false, "N2")]
        [RadarField]
        public decimal PowerToEM { get; set; }
        [ColumnData("Decay Rate of EM", 25, true, true, "N2")]
        [RadarField]
        public decimal DecayRateOfEM { get; set; }
        [ColumnData("EM Signature", 24, true, false, "N2")]
        [RadarField]
        public decimal EM
        {
            get { return (PowerBase + PowerDraw) * PowerToEM; }
        }

        public decimal ThermalEnergyBase { get; set; }
        public decimal ThermalEnergyDraw { get; set; }
        [ColumnData("Temperature to IR", 26, true, false, "N2")]
        [RadarField]
        public decimal TemperatureToIR { get; set; }
        [ColumnData("Max Cooling Rate", 27, true, true, "N2")]
        [RadarField]
        public decimal MaxCoolingRate { get; set; }



        [ColumnData("Lifetime Hours", 28, true, true, "N2")]
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

            // everyone involved should be ashamed
            PhysicalAbsorptionMin = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[0].Min;
            PhysicalAbsorptionMax = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[0].Max;
            EnergyAbsorptionMin = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[1].Min;
            EnergyAbsorptionMax = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[1].Max;
            DistortionAbsorptionMin = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[2].Min;
            DistortionAbsorptionMax = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[2].Max;
            ThermalAbsorptionMin  = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[3].Min;
            ThermalAbsorptionMax = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[3].Max;
            BiochemicalAbsorptionMin = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[4].Min;
            BiochemicalAbsorptionMax = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[4].Max;
            StunAbsorptionMin  = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[5].Min;
            StunAbsorptionMax = json.Components.SCItemShieldGeneratorParams.ShieldAbsorption.SShieldAbsorption[5].Max;

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
