using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clusterisation
{
    class DataSet : ICloneable
    {
        public List<Obj> Objects { get; set; }
        public List<Cluster> Hierarchy { get; set; }
        public List<KMCluster> KMeans{ get; set; }

        public double[] Quality
        {
            get
            {
                double[] quality = new double[2];
                double inside = 0, outside = 0;
                int count;
                foreach (Cluster c in Hierarchy)
                {
                    count = 0; double sum = 0;
                    for (int i = 0; i < c.Objects.Count; i++)
                        for (int j = i; i < c.Objects.Count; i++)
                        {
                            sum += c.Objects[i].D(c.Objects[j]);
                            count++;
                        }
                    inside += sum / count;
                }
                count = 0;
                for (int i = 0; i < Hierarchy.Count; i++)
                    for (int j = i + 1; i < Hierarchy.Count; i++)
                    {
                        outside += Hierarchy[i].D(Hierarchy[i]);
                        count++;
                    }
                outside /= count;
                quality[0] = inside / outside;
                inside = 0;
                outside = 0;
                foreach (KMCluster c in KMeans)
                {
                    count = 0; double sum = 0;
                    for (int i = 0; i < c.Objects.Count; i++)
                        for (int j = i; i < c.Objects.Count; i++)
                        {
                            sum += c.Objects[i].D(c.Objects[j]);
                            count++;
                        }
                    inside += sum / count;
                }
                count = 0;
                for (int i = 0; i < KMeans.Count; i++)
                    for (int j = i + 1; i < KMeans.Count; i++)
                    {
                        outside += KMeans[i].D(KMeans[i]);
                        count++;
                    }
                outside /= count;
                quality[1] = inside / outside;
                return quality;
            }
        }

        public static double QualityExcl(List<KMCluster> KMeans, int excludeAttr) {
            double inside = 0, outside = 0;
            int count;
            foreach (KMCluster c in KMeans)
            {
                count = 0; double sum = 0;
                for (int i = 0; i < c.Objects.Count; i++)
                    for (int j = i; i < c.Objects.Count; i++)
                    {
                        sum += c.Objects[i].D(c.Objects[j], excludeAttr);
                        count++;
                    }
                inside += sum / count;
            }
            count = 0;
            for (int i = 0; i < KMeans.Count; i++)
                for (int j = i + 1; i < KMeans.Count; i++)
                {
                    outside += KMeans[i].D(KMeans[i], excludeAttr);
                    count++;
                }
            outside /= count;
            return inside / outside;
        }


        public int AttributesCount {
            get { return (Objects.Count > 0) ? (Objects[0].Attributes.Count) : 0; }
        }
        public int Count {
            get { return Objects.Count; }
        }

        public void RemoveAttr(int i)
        {
            if (i > AttributesCount)
            {
                MessageBox.Show("Invalid attribute index to remove");
                return;
            }
            foreach (Obj o in Objects)
                o.Attributes.RemoveAt(i);
        }

        public DataSet()
        {
            Objects = new List<Obj>();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
