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
    public class GunFormWriter : FormWriter
    {
        public GunFormWriter(Type type) : base(type) { }

        public override TreeNode[] BuildTree(object[] Items)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<TreeNode>>>> tree = new Dictionary<string, Dictionary<string, Dictionary<string, List<TreeNode>>>>();
            foreach (Gun item in Items)
            {
                TreeNode n = new TreeNode();
                n.Name = item.Id;
                n.Text = item.Name;

                string sizeKey = item.Size.ToString();

                string damageKey = "Unknown";
                if (item.DamagePhysical > 0)
                    damageKey = "Physical";
                else if (item.DamageEnergy > 0)
                    damageKey = "Energy";
                else if (item.DamageBiochemical > 0)
                    damageKey = "Biochemical";
                else if (item.DamageThermal > 0)
                    damageKey = "Thermal";
                else if (item.DamageDistortion > 0)
                    damageKey = "Distortion";

                string shotKey = "Unknown";
                if (
                    (item.Single == null && item.Rapid == null && item.Burst != null) // It only has burstfire
                    || (item.Rapid != null && item.Rapid.ProjectilesPerShot > 1) // It's rapid fires bursts
                    || (item.Single != null && item.Single.ProjectilesPerShot > 1) // It's single fires bursts
                    )
                    shotKey = "Multiple Projectiles";
                else
                    shotKey = "Single Projectile";

                if (tree.ContainsKey(sizeKey))
                {
                    if (tree[sizeKey].ContainsKey(damageKey))
                    {
                        if (tree[sizeKey][damageKey].ContainsKey(shotKey))
                        {
                            tree[sizeKey][damageKey][shotKey].Add(n);
                        }
                        else
                        {
                            tree[sizeKey][damageKey].Add(shotKey, new List<TreeNode>() { n });
                        }
                    }
                    else
                    {
                        tree[sizeKey].Add(damageKey, new Dictionary<string, List<TreeNode>>());
                        tree[sizeKey][damageKey].Add(shotKey, new List<TreeNode>() { n });
                    }
                }
                else
                {
                    tree.Add(sizeKey, new Dictionary<string, Dictionary<string, List<TreeNode>>>());
                    tree[sizeKey].Add(damageKey, new Dictionary<string, List<TreeNode>>());
                    tree[sizeKey][damageKey].Add(shotKey, new List<TreeNode>() { n });
                }
            }


            List<TreeNode> nodes = new List<TreeNode>();
            foreach (var size in tree.Keys)
            {
                List<TreeNode> sizeNodes = new List<TreeNode>();
                foreach (var damage in tree[size].Keys)
                {
                    List<TreeNode> damageNodes = new List<TreeNode>();
                    foreach (var shot in tree[size][damage].Keys)
                    {
                        TreeNode shotNode = new TreeNode(shot);
                        shotNode.Nodes.AddRange(tree[size][damage][shot].OrderBy(x => x.Text).ToArray());
                        damageNodes.Add(shotNode);
                    }
                    TreeNode damageNode = new TreeNode(damage);
                    damageNode.Nodes.AddRange(damageNodes.OrderBy(x => x.Text).ToArray());
                    sizeNodes.Add(damageNode);
                }
                TreeNode sizeNode = new TreeNode(size);
                sizeNode.Nodes.AddRange(sizeNodes.OrderBy(x => x.Text).ToArray());
                nodes.Add(sizeNode);
            }

            return nodes.OrderBy(x => Convert.ToInt16(x.Text)).ToArray();
        }

        public override List<Series> CreateRadarGraphSeries(List<object> data, CancellationToken token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 2, CancellationToken = token };
                Parallel.ForEach(data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;
                    Series s = g.GetNewRadarGraphSeries();
                    foreach (var col in Columns)
                    {
                        if (col.RadarField)
                        {
                            string[] fieldPath = col.DataFieldName.Split('.');
                            if (fieldPath.Count() > 1)
                            {
                                s.Points.Add(new DataPoint(0, GetRank(col.DataFieldName, Convert.ToDouble(Utility.GetValue(g, col.DataFieldName)), col.SortDescending)));
                            }
                            else
                                s.Points.Add(new DataPoint(0, GetRank(col.DataFieldName, Convert.ToDouble(Utility.GetValue(g, col.DataFieldName)), col.SortDescending)));
                        }
                    }
                    list.Enqueue(s);
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).ToList();
        }

        public override List<string[]> GetDownloadInfo()
        {
            return new List<string[]>
            {
                new string[] { "http://starcitizendb.com/api/components/df/WeaponPersonal", Gun.GunPath }
                ,new string[] { "http://starcitizendb.com/api/ammo/energy", Gun.AmmoPath }
                ,new string[] { "http://starcitizendb.com/api/ammo/projectile", Gun.AmmoPath }
                ,new string[] { "http://starcitizendb.com/api/components/df/WeaponAttachment", Gun.AttachmentPath }
            };
        }
    }
}
