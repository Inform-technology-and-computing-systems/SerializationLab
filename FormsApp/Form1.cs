using System;
using PointLib;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;

namespace FormsApp
{
    public partial class Form1: Form
    {
        private Point[] points = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

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
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|YAML|*.yaml|Custom Format|*.txt";

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
                        var serializer = new SerializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
                        using (var writer = new StreamWriter(fs))
                        {
                            var yaml = serializer.Serialize(points);
                            writer.Write(yaml);
                        }
                        break;
                    case ".txt":
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.WriteLine("X;Y;Z"); // Заголовок
                            foreach (var point in points)
                            {
                                if (point is Point3D p3d)
                                    writer.WriteLine($"{p3d.X};{p3d.Y};{p3d.Z}");
                                else
                                    writer.WriteLine($"{point.X};{point.Y};0"); // Заполняем Z нулём для 2D-точек
                            }
                        }
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";
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
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .IgnoreUnmatchedProperties()
                            .Build(); //fdgfdgdf
                        using (var reader = new StreamReader(fs))
                        {
                            points = deserializer.Deserialize<Point[]>(reader);
                        }
                        break;
                    case ".txt":
                        using (var reader = new StreamReader(fs))
                        {
                            var lines = reader.ReadToEnd().Split("\n").Skip(1); // Пропускаем заголовок
                            var tempList = new List<Point>();

                            foreach (var line in lines)
                            {
                                var parts = line.Trim().Split(';');
                                if (parts.Length == 3 && int.TryParse(parts[0], out int x) &&
                                    int.TryParse(parts[1], out int y) && int.TryParse(parts[2], out int z))
                                {
                                    tempList.Add(z == 0 ? new Point(x, y) : new Point3D(x, y, z));
                                }
                            }
                            points = tempList.ToArray();
                        }
                        break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;
        }
    }
}
