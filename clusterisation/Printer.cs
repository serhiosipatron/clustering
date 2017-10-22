using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Clusterisation
{
    static class Printer
    {
        public enum PlotSet
        {
            Data, Hierarchy, kMeans
        }

        static private Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Black, Color.Violet, Color.Pink, Color.Indigo };

        public static void Objects(DataSet ds, DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.ColumnCount = ds.AttributesCount + 2;
            dgv.RowHeadersWidth = 70;
            for (int i = 0; i < ds.AttributesCount; i++)
                dgv.Columns[i].Name = "Attr #" + (i + 1);
            dgv.Columns[ds.AttributesCount].Name = "K-means";
            dgv.Columns[ds.AttributesCount+1].Name = "Hierarchy";
            for (int i = 0; i < ds.Count; i++)
            {
                string[] row = new string[ds.AttributesCount + 2];
                for (int j = 0; j < row.Length-2; j++)
                    row[j] = ds.Objects[i].Attributes[j].ToString("0.000");
                row[row.Length - 2] = (ds.Objects[i].KMeansClusterID == -1) ? " - " : ds.Objects[i].KMeansClusterID.ToString();
                row[row.Length - 1] = (ds.Objects[i].HierarchyClusterID == -1) ? " - " : ds.Objects[i].HierarchyClusterID.ToString();
                dgv.Rows.Add(row);
                dgv.Rows[i].HeaderCell.Value = "Item #" + (i + 1);
            }
        }

        public static void Plot(GraphPane gp, DataSet ds, int x, int y, PlotSet ps)
        {
            if (x < 0 || y < 0 || x >= ds.AttributesCount || y >= ds.AttributesCount)
            {
                MessageBox.Show("Invalid x or y axis values");
                return;
            }
            gp.XAxis.Title.Text = gp.XAxis.Title.Text = "Attribute #" + (x + 1);
            gp.YAxis.Title.Text = gp.YAxis.Title.Text = "Attribute #" + (y + 1);
            if (ps == PlotSet.Data)
            {
                PointPairList ppl = new PointPairList();
                foreach (Obj o in ds.Objects)
                    ppl.Add(o.Attributes[x], o.Attributes[y]);
                LineItem li = gp.AddCurve(null, ppl, Color.Gray, SymbolType.Diamond);
                li.Line.IsVisible = false;
            } else if (ps == PlotSet.Hierarchy)
            {
                for (int i = 0; i < ds.Hierarchy.Count; i++)
                {
                    PointPairList ppl = new PointPairList();
                    foreach (Obj o in ds.Hierarchy[i].Objects)
                        ppl.Add(o.Attributes[x], o.Attributes[y]);
                    LineItem li = gp.AddCurve(null, ppl, colors[i], SymbolType.Diamond);
                    li.Line.IsVisible = false;
                }
            } else 
            {
                for (int i = 0; i < ds.KMeans.Count; i++)
                {
                    PointPairList ppl = new PointPairList();
                    foreach (Obj o in ds.KMeans[i].Objects)
                        ppl.Add(o.Attributes[x], o.Attributes[y]);
                    LineItem li = gp.AddCurve(null, ppl, colors[i], SymbolType.Diamond);
                    li.Line.IsVisible = false;
                }
            }

        }

        public static void HierarchyMeans(DataSet ds, DataGridView dgv1)
        {
            dgv1.Rows.Clear();
            dgv1.ColumnCount = ds.AttributesCount;
            dgv1.RowHeadersWidth = 150;
            for (int i = 0; i < ds.AttributesCount; i++)
                dgv1.Columns[i].Name = "Attr #" + (i + 1);
            foreach (Cluster cluster in ds.Hierarchy)
            {
                string[] row = new string[ds.AttributesCount];
                double[] means = new double[ds.AttributesCount];
                for (int i = 0; i < ds.AttributesCount; i++)
                    means[i] = 0;
                foreach (Obj o in cluster.Objects)
                    for (int i = 0; i < ds.AttributesCount; i++)
                        means[i] += o.Attributes[i];
                for (int i = 0; i < ds.AttributesCount; i++)
                    means[i] /= cluster.Count;
                for (int i = 0; i < row.Length; i++)
                    row[i] = means[i].ToString("0.000");
                dgv1.Rows.Add(row);
            }
            for (int i = 0; i < ds.Hierarchy.Count; i++)
                dgv1.Rows[i].HeaderCell.Value = "Cluster #" + (i + 1);
        }
        public static void KMeansMeans(DataSet ds, DataGridView dgv1) { 

            dgv1.Rows.Clear();
            dgv1.ColumnCount = ds.AttributesCount;
            dgv1.RowHeadersWidth = 150;
            for (int i = 0; i < ds.AttributesCount; i++)
                dgv1.Columns[i].Name = "Attr #" + (i + 1);
            foreach (KMCluster cluster in ds.KMeans)
            {
                string[] row = new string[ds.AttributesCount];
                double[] means = new double[ds.AttributesCount];
                for (int i = 0; i < ds.AttributesCount; i++)
                    means[i] = 0;
                foreach (Obj o in cluster.Objects)
                    for (int i = 0; i < ds.AttributesCount; i++)
                        means[i] += o.Attributes[i];
                for (int i = 0; i < ds.AttributesCount; i++)
                    means[i] /= cluster.Count;
                for (int i = 0; i < row.Length; i++)
                    row[i] = means[i].ToString("0.000");
                dgv1.Rows.Add(row);
            }
            for (int i = 0; i < ds.KMeans.Count; i++)
                dgv1.Rows[i].HeaderCell.Value = "Cluster #" + (i + 1);
        }

        public static void Quality (DataSet ds, RichTextBox rtb1)
        {
            if (ds.Hierarchy == null || ds.KMeans == null)
            {
                MessageBox.Show("Objects not clustered yet!");
                return;
            }

            double inside = 0, outside = 0;
            int count;

            foreach (Cluster c in ds.Hierarchy)
            {
                count = 0; double sum = 0;
                for (int i = 0; i < c.Objects.Count; i++)
                    for (int j = i; i<c.Objects.Count; i++)
                    {
                        sum += c.Objects[i].D(c.Objects[j]);
                        count++;
                    }
                inside += sum / count;
            }

            count = 0;
            for (int i = 0; i<ds.Hierarchy.Count; i++)
                for (int j = i + 1; i<ds.Hierarchy.Count; i++)
                {
                    outside += ds.Hierarchy[i].D(ds.Hierarchy[i]);
                    count++;
                }
            outside /= count;
            rtb1.Text = "\n\nHierarchy: \n Average distance inside clusters: \n" + inside + "\n\nAverage distance between clusters: \n" + outside;

            count = 0; inside = 0; outside = 0;
            foreach (KMCluster c in ds.KMeans)
            {
                count = 0; double sum = 0;
                for (int i = 0; i < c.Objects.Count; i++)
                {
                    for (int j = i; i < c.Objects.Count; i++)
                    {
                        sum += c.Objects[i].D(c.Objects[j]);
                        count++;
                    }
                }
                inside += sum / count;
            }

            count = 0;
            for (int i = 0; i < ds.KMeans.Count; i++)
            {
                for (int j = i + 1; i < ds.KMeans.Count; i++)
                {
                    outside += ds.Hierarchy[i].D(ds.KMeans[i]);
                    count++;
                }
            }
            outside /= count;
            rtb1.Text += "\n\nkMeans: \n Average distance inside clusters: \n" + inside + "\n\nAverage distance between clusters: \n" + outside;

        }



    }
}
