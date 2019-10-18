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
    public class WeaponFormWriter : FormWriter
    {
        public WeaponFormWriter(Type type) : base(type) { }

        public override TreeNode[] BuildTree(object[] Items)
        {
            Dictionary<string,Dictionary<string,Dictionary<string, List<TreeNode>>>> tree = new Dictionary<string, Dictionary<string, Dictionary<string, List<TreeNode>>>>();
            foreach (Weapon item in Items)
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
                if (item.ProjectilesPerShot > 1)
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

        public override List<Series> CreateLineGraphSeries(List<object> data, int ticks, CancellationToken token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 5, CancellationToken = token };
                Parallel.ForEach(data, options, (item, loopState) =>
                {
                    Weapon w = (Weapon)item;
                    Series s = w.GetNewLineGraphSeries();

                    decimal[] x = new decimal[ticks + 1];
                    decimal[] y = new decimal[ticks + 1];
                    decimal firerate = Convert.ToDecimal(item.Get("Firerate"));
                    for (int i = 0; i <= ticks; i++)
                    {
                        x[i] = i * .05M;
                        y[i] = w.DamageSpecial + (w.DamageSpecial * Math.Floor((i * .05M) / (1 / w.Firerate)));
                        if (options.CancellationToken.IsCancellationRequested)
                            break;
                    }

                    if (options.CancellationToken.IsCancellationRequested)
                        loopState.Break();

                    s.Points.DataBindXY(x, y);
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
                new string[] { "http://starcitizendb.com/api/components/df/WeaponGun", Weapon.Filepath }
            };
        }
    }
}
