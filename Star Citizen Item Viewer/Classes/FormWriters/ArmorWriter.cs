using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Star_Citizen_Item_Viewer.Classes.NewFolder1
{
    public class ArmorWriter : FormWriter
    {
        public ArmorWriter(Type type) : base(type) { }

        public override List<string[]> GetDownloadInfo()
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Arms", Armor.Filepath }
                ,new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Helmet", Armor.Filepath }
                ,new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Legs", Armor.Filepath }
                ,new string[] { "http://starcitizendb.com/api/components/df/Char_Armor_Torso", Armor.Filepath }
            };
        }
    }
}
