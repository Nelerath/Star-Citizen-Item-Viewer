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

        [ColumnData("Power Draw", 10, true, false, "N2")]
        [RadarField]
        public decimal PowerDraw { get; set; }

        [ColumnData("Lifetime Hours", 11, true, true, "N2")]
        [RadarField]
        public decimal MaxLifetimeHours { get; set; }

        public Shield(dynamic Json, string File)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;
            Type = Types.Shield;

            DamagedRegenDelay = Json.Components.SCItemShieldGeneratorParams.DamagedRegenDelay;
            DecayRatio = Json.Components.SCItemShieldGeneratorParams.DecayRatio;
            DownedRegenDelay = Json.Components.SCItemShieldGeneratorParams.DownedRegenDelay;
            MaxReallocation = Json.Components.SCItemShieldGeneratorParams.MaxReallocation;
            MaxShieldHealth = Json.Components.SCItemShieldGeneratorParams.MaxShieldHealth;
            MaxShieldRegen = Json.Components.SCItemShieldGeneratorParams.MaxShieldRegen;
            ReallocationRate = Json.Components.SCItemShieldGeneratorParams.ReallocationRate;

            Cooldown = Json.Components.SCItemShieldGeneratorParams.ShieldHardening.Cooldown;
            Duration = Json.Components.SCItemShieldGeneratorParams.ShieldHardening.Duration;
            Factor = Json.Components.SCItemShieldGeneratorParams.ShieldHardening.Factor;

            PowerDraw = Json.Components.EntityComponentPowerConnection.PowerDraw;

            MaxLifetimeHours = Json.Components.SDegradationParams.MaxLifetimeHours;
        }

        public static Dictionary<string, object> parseAll(string filePath)
        {
            ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();
            Parallel.ForEach(Directory.GetFiles(filePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Shield s = new Shield(json, path.Replace(filePath + "\\", "").Replace(".json", ""));
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
