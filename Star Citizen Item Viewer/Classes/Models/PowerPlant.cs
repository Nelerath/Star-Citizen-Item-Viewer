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
    public class PowerPlant : Item
    {
        #region File Path
        public static string Filepath
        {
            get
            {
                return $"{_filePath}\\power plants";
            }
        }
        #endregion

        [ColumnData("Power Base", 3, true, false, "N2", false)]
        [RadarField]
        public decimal PowerBase { get; set; }
        [ColumnData("Power", 4, true, true, "N2")]
        [RadarField]
        public decimal PowerDraw { get; set; }
        [ColumnData("Power to EM", 5, true, false, "N2")]
        [RadarField]
        public decimal PowerToEM { get; set; }
        [ColumnData("Decay Rate of EM", 7, true, true, "N2")]
        [RadarField]
        public decimal DecayRateOfEM { get; set; }
        [ColumnData("EM Signature", 6, true, false, "N2")]
        [RadarField]
        public decimal EM
        {
            get { return (PowerBase + PowerDraw) * PowerToEM; }
        }

        public decimal ThermalEnergyBase { get; set; }
        public decimal ThermalEnergyDraw { get; set; }
        [ColumnData("Temperature to IR", 8, true, false, "N2")]
        [RadarField]
        public decimal TemperatureToIR { get; set; }
        [ColumnData("Max Cooling Rate", 9, true, true, "N2")]
        [RadarField]
        public decimal MaxCoolingRate { get; set; }

        [ColumnData("Health", 10, true, true)]
        [RadarField]
        public int Health { get; set; }

        [ColumnData("Lifetime Hours", 11, true, true, "N2")]
        [RadarField]
        public decimal MaxLifetimeHours { get; set; }


        public PowerPlant(dynamic json, string file)
        {
            Id = json.__ref;
            Name = string.IsNullOrEmpty((string)json.name_local) ? file : json.name_local;
            Size = json.size;
            Filename = file;
            Type = Types.Powerplant;

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
                    PowerPlant p = new PowerPlant(json, path.Replace(Filepath + "\\", "").Replace(".json", ""));
                    output.TryAdd(p.Id, p);
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
