using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clusterisation
{
    class Obj
    {
        public List<double> Attributes { get; set; }

        public int KMeansClusterID { get; set; }
        public int HierarchyClusterID { get; set; }

        public Obj(string[] AttributesStr)
        {
            this.Attributes = new List<double>();
            try
            {
                foreach (string attr in AttributesStr)
                {
                    double val = Double.Parse(attr.Replace('.', ','));
                    Attributes.Add(val);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error had place while parsing data" + ex.Message);
                return;
            }
        }
        public Obj(List<double> Attributes)
        {
            this.Attributes = new List<double>();
            this.Attributes.AddRange(Attributes);
            KMeansClusterID = -1;
            HierarchyClusterID = -1;
        }

        public double D(Obj o)
        {
            double d = 0;
            for (int i = 0; i < Attributes.Count; i++)
                d += Math.Pow((Attributes[i] - o.Attributes[i]), 2);
            return Math.Sqrt(d);
        }

        public double D(Obj o, int excludeAttr)
        {
            double d = 0;
            for (int i = 0; i < Attributes.Count; i++)
            {
                if (i == excludeAttr) continue;
                d += Math.Pow((Attributes[i] - o.Attributes[i]), 2);
            }
            return Math.Sqrt(d);
        }

    }
}
