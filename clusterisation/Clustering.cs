using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clusterisation
{
    static class Clustering
    {
        private static double eps = 0.1;

        public static void Standardize (DataSet ds)
        {
            double[] averages = new double[ds.AttributesCount];
            double[] devs = new double[ds.AttributesCount];
            for (int i = 0; i < ds.AttributesCount; i++)
            {
                double average = 0, dev = 0;
                for (int j = 0; j < ds.Count; j++)
                    average += ds.Objects[j].Attributes[i];
                averages[i] = average / ds.Count;
                for (int j = 0; j < ds.Count; j++)
                    dev += Math.Pow(ds.Objects[j].Attributes[i] - average, 2);
                devs[i] = Math.Sqrt(dev / (ds.Count - 1));
            }
            for (int i = 0; i < ds.Count; i++)
                for (int j = 0; j < ds.AttributesCount; j++)
                {
                    ds.Objects[i].Attributes[j] -= averages[j];
                    ds.Objects[i].Attributes[j] /= devs[j];
                }
        }

        public static List<Cluster> Hierarchy(DataSet ds, int clusterCount)
        {
            List<Cluster> clusters = new List<Cluster>();
            for (int i = 0; i < ds.Count; i++)
            {
                Cluster c = new Cluster(i);
                c.Objects.Add(ds.Objects[i]);
                clusters.Add(c);
            }
            double[,] matrix = new double[ds.Count, ds.Count];

            for (int i = 0; i < ds.Count; i++)
                for (int j = 0; j < i; j++)
                {
                    double d =  clusters[i].D(clusters[j]);
                    matrix[i, j] = d;
                }

            //while ( clusters.Count > 1)
            while (clusters.Count > clusterCount)
            {
                double min = clusters[0].D(clusters[1]);
                int cl1 = 0, cl2 = 0;

                for (int i = 0; i < clusters.Count; i++)
                    for (int j = 0; j < i; j++)
                    {
                        double d = clusters[i].D(clusters[j]);
                        matrix[i, j] = d;
                        if (d < min)
                        {
                            min = d;
                            cl1 = i;
                            cl2 = j;
                        }
                    }
                for (int i = 0; i < cl1; i++)
                {
                    double alpha1 = clusters[cl1].Count / (clusters[cl1].Count + clusters[cl2].Count);
                    double alpha2 = clusters[cl2].Count / (clusters[cl1].Count + clusters[cl2].Count);
                    matrix[cl1, i] = alpha1 * matrix[cl1, i] + alpha2 * matrix[cl2, i];
                }
                for (int i = cl1+1; i < clusters.Count; i++)
                {
                    double alpha1 = clusters[cl1].Count / (clusters[cl1].Count + clusters[cl2].Count);
                    double alpha2 = clusters[cl2].Count / (clusters[cl1].Count + clusters[cl2].Count);
                    matrix[i, cl1] = alpha1 * matrix[cl1, cl1] + alpha2 * matrix[i, cl1];
                }

                matrix = TrimArray(cl2, cl2, matrix);
                clusters[cl1].Objects.AddRange(clusters[cl2].Objects);
                clusters.RemoveAt(cl2);
                for (int i = 0; i < clusters.Count; i++)
                    clusters[i].Id = i;
            }
            foreach (Cluster c in clusters)
                foreach (Obj o in c.Objects)
                    o.HierarchyClusterID = c.Id;

            return clusters;    
        }


        private static double[,] TrimArray(int rowToRemove, int columnToRemove, double[,] originalArray)
        {
            double[,] result = new double[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }
        
        public static List<KMCluster> KMeans(DataSet ds, int clusterCount)
        {
            List<KMCluster> clusters = new List<KMCluster>();
            Obj curItem = ds.Objects[0];
            for (int i = 0; i < clusterCount; i++)
            {
                KMCluster cluster = new KMCluster(i, curItem);
                clusters.Add(cluster);
                cluster.Objects.Add(curItem);

                double maxD = 0;
                foreach (Obj o in ds.Objects)
                {
                    double d = 0;
                    foreach (KMCluster c in clusters)
                        d += o.D(c.Center);
                    if (d > maxD)
                    {
                        maxD = d;
                        curItem = o;
                    }
                }
            }

            foreach (KMCluster c in clusters)
                c.Objects.Clear();

            int count = 0;
            Obj[] oldcenters = new Obj[clusterCount];
            bool stop = false;
            while (count < 9999 && !stop)
            {
                foreach (Obj o in ds.Objects)
                {
                    double minD = clusters[0].Center.D(o);
                    KMCluster cl = clusters[0];
                    foreach (KMCluster c in clusters)
                    {
                        if (c.Center.D(o) < minD)
                        {
                            minD = c.Center.D(o);
                            cl = c;
                        }
                    }
                    cl.Add(o);
                }
                count++;
                for (int i = 0; i < clusterCount; i++)
                {
                    oldcenters[i] = clusters[i].Center;
                    clusters[i].setCenter();
                }
                stop = true;
                for (int i = 0; i < clusterCount; i++)
                    for (int j = 0; j < ds.AttributesCount; j++)
                        if (Math.Abs(clusters[i].Center.Attributes[j] - oldcenters[i].Attributes[j]) > eps) stop = false;

            }
            foreach (KMCluster c in clusters)
                c.Objects.Clear();
            foreach (Obj o in ds.Objects)
            {
                double minD = clusters[0].Center.D(o);
                KMCluster cl = clusters[0];
                foreach (KMCluster c in clusters)
                {
                    if (c.Center.D(o) < minD)
                    {
                        minD = c.Center.D(o);
                        cl = c;
                    }
                }
                cl.Add(o);
                o.KMeansClusterID = cl.Id;
            }
            return clusters;
        }

        public static List<KMCluster> KMeans(DataSet ds, int clusterCount, int excludeAttr) // without 1 attr 
        {
            List<KMCluster> clusters = new List<KMCluster>();
            Obj curItem = ds.Objects[0];
            for (int i = 0; i < clusterCount; i++)
            {
                KMCluster cluster = new KMCluster(i, curItem);
                clusters.Add(cluster);
                cluster.Objects.Add(curItem);

                double maxD = 0;
                foreach (Obj o in ds.Objects)
                {
                    double d = 0;
                    foreach (KMCluster c in clusters)
                        d += o.D(c.Center, excludeAttr);
                    if (d > maxD)
                    {
                        maxD = d;
                        curItem = o;
                    }
                }
            }

            foreach (KMCluster c in clusters)
                c.Objects.Clear();

            int count = 0;
            Obj[] oldcenters = new Obj[clusterCount];
            bool stop = false;
            while (count < 9999 && !stop)
            {
                foreach (Obj o in ds.Objects)
                {
                    double minD = clusters[0].Center.D(o, excludeAttr);
                    KMCluster cl = clusters[0];
                    foreach (KMCluster c in clusters)
                    {
                        if (c.Center.D(o, excludeAttr) < minD)
                        {
                            minD = c.Center.D(o, excludeAttr);
                            cl = c;
                        }
                    }
                    cl.Add(o);
                }
                count++;
                for (int i = 0; i < clusterCount; i++)
                {
                    oldcenters[i] = clusters[i].Center;
                    clusters[i].setCenter();
                }
                stop = true;
                for (int i = 0; i < clusterCount; i++)
                    for (int j = 0; j < ds.AttributesCount; j++)
                    {
                        if (j == excludeAttr)
                            continue;
                        if (Math.Abs(clusters[i].Center.Attributes[j] - oldcenters[i].Attributes[j]) > eps) 
                            stop = false;
                    }
  

            }
            foreach (KMCluster c in clusters)
                c.Objects.Clear();
            foreach (Obj o in ds.Objects)
            {
                double minD = clusters[0].Center.D(o, excludeAttr);
                KMCluster cl = clusters[0];
                foreach (KMCluster c in clusters)
                {
                    if (c.Center.D(o, excludeAttr) < minD)
                    {
                        minD = c.Center.D(o, excludeAttr);
                        cl = c;
                    }
                }
                cl.Add(o);
                o.KMeansClusterID = cl.Id;
            }
            return clusters;
        } 

    }






    class Cluster
    {
        public int Id { get; set; }
        public List<Obj> Objects { get; set; }
        public int Count
        {
            get { return Objects.Count; }
        }

        public Cluster (int id)
        {
            Id = id;
            Objects = new List<Obj>();
        }

        public double D (Cluster c)
        {
            double sum = 0;
            foreach (Obj o1 in this.Objects)
                foreach (Obj o2 in c.Objects)
                    sum += o1.D(o2);
            return sum / (this.Objects.Count * c.Objects.Count);
        }
        public double D(Cluster c, int excludeAttr)
        {
            double sum = 0;
            foreach (Obj o1 in this.Objects)
                foreach (Obj o2 in c.Objects)
                    sum += o1.D(o2, excludeAttr);
            return sum / (this.Objects.Count * c.Objects.Count);
        }
    }

    class KMCluster : Cluster
    {
        public Obj Center { get; set; }

        public KMCluster(int id, Obj o) : base(id) {
            Center = o;
            Objects = new List<Obj>();
        }

        public void Add(Obj o)
        {
            this.Objects.Add(o);
            //setCenter();
        }

        public void setCenter()
        {
            List<double> list = new List<double>();
            for (int i = 0; i < Center.Attributes.Count; i++)
            {
                double tmp = 0;
                foreach (Obj o in Objects)
                    tmp += o.Attributes[i];
                list.Add(tmp / Objects.Count);
            }
            Center = new Obj(list);
        }


    }

}

