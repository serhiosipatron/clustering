using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;


namespace Clusterisation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }







        DataSet ds;

        String fpath;


        

        private void button1_Click(object sender, EventArgs e) //IMPORT
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] filelines = File.ReadAllLines(theDialog.FileName);
                    ds = new DataSet();
                    int id = 0;
                    foreach (string line in filelines)
                    {
                        string[] list = line.Split('|');
                        Obj o = new Obj(list);
                        ds.Objects.Add(o);
                        id++;
                    }
                    Printer.Objects(ds, dgv);
                    numericUpDown1.Maximum = numericUpDown2.Maximum = (ds.AttributesCount - 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clustering.Standardize(ds);
            Printer.Objects(ds, dgv);
        }

        private void button4_Click(object sender, EventArgs e) //#2
        {
            ds.Hierarchy = Clustering.Hierarchy(ds, (int)numericUpDown3.Value);
            Printer.Objects(ds, dgv);
            Printer.HierarchyMeans(ds,dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ds.KMeans = Clustering.KMeans(ds, (int)numericUpDown3.Value);
            Printer.Objects(ds, dgv);
            Printer.KMeansMeans(ds, dataGridView2);
        }

        /**
  

            public void MeansToDGV(DataGridView dgv1, DataGridView dgv2)
            {
                dgv1.Rows.Clear();
                dgv1.ColumnCount = AttributesCount;
                dgv1.RowHeadersWidth = 150;
                for (int i = 0; i < AttributesCount; i++)
                    dgv1.Columns[i].Name = "Attr #" + i;
                foreach (Cluster cluster in Clusters1)
                    cluster.SetMeans();
                for (int i = 0; i < Clusters1.Count; i++)
                {
                    string[] row = new string[AttributesCount];
                    for (int j = 0; j < row.Length; j++)
                        row[j] = Clusters1[i].Means[j].ToString();
                    dgv1.Rows.Add(row);
                    dgv1.Rows[i].HeaderCell.Value = "Cluster #" + i;
                }

                dgv2.Rows.Clear();
                dgv2.ColumnCount = AttributesCount;
                dgv2.RowHeadersWidth = 150;
                for (int i = 0; i < AttributesCount; i++)
                    dgv2.Columns[i].Name = "Attr #" + i;
                foreach (Cluster cluster in Clusters2)
                    cluster.SetMeans();
                for (int i = 0; i < Clusters2.Count; i++)
                {
                    string[] row = new string[AttributesCount];
                    for (int j = 0; j < row.Length; j++)
                        row[j] = Clusters2[i].Means[j].ToString();
                    dgv2.Rows.Add(row);
                    dgv2.Rows[i].HeaderCell.Value = "Cluster #" + i;
                }
            }

        private void Quality(List<Cluster4Kmeans> Clusters0, List<Cluster4hierarchy> Clusters2, RichTextBox rtb, int n)
        {
            Cluster[] Clusters1;
            if (n == 1)
                Clusters1 = Clusters0.ToArray();
            else
                Clusters1 = Clusters2.ToArray();
            double inside = 0, outside = 0;
            int count;

            foreach (Cluster c in Clusters1)
            {
                count = 0; double sum = 0;
                for (int i = 0; i < c.Items.Count; i++)
                {
                    for (int j = i; i < c.Items.Count; i++)
                    {
                        sum += c.Items[i].d(c.Items[j]);
                        count++;
                    }
                }
                inside += sum / count;
            }

            count = 0;
            for (int i = 0; i < Clusters1.Length; i++)
            {
                for (int j = i + 1; i < Clusters1.Length; i++)
                {
                    outside += Clusters1[i].D(Clusters1[i]);
                    count++;
                }
            }
            outside /= count;
            rtb.Text = "Average distance inside clusters: \n" + inside + "\n\nAverage distance between clusters: \n" + outside;
        }
    **/
        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (ds == null)
            {
                MessageBox.Show("Data not imported yet");
                return;
            }
            int x = 0, y = 0;
            try
            {
                x = (int)numericUpDown1.Value;
                y = (int)numericUpDown2.Value;
                GraphPane gp = new GraphPane();
                gp = zgc.GraphPane;
                zgc.GraphPane.CurveList.Clear();
                zgc.GraphPane.GraphObjList.Clear();
                Printer.Plot(gp, ds, x, y, Printer.PlotSet.Data);
                zgc.AxisChange();
                zgc.Invalidate();
                zgc.Refresh();
            } catch (Exception ex)
            {
                MessageBox.Show("Invalid x or y axis values");
                return;
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (ds == null)
            {
                MessageBox.Show("Data not imported yet");
                return;
            }
            int x = 0, y = 0;
            try
            {
                x = (int)numericUpDown1.Value;
                y = (int)numericUpDown2.Value;
                GraphPane gp = new GraphPane();
                gp = zgc.GraphPane;
                zgc.GraphPane.CurveList.Clear();
                zgc.GraphPane.GraphObjList.Clear();
                Printer.Plot(gp, ds, x, y, Printer.PlotSet.Hierarchy);
                zgc.AxisChange();
                zgc.Invalidate();
                zgc.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid x or y axis values");
                return;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (ds == null)
            {
                MessageBox.Show("Data not imported yet");
                return;
            }
            int x = 0, y = 0;
            try
            {
                x = (int)numericUpDown1.Value;
                y = (int)numericUpDown2.Value;
                GraphPane gp = new GraphPane();
                gp = zgc.GraphPane;
                zgc.GraphPane.CurveList.Clear();
                zgc.GraphPane.GraphObjList.Clear();
                Printer.Plot(gp, ds, x, y, Printer.PlotSet.kMeans);
                zgc.AxisChange();
                zgc.Invalidate();
                zgc.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid x or y axis values");
                return;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Printer.Quality(ds, richTextBox1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
        }

        bool[] removedAttrs;

        private void button9_Click_1(object sender, EventArgs e)
        {
            while (ds.AttributesCount > (int)numericUpDown4.Value)
            {
                rtbdel.Text += "\n\n    DataSet: " + ds.Quality[1];

                double[] quals = new double[ds.AttributesCount];
                if (removedAttrs == null)
                    removedAttrs = new bool[ds.AttributesCount];
                for (int i = 0; i < quals.Length; i++)
                {
                    List<KMCluster> KMeans = Clustering.KMeans(ds, (int)numericUpDown3.Value, i);
                    quals[i] = DataSet.QualityExcl(KMeans, i);
                    rtbdel.Text += "\n    DataSet without attr #" + i + ": " + quals[i];
                }
                int minIndex = 0;
                for (int i = 0; i < quals.Length; i++)
                    if (quals[i] < quals[minIndex]) minIndex = i;
                rtbdel.Text += "\n    REMOVING attr #" + minIndex;
                ds.RemoveAttr(minIndex);

                int co = 0;
                for (int i = 0; i <= minIndex; i++)
                    if (removedAttrs[i]) co += 1;
                removedAttrs[co + minIndex] = true;

                ds.KMeans = Clustering.KMeans(ds, (int)numericUpDown3.Value);

                rtbdel.Text += "\n    New DataSet: " + ds.Quality[1];
            }

            Printer.Objects(ds, dgv);
            Printer.KMeansMeans(ds, dataGridView2);
            rtbdel.Text += "\n\n\n    Attrs were removed: ";
            for (int i = 0; i < removedAttrs.Length; i++)
                if (removedAttrs[i]) rtbdel.Text += " #" + i;

        }
            
    }
}


/*
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

// Kmeans цикл пока центры не перестанут смещаться 
// Не пересчитывать матрицу
// Иерархия: дендрограмма (цикл до конца)
// (Другое расстояние между кластерами)
// 


namespace Clusterisation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataHandle dh;
        GraphPane gp1 = new GraphPane();
        GraphPane gp2 = new GraphPane();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //IMPORT
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] filelines = File.ReadAllLines(theDialog.FileName);
                    dh = new DataHandle();
                    foreach (string line in filelines)
                    {
                        string[] list = line.Split('|');
                        Item item = new Item(list);
                        dh.addItem(item);
                    }
                    dh.toDGV(dgv);
                    numericUpDown1.Maximum = numericUpDown2.Maximum = dh.AttributesCount - 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        public class Item
        {
            public List<double> Attributes { get; }
            public int[] Mark { get; set;  }

            public Item(string[] AttributesStr)
            {
                this.Mark = new int[2] { -1, -1};
                this.Attributes = new List<double>();
                try
                {
                    foreach (string attr in AttributesStr)
                    {
                        double val = Double.Parse(attr.Replace('.', ','));
                        Attributes.Add(val);
                    }
                } catch (Exception ex)
                {
                    MessageBox.Show("Error had place while parsing data" + ex.Message);
                    return;
                }
            }
            public Item(List<double> Attributes)
            {
                this.Mark = new int[2]{ -1, -1};
                this.Attributes = new List<double>();
                this.Attributes.AddRange(Attributes);
            }

            public void Standardize(double[] x_, double[] s)
            {
                for (int i = 0; i < Attributes.Count; i++)
                    Attributes[i] = (Attributes[i] - x_[i]) / s[i];
            }

            public int Length()
            {
                return Attributes.Count;
            }

            public double d(Item item)
            {
                double d = 0;
                for (int i = 0; i < Attributes.Count; i++)
                    d += Math.Pow((Attributes[i] - item.Attributes[i]), 2);
                return Math.Sqrt(d);
            }
        }

        public abstract class Cluster
        {
            public int Id { get; set;  }
            public List<Item> Items { get; set; }
            public List<double> Means { get; }

            public Cluster(int id, Item item)
            {
                this.Id = id;
                this.Items = new List<Item>();
                Means = new List<double>();
            }

            public virtual void AddItem(Item item)
            {
                Items.Add(item);
            }

            public void SetMeans()
            {
                for (int i = 0; i < Items[0].Length(); i++)
                {
                    double x = 0;
                    foreach (Item item in Items)
                        x += item.Attributes[i];
                    Means.Add(x / Items.Count);
                }
            }

            public double D(Cluster c)
            {
                double sum = 0;
                foreach (Item c1 in this.Items)
                    foreach (Item c2 in c.Items)
                        sum += c1.d(c2);
                return sum / (this.Items.Count * c.Items.Count);
            }
        }

        public class Cluster4Kmeans : Cluster
        {
            public Item Center;

            public Cluster4Kmeans(int id, Item item)
                :base(id, item) 
            {
                this.Center = item;
                AddItem(item);
            }
            
            public override void AddItem(Item item)
            {
                Items.Add(item);
                item.Mark[0] = Id;
                SetCenter();
            }

            private void SetCenter()
            {
                List<double> list = new List<double>();
                for (int i = 0; i < Center.Attributes.Count; i++)
                {
                    double tmp = 0;
                    foreach (Item item in Items)
                        tmp += item.Attributes[i];
                    list.Add(tmp / Items.Count);
                }
                this.Center = new Item(list);
            }
        }

        public class Cluster4hierarchy : Cluster
        {
            public Cluster4hierarchy(int id, Item item):
                base(id, item)
            {
                AddItem(item);
            }

            public override void AddItem(Item item)
            {
                Items.Add(item);
                item.Mark[1] = Id;
            }

            public void setId(int id)
            {
                Id = id;
                foreach (Item i in Items)
                    i.Mark[1] = id;
            }

            public void Merge(Cluster c)
            {
                if (c == this) return;
                foreach (Item i in c.Items)
                {
                    i.Mark[1] = this.Id;
                    this.AddItem(i);
                }
                c.Items.Clear();
            }
        }

        public class DataHandle
        {
            public List<Item> Items { get; }
            public List<Cluster4Kmeans> Clusters1 { get; }
            public List<Cluster4hierarchy> Clusters2 { get; }
            public int AttributesCount;
            public int ItemsCount;
            private int ClustersCount;
            static private Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Black, Color.Violet, Color.Pink, Color.Indigo };
            private List<List<double>> matrix = new List<List<double>>();
            
            public DataHandle()
            {
                Items = new List<Item>();
                Clusters1 = new List<Cluster4Kmeans>();
                Clusters2 = new List<Cluster4hierarchy>();
                AttributesCount = 0;
                ItemsCount = 0;
            }

            public void addItem(Item item)
            {
                Items.Add(item);
                if (AttributesCount == 0)
                    this.AttributesCount = item.Length();
                ItemsCount++;
            }

            public void Standardize()
            {
                double[] x_ = new double[AttributesCount];
                double[] s  = new double[AttributesCount];
                for (int i = 0; i < AttributesCount; i++)
                {
                    x_[i] = 0;
                    for (int j = 0; j < ItemsCount; j++)
                        x_[i] += Items[j].Attributes[i];
                    x_[i] /= ItemsCount;
                }
                for (int i = 0; i < AttributesCount; i++)
                {
                    s[i] = 0;
                    for (int j = 0; j < ItemsCount; j++)
                    {
                        s[i] += Math.Pow((Items[j].Attributes[i] - x_[i]), 2);
                    }
                    s[i] = Math.Sqrt(s[i] / (ItemsCount - 1));
                }
                for (int i = 0; i < ItemsCount; i++)
                    Items[i].Standardize(x_, s);
            }

            public void toDGV(DataGridView dgv)
            {
                dgv.Rows.Clear();
                dgv.ColumnCount = AttributesCount + 2;
                dgv.RowHeadersWidth = 150;
                for (int i = 0; i < AttributesCount; i++)
                    dgv.Columns[i].Name = "Attr #" + i;
                dgv.Columns[AttributesCount].Name = "K means";
                dgv.Columns[AttributesCount + 1].Name = "Hierarchy";
                for (int i = 0; i < Items.Count; i++)
                {
                    string[] row = new string[AttributesCount + 2];
                    for (int j = 0; j < row.Length - 2; j++)
                        row[j] = Items[i].Attributes[j].ToString();
                    row[row.Length - 2] = "Cluster #" + ( (Items[i].Mark[0] == -1) ? "?" : Items[i].Mark[0].ToString() );
                    row[row.Length - 1] = "Cluster #" + ( (Items[i].Mark[1] == -1) ? "?" : Items[i].Mark[1].ToString() );
                    dgv.Rows.Add(row);
                    dgv.Rows[i].HeaderCell.Value = "Item #" + i;
                }
            }
            public void MeansToDGV(DataGridView dgv1, DataGridView dgv2)
            {
                dgv1.Rows.Clear();
                dgv1.ColumnCount = AttributesCount;
                dgv1.RowHeadersWidth = 150;
                for (int i = 0; i < AttributesCount; i++)
                    dgv1.Columns[i].Name = "Attr #" + i;
                foreach (Cluster cluster in Clusters1)
                    cluster.SetMeans();
                for (int i = 0; i < Clusters1.Count; i++)
                {
                    string[] row = new string[AttributesCount];
                    for (int j = 0; j < row.Length; j++)
                        row[j] = Clusters1[i].Means[j].ToString();
                    dgv1.Rows.Add(row);
                    dgv1.Rows[i].HeaderCell.Value = "Cluster #" + i;
                }

                dgv2.Rows.Clear();
                dgv2.ColumnCount = AttributesCount;
                dgv2.RowHeadersWidth = 150;
                for (int i = 0; i < AttributesCount; i++)
                    dgv2.Columns[i].Name = "Attr #" + i;
                foreach (Cluster cluster in Clusters2)
                    cluster.SetMeans();
                for (int i = 0; i < Clusters2.Count; i++)
                {
                    string[] row = new string[AttributesCount];
                    for (int j = 0; j < row.Length; j++)
                        row[j] = Clusters2[i].Means[j].ToString();
                    dgv2.Rows.Add(row);
                    dgv2.Rows[i].HeaderCell.Value = "Cluster #" + i;
                }
            }

            public void Draw(GraphPane gp1, GraphPane gp2, int x, int y)
            {
                gp1.XAxis.Title.Text = gp2.XAxis.Title.Text = "Attribute #" + x;
                gp1.YAxis.Title.Text = gp2.YAxis.Title.Text = "Attribute #" + y;
                foreach (Cluster cluster in Clusters1) {
                    PointPairList ppl = new PointPairList();
                    foreach (Item item in cluster.Items)
                        ppl.Add(item.Attributes[x], item.Attributes[y]);
                    LineItem li = gp1.AddCurve(null,ppl, colors[cluster.Id], SymbolType.Diamond);
                    li.Line.IsVisible = false;
                }
                PointPairList pplNoCl = new PointPairList();    
                foreach (Item item in Items)
                    if (item.Mark[0] == -1)
                        pplNoCl.Add(item.Attributes[x], item.Attributes[y]);
                LineItem liNoCl = gp1.AddCurve(null, pplNoCl, Color.Gray, SymbolType.Diamond);
                liNoCl.Line.IsVisible = false;

                foreach (Cluster cluster in Clusters2)
                {
                    PointPairList ppl = new PointPairList();
                    foreach (Item item in cluster.Items)
                        ppl.Add(item.Attributes[x], item.Attributes[y]);
                    LineItem li = gp2.AddCurve(null, ppl, colors[cluster.Id], SymbolType.Diamond);
                    li.Line.IsVisible = false;
                }
                PointPairList pplNoCl1 = new PointPairList();
                foreach (Item item in Items)
                    if (item.Mark[0] == -1)
                        pplNoCl.Add(item.Attributes[x], item.Attributes[y]);
                LineItem liNoCl1 = gp2.AddCurve(null, pplNoCl, Color.Gray, SymbolType.Diamond);
                liNoCl.Line.IsVisible = false;
            }

            public void ClusteriseKMeans(int k)
            {
                Clusters1.Clear();
                foreach (Item item in Items)
                    item.Mark[0] = -1;

                if (k >= ItemsCount || k < 2)
                {
                    MessageBox.Show("Invalid number of clusters");
                    return;
                }
                this.ClustersCount = k;
                Item curItem = Items[0];
                for (int i = 0; i < ClustersCount; i++)
                {
                    Cluster4Kmeans cluster = new Cluster4Kmeans(i, curItem);
                    Clusters1.Add(cluster);
                    cluster.AddItem(curItem);
                    //looking for farther item
                    double maxD = 0, d;
                    Item nextItem = Items[i];
                    foreach (Item item in Items)
                    {
                        if (item.Mark[0] != -1)
                                continue;
                        d = 0;
                        for (int j = 0; j < Clusters1.Count; j++)
                            d += item.d(Clusters1[j].Items[0]);
                        if (d > maxD)
                        {
                            maxD = d;
                            nextItem = item;
                        }
                        curItem = nextItem;
                    }
                }
                foreach (Item item in Items)
                {
                    if (item.Mark[0] != -1)
                        continue;
                    double minD = item.d(Clusters1[1].Center);
                    Cluster4Kmeans suggestedCluster = Clusters1[1];
                    foreach (Cluster4Kmeans cluster in Clusters1)
                    {
                        double d = item.d(cluster.Center);
                        if (d < minD)
                        {
                            suggestedCluster = cluster;
                            minD = d;
                        }

                    }
                    suggestedCluster.AddItem(item);
                }
            }

            public void ClesteriseHierarchy(int k)
            {
                this.Clusters2.Clear();
                foreach (Item item in Items)
                    item.Mark[1] = -1;

                if (k >= ItemsCount || k < 2)
                {
                    MessageBox.Show("Invalid number of clusters");
                    return;
                }
                this.ClustersCount = k;
                for (int i = 0; i < ItemsCount; i++)
                    Clusters2.Add(new Cluster4hierarchy(i, Items[i]));
                for (int i = 0; i < Clusters2.Count; i++)
                {
                    List<double> l = new List<double>();
                    for (int j = 0; j < Clusters2.Count; j++)
                        l.Add(Clusters2[i].D(Clusters2[j]));
                    matrix.Add(l);
                }
                while (Clusters2.Count > ClustersCount)
                {
                    double min = Clusters2[0].D(Clusters2[1]);
                    int indexI = 0, indexJ = 0;
                    for (int i = 0; i < Clusters2.Count; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (matrix[i][j] < min && i != j)
                            {
                                min = matrix[i][j];
                                indexI = i;
                                indexJ = j;
                            }
                        }
                    }
                    Clusters2[indexI].Merge(Clusters2[indexJ]);
                    Clusters2.RemoveAt(indexJ);
                   
                    List<double> h = matrix[indexJ];
                    List<double> l = matrix[indexI];
                    List<double> newDs = new List<double>();
                    for (int i = 0; i < matrix.Count; i++)
                    {
                        //matrix[indexI][i] = 
                        newDs.Add(
                            (Clusters2[indexI].Items.Count / (Clusters2[indexJ].Items.Count + Clusters2[indexI].Items.Count)) * matrix[indexI][i] 
                            + (Clusters2[indexJ].Items.Count / (Clusters2[indexJ].Items.Count + Clusters2[indexI].Items.Count)) * matrix[indexJ][i]);
                        //matrix[i][indexI] = matrix[indexI][i];
                    }
                    for (int i = 0; i < matrix.Count(); i++)
                            matrix[i].RemoveAt(indexJ);
                    matrix.RemoveAt(indexJ);
                    for (int i = 0; i < matrix.Count(); i++)
                    {
                        matrix[indexI][i] = matrix[i][indexI] = newDs[i];
                    }
                    for (int i = 0; i < this.Clusters2.Count; i++)
                        this.Clusters2[i].setId(i);
                };
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dh == null)
                return;
            dh.Standardize();
            dh.toDGV(dgv);
            Draw(1);
            Draw(2);
            Quality(dh.Clusters1, dh.Clusters2, richTextBox1, 1);
            Quality(dh.Clusters1, dh.Clusters2, richTextBox2, 2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dh == null)
                return;
            int k;
            try
            {
                k = int.Parse(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid number of clusters");
                return;
            }
            dh.ClusteriseKMeans(k);
            dh.MeansToDGV(dataGridView1, dataGridView2);
            dh.toDGV(dgv);
            Draw(1);
            Draw(2);
            Quality(dh.Clusters1, dh.Clusters2, richTextBox1, 1);
        }

        private void button4_Click(object sender, EventArgs e) //#2
        {
            if (dh == null)
                return;
            int k;
            try
            {
                k = int.Parse(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid number of clusters");
                return;
            }
            dh.ClesteriseHierarchy(k);
            dh.MeansToDGV(dataGridView1, dataGridView2);
            dh.toDGV(dgv);
            Draw(1);
            Draw(2);
            Quality(dh.Clusters1, dh.Clusters2, richTextBox2, 2);
        }

        private void Draw(int i)
        {
                int x, y;
            try
                {
                    x = (int)numericUpDown1.Value;
                    y = (int)numericUpDown2.Value;
                }
                catch (Exception)
                {
                    MessageBox.Show("Invalid x or y axis values");
                    return;
                }
            if (i == 1)
            {
                gp1.CurveList.Clear();
                gp1 = zedGraphControl1.GraphPane;
                dh.Draw(gp1, gp2, x, y);
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                zedGraphControl1.Refresh();
            } else
            {

            
            gp2.CurveList.Clear();
            gp2 = zedGraphControl2.GraphPane;

            dh.Draw(gp1, gp2, x, y);
           
            zedGraphControl2.AxisChange();
                zedGraphControl2.Invalidate();
                zedGraphControl2.Refresh();
}
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Draw(1);
            Draw(2);
        }

        private void Quality(List<Cluster4Kmeans> Clusters0, List<Cluster4hierarchy> Clusters2, RichTextBox rtb, int n)
        {
            Cluster[] Clusters1;
            if (n == 1)
                Clusters1 = Clusters0.ToArray();
            else
                Clusters1 = Clusters2.ToArray();
            double inside = 0, outside = 0;
            int count;

            foreach (Cluster c in Clusters1)
            {
                count = 0; double sum = 0;
                for (int i = 0; i < c.Items.Count; i++)
                {
                    for (int j = i; i < c.Items.Count; i++)
                    {
                        sum += c.Items[i].d(c.Items[j]);
                        count++;
                    }
                }
                inside += sum / count;
            }

            count = 0;
            for (int i = 0; i < Clusters1.Length; i++)
            {
                for (int j = i + 1; i < Clusters1.Length; i++)
                {
                    outside += Clusters1[i].D(Clusters1[i]);
                    count++;
                }
            }
            outside /= count;
            rtb.Text = "Average distance inside clusters: \n" + inside + "\n\nAverage distance between clusters: \n" + outside;
        }

    }
}

    */
