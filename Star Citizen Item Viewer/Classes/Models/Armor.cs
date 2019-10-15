using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Star_Citizen_Item_Viewer.Classes
{
    public class Armor : Item
    {
        #region File Path
        public static string Filepath
        {
            get
            {
                return $"{_filePath}\\armor";
            }
        }
        #endregion

        [ColumnData("Weight", 3, true, false, "N2")]
        public decimal Weight { get; set; }

        public Armor(dynamic Json, string File)
        {
            Id = Json.__ref;
            Name = string.IsNullOrEmpty((string)Json.name_local) ? File : Json.name_local;
            Size = Json.size;
            Filename = File;

            switch (Json.type.ToString())
            {
                case "Char_Armor_Helmet": Type = Types.Helmet; break;
                case "Char_Armor_Torso": Type = Types.Torso; break;
                case "Char_Armor_Arms": Type = Types.Arms; break;
                case "Char_Armor_Legs": Type = Types.Legs; break;
            }

            Weight = Json.Components.SEntityPhysicsControllerParams.PhysType.SEntityDummyPhysicsControllerParams.Mass;
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
                    Armor a = new Armor(json, path.Replace(Filepath + "\\", "").Replace(".json", ""));
                    output.TryAdd(a.Id, a);
                }
                catch (Exception ex)
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
