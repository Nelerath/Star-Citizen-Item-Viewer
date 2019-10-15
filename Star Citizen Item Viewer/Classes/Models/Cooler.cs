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
    public class Cooler : Item
    {
        #region File Path
        public static string Filepath
        {
            get
            {
                return $"{_filePath}\\coolers";
            }
        }
        #endregion

        [ColumnData("Cooling Rate", 3, true, true)]
        [RadarField]
        public long CoolingRate { get; set; }
        [ColumnData("Suppression Heat Factor", 4, true, false, "N2")]
        [RadarField]
        public decimal SuppressionHeatFactor { get; set; }
        [ColumnData("Suppression IR Factor", 5, true, false, "N2")]
        [RadarField]
        public decimal SuppressionIRFactor { get; set; }

        [ColumnData("Power Base", 6, true, false, "N2")]
        [RadarField]
        public decimal PowerBase { get; set; }
        [ColumnData("Power Draw", 7, true, false, "N2")]
        [RadarField]
        public decimal PowerDraw { get; set; }
        [ColumnData("Power to EM", 8, true, false, "N2")]
        [RadarField]
        public decimal PowerToEM { get; set; }
        [ColumnData("Decay Rate of EM", 10, true, true, "N2")]
        [RadarField]
        public decimal DecayRateOfEM { get; set; }
        [ColumnData("EM Signature", 9, true, false, "N2")]
        [RadarField]
        public decimal EM
        {
            get { return (PowerBase + PowerDraw) * PowerToEM; }
        }

        public decimal ThermalEnergyBase { get; set; }
        public decimal ThermalEnergyDraw { get; set; }
        [ColumnData("Temperature to IR", 11, true, false, "N2")]
        [RadarField]
        public decimal TemperatureToIR { get; set; }
        [ColumnData("Max Cooling Rate", 12, true, true, "N2")]
        [RadarField]
        public decimal MaxCoolingRate { get; set; }

        [ColumnData("Health", 13, true, true)]
        [RadarField]
        public int Health { get; set; }

        [ColumnData("Lifetime Hours", 14, true, true)]
        [RadarField]
        public decimal MaxLifetimeHours { get; set; }

        public Cooler(dynamic json, string file)
        {
            Id = json.__ref;
            Name = string.IsNullOrEmpty((string)json.name_local) ? file : json.name_local;
            Size = json.size;
            Filename = file;
            Type = Types.Powerplant;

            CoolingRate = json.Components.SCItemCoolerParams.CoolingRate;
            SuppressionHeatFactor = json.Components.SCItemCoolerParams.SuppressionHeatFactor;
            SuppressionIRFactor = json.Components.SCItemCoolerParams.SuppressionIRFactor;

            PowerBase = json.Components.EntityComponentPowerConnection.PowerBase;
            PowerDraw = json.Components.EntityComponentPowerConnection.PowerDraw;
            PowerToEM = json.Components.EntityComponentPowerConnection.PowerToEM;
            DecayRateOfEM = json.Components.EntityComponentPowerConnection.DecayRateOfEM;

            ThermalEnergyBase = json.Components.EntityComponentHeatConnection.ThermalEnergyBase;
            ThermalEnergyDraw = json.Components.EntityComponentHeatConnection.ThermalEnergyDraw;
            TemperatureToIR = json.Components.EntityComponentHeatConnection.TemperatureToIR;
            MaxCoolingRate = json.Components.EntityComponentHeatConnection.MaxCoolingRate;

            Health = json.Components.SHealthComponentParams.Health;

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
                    Cooler c = new Cooler(json, path.Replace(Filepath + "\\", "").Replace(".json", ""));
                    output.TryAdd(c.Id, c);
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
