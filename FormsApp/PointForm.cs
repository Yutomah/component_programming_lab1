using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using PointLib;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using YamlDotNet;
namespace FormsApp
{
    public partial class PointForm : Form
    {
        private Point[] points = null;
        public PointForm()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];
            var rnd = new Random();
            for (int i = 0; i < points.Length; i++) {
                points[i] = rnd.Next(3) == 0 ? new Point() : new Point3D();
            }
            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (points == null)
                return;
            Array.Sort(points);

            listBox.DataSource = null;
            listBox.DataSource = points;
        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|Yaml|*.yaml|MySer|*.myser";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        var jf = new JsonSerializer();
                        using (var w = new StreamWriter(fs))
                            jf.Serialize(w, points);
                        break;
                    case ".yaml":
                        var yf = new YamlDotNet.Serialization.Serializer();
                        using (var w2 = new StreamWriter(fs))
                        {
                            var str = yf.Serialize(points, typeof(Point3D[]));
                            w2.Write(str);
                        }
                        break;
                    case ".myser":
                        mySerialization(points, fs);
                        break;
                }
            }

        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|Yaml|*.yaml|MySer|*.myser";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xf.Deserialize(fs);
                        break;
                    case ".json":
                        var jf = new JsonSerializer();
                        using (var r = new StreamReader(fs))
                            points = (Point[])jf.Deserialize(r, typeof(Point[]));
                        break;
                    case ".yaml":
                        var yf = new YamlDotNet.Serialization.Deserializer();
                        using (var r2 = new StreamReader(fs)) {
                            points = (Point3D[])yf.Deserialize(r2, typeof(Point3D[]));
                        }
                        break;
                    case ".myser":
                        points = myDeserialization(fs);
                        break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;

        }
        private void mySerialization(Point[] points, FileStream fs)
        {
            
            var w = new StreamWriter(fs);
            for (int i = 0; i < points.Length; i++)
            {
                string str = "";
                if (points[i].GetType() == typeof(Point))
                {
                    str += points[i].X.ToString() + " " + points[i].Y.ToString();
                }
                else
                {
                    Point3D k = (Point3D)points[i];
                    str += points[i].X.ToString() + " " + points[i].Y.ToString() + " " + k.Z.ToString();
                }
                w.WriteLine(str);
            }
            w.Close();
        }
        private Point[] myDeserialization(FileStream fs)
        {
            Point[] points = new Point[5];
            var r = new StreamReader(fs);
            string line;
            int i = 0;
            while ((line = r.ReadLine()) != null)
            {
                string[] XYZ = line.Split(' ');
                if (XYZ.Length > 2)
                    points[i] = new Point3D(Convert.ToInt32(XYZ[0]), 
                        Convert.ToInt32(XYZ[1]), Convert.ToInt32(XYZ[2]));
                else
                    points[i] = new Point(Convert.ToInt32(XYZ[0]), Convert.ToInt32(XYZ[1]));
                i++;
            }
            r.Close();
            return points;
        }
    }
    
   
}
