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
    public class ShieldFormWriter : FormWriter
    {
        public override List<string[]> GetDownloadInfo(string filePath)
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/Shield", filePath + "\\shields" }
            };
        }
    }
}
