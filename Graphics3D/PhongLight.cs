using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    class PhongLight
    {
        Vector3 observator;  //obserwator

        private double Ks = 0.25;
        private double Kd = 0.75;
        private int shiny = 50;

        Color Ia;
        List<(Vector4 pos, Color Ip)> lights;
        double Ka = 0.4;
        
        public PhongLight(double ka, double ks, double kd, int shiny, Color Ia, List<(Vector4 pos, Color Ip)> lights, Vector3 observator)
        {
            Ka = ka;
            Ks = ks;
            Kd = kd;
            this.shiny = shiny;
            this.Ia = Ia;
            this.lights = lights;
            this.observator = observator;
        }

        private int CheckColor(double i)
        {
            if (i < 0) return 0;
            else if (i > 255) return 255;
            else return (int)i;
        }

        public Color CalculateLight(Vector4 point, Vector3 N)
        {

            var r = Ka * Ia.R;
            var g = Ka * Ia.G;
            var b = Ka * Ia.B;
            Vector3 V = Vector3.Normalize(new Vector3(observator.X - point.X, observator.Y - point.Y, observator.Z - point.Z));


            for (int i = 0; i < lights.Count; ++i)
            {
                var l = lights[i].pos - point;
                var L = Vector3.Normalize(new Vector3(l.X, l.Y, l.Z));
                var Ip = lights[i].Ip;
                float lambertian = (float)Math.Max(Vector3.Dot(L, N), 0.0);
                float specular = 0.0f;
                if(lambertian > 0)
                {
                    Vector3 R = Vector3.Normalize(2 * lambertian * N - L);
                    //Vector3 R = Vector3.Normalize(Vector3.Reflect(L, N));

                    specular = (float)Math.Max(Vector3.Dot(R, V), 0.0);
                   
                }
                double wsp1 = Kd * lambertian;
                double wsp3 = Ks * Math.Pow(specular, shiny);
                r += Ip.R * wsp1 + Ip.R * wsp3;
                g += Ip.G * wsp1 + Ip.G * wsp3;
                b += Ip.B * wsp1 + Ip.B * wsp3;
            }

            return Color.FromArgb(CheckColor(r), CheckColor(g), CheckColor(b));

        }

    }
}
