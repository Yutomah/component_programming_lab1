using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointLib;
using System.Threading.Tasks;

namespace FormsApp
{
    [Serializable]
    public class Point3D: Point
    {
        public int Z { get; set; }

        public Point3D():base()
        {
            Z = rnd.Next(10);
        }
        public Point3D(int X, int Y, int Z) : base(X, Y)
        {
            this.Z = Z;
        }
        public override double Metric()
        {
            return Math.Sqrt(X*X + Y* Y + Z * Z); 
        }
        public override string ToString()
        {
            return string.Format($"({X}, {Y}, {Z})");
        }
    }
}
