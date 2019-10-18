using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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

        private static decimal TickSeconds = .01M;
        public override List<Series> CreateLineGraphSeries(List<object> data, int ticks, CancellationToken token)
        {
            ConcurrentQueue<Series> list = new ConcurrentQueue<Series>();
            try
            {
                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 2, CancellationToken = token };
                Parallel.ForEach(data, options, (item, loopState) =>
                {
                    Gun g = (Gun)item;

                    decimal x = 0M;
                    int shotsFired = 1;

                    if (g.Rapid != null)
                    {
                        List<decimal> rapidX = new List<decimal>();
                        List<decimal> rapidY = new List<decimal>();
                        x = 0;
                        shotsFired = 1;
                        rapidX.Add(x);
                        rapidY.Add(shotsFired * g.DamageTotal);
                        while (x <= ticks * TickSeconds)
                        {
                            x += (1 / g.Rapid.Firerate);
                            if (x <= ticks * TickSeconds)
                            {
                                shotsFired++;
                                rapidX.Add(x);
                            }
                            else
                            {
                                rapidX.Add(ticks * TickSeconds);
                            }
                            rapidY.Add(shotsFired * g.DamageTotal);
                        }

                        Series s = g.GetNewLineGraphSeries(g.Name + " Rapid");
                        s.Points.DataBindXY(rapidX.ToList(), rapidY.ToList());
                        list.Enqueue(s);
                    }

                    if (g.Burst != null)
                    {
                        List<decimal> burstX = new List<decimal>();
                        List<decimal> burstY = new List<decimal>();
                        x = 0;
                        shotsFired = 0;
                        decimal q = 0;
                        for (int i = 0; i < g.Burst.ShotCount; i++)
                        {
                            q = x + (i * (1 / g.Burst.Firerate));
                            if (q <= ticks * TickSeconds)
                            {
                                shotsFired++;
                                burstX.Add(q);
                                burstY.Add(shotsFired * g.DamageTotal);
                            }
                            else
                                break;
                        }
                        x += g.Burst.BurstCooldown;
                        while (x <= ticks * TickSeconds)
                        {
                            for (int i = 0; i < g.Burst.ShotCount; i++)
                            {
                                q = x + (i * (1 / g.Burst.Firerate));
                                if (q <= ticks * TickSeconds)
                                {
                                    shotsFired++;
                                    burstX.Add(q);
                                    burstY.Add(shotsFired * g.DamageTotal);
                                }
                                else
                                    break;
                            }
                            x += g.Burst.BurstCooldown;
                        }
                        burstX.Add(ticks * TickSeconds);
                        burstY.Add(shotsFired * g.DamageTotal);

                        Series s = g.GetNewLineGraphSeries(g.Name + " Burst");
                        s.BorderDashStyle = ChartDashStyle.Dash;
                        s.Points.DataBindXY(burstX.ToList(), burstY.ToList());
                        list.Enqueue(s);
                    }

                    if (g.Single != null)
                    {
                        List<decimal> singleX = new List<decimal>();
                        List<decimal> singleY = new List<decimal>();
                        x = 0;
                        shotsFired = 1;
                        singleX.Add(x);
                        singleY.Add(shotsFired * g.DamageTotal);
                        while (x <= ticks * TickSeconds)
                        {
                            x += (1 / g.Single.Firerate);
                            if (x <= ticks * TickSeconds)
                            {
                                shotsFired++;
                                singleX.Add(x);
                            }
                            else
                            {
                                singleX.Add(ticks * TickSeconds);
                            }
                            singleY.Add(shotsFired * g.DamageTotal);
                        }

                        Series s = g.GetNewLineGraphSeries(g.Name + " Single");
                        s.BorderDashStyle = ChartDashStyle.Dot;
                        s.Points.DataBindXY(singleX.ToList(), singleY.ToList());
                        list.Enqueue(s);
                    }
                });
            }
            catch (OperationCanceledException) { }
            return new List<Series>(list).OrderBy(x => x.Name).ToList();
        }

        public override List<Series> DrawKillLines(int ticks)
        {
            List<Series> output = new List<Series>();
            foreach (var bodyPart in new string[] { "Head", "Torso" })
            {
                Color color;
                decimal health = 0;
                decimal armoredHealth = 0;
                if (bodyPart == "Head")
                {
                    health = 10M / 1.5M;
                    color = Color.Red;
                }
                else
                {
                    health = 20M;
                    color = Color.Yellow;
                }
                foreach (var armor in new string[] { "No Armor", "Light", "Medium", "Heavy" })
                {
                    switch (armor)
                    {
                        case "Light": armoredHealth = health / .8M; break;
                        case "Medium": armoredHealth = health / .7M; break;
                        case "Heavy": armoredHealth = health / .6M; break;
                        default: armoredHealth = health; break;
                    }

                    decimal[] x = new decimal[2] { 0, ticks * TickSeconds };
                    decimal[] y = new decimal[2] { armoredHealth, armoredHealth };

                    Series s = new Series(armor + " " + bodyPart);
                    s.ChartType = SeriesChartType.Line;
                    s.BorderWidth = 1;
                    s.MarkerStyle = MarkerStyle.None;
                    s.BorderDashStyle = ChartDashStyle.Solid;
                    s.IsValueShownAsLabel = false;
                    s.IsVisibleInLegend = false;
                    s.Color = color;
                    s.Points.DataBindXY(x, y);
                    output.Add(s);
                }
            }

            return output;
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
