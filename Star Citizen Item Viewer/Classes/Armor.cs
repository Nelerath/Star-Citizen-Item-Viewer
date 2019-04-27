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

        public static Dictionary<string, object> parseAll(string FilePath)
        {
            ConcurrentDictionary<string, object> output = new ConcurrentDictionary<string, object>();

            Parallel.ForEach(Directory.GetFiles(FilePath), new ParallelOptions { MaxDegreeOfParallelism = 5 }, path =>
            {
                try
                {
                    string raw = File.ReadAllText(path).Replace("@", "");
                    dynamic json = JsonConvert.DeserializeObject(raw);
                    Armor a = new Armor(json, path.Replace(FilePath + "\\", "").Replace(".json", ""));
                    output.TryAdd(a.Id, a);
                }
                catch (Exception ex)
                {
                    File.Delete(path);
                }
            });

            return new Dictionary<string, object>(output);
        }

        public static Column[] GetColumns()
        {
            return new Column[] {
                new Column("Id", "Id", false, false, "", false),
                new Column("Name", "Name", false),
                new Column("Weight", "Weight", true, false, "N2"),
                new Column("Score", null, true, true, "N2", false),
            };
        }

        public static List<string[]> GetDownloadInfo(string FilePath)
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Arms", FilePath + "\\armor" }
                ,new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Helmet", FilePath + "\\armor" }
                ,new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Legs", FilePath + "\\armor" }
                ,new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Torso", FilePath + "\\armor" }
            };
        }

        public static TreeNode[] BuildTree(object[] Items)
        {
            Dictionary<string, TreeNode> tree = new Dictionary<string, TreeNode>();
            foreach (Item item in Items)
            {
                TreeNode n = new TreeNode();
                n.Name = item.Id;
                n.Text = item.Name;

                string key = item.Type.ToString("g");
                if (tree.ContainsKey(key))
                    tree[key].Nodes.Add(n);
                else
                    tree.Add(key, new TreeNode(key, new TreeNode[] { n }));
            }
            List<TreeNode> output = new List<TreeNode>();
            foreach (var key in tree.Keys.OrderBy(x => x))
            {
                output.Add(tree[key]);
            }
            return output.ToArray();
        }
    }
}
