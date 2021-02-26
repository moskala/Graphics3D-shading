using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    interface IShading
    {
        void StartNewTriangle(Triangle triangle);
        Color GetShadingColor((double alfa, double beta, double gamma) bary);
    }

    class FlatShading : IShading
    {
        Triangle triangle;
        PhongLight phongLight;
        Color colorForTriangle;

        public FlatShading(PhongLight phongLight)
        {
            this.phongLight = phongLight;
        }
        public Color GetShadingColor((double alfa, double beta, double gamma) bary)
        {
            return colorForTriangle;
        }

        public void StartNewTriangle(Triangle triangle)
        {
            this.triangle = triangle;
            var normal = triangle.GetNormalToTraingle();

            if(normal == Vector3.Zero)
            {
                colorForTriangle = Color.Black;
                return;
            }
            var point = new Vector4(
                triangle.vertexOne.worldCoordinates.X,
                triangle.vertexOne.worldCoordinates.Y,
                triangle.vertexOne.worldCoordinates.Z,
                1);
            colorForTriangle = phongLight.CalculateLight(point, normal);
        }
    }

    class GouraudShading : IShading
    {
        Triangle triangle;
        PhongLight phongLight;
        Color colorA;
        Color colorB;
        Color colorC;

        public GouraudShading(PhongLight phongLight)
        {
            this.phongLight = phongLight;
        }

        public Color GetShadingColor((double alfa, double beta, double gamma) bary)
        {
            var red = bary.alfa * colorA.R + bary.beta * colorB.R + bary.gamma * colorC.R;
            var green = bary.alfa * colorA.G + bary.beta * colorB.G + bary.gamma * colorC.G;
            var blue = bary.alfa * colorA.B + bary.beta * colorB.B + bary.gamma * colorC.B;

            red = red < 0 ? 0 : (red > 255 ? 255 : red);
            green = green < 0 ? 0 : (green > 255 ? 255 : green);
            blue = blue < 0 ? 0 : (blue > 255 ? 255 : blue);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }

        public void StartNewTriangle(Triangle triangle)
        {
            this.triangle = triangle;
            colorA = phongLight.CalculateLight(triangle.vertexOne.worldCoordinates, triangle.vertexOne.normal);
            colorB = phongLight.CalculateLight(triangle.vertexTwo.worldCoordinates, triangle.vertexTwo.normal);
            colorC = phongLight.CalculateLight(triangle.vertexThree.worldCoordinates, triangle.vertexThree.normal);
        }
    }

    class PhongShading : IShading
    {
        Triangle triangle;
        PhongLight phongLight;

        public PhongShading(PhongLight phongLight)
        { 
            this.phongLight = phongLight;
        }

        public Color GetShadingColor((double alfa, double beta, double gamma) bary)
        {
            var norm1 = triangle.vertexOne.normal;
            var norm2 = triangle.vertexTwo.normal;
            var norm3 = triangle.vertexThree.normal;            

            double dx = bary.alfa * norm1.X + bary.beta * norm2.X + bary.gamma * norm3.X;
            double dy = bary.alfa * norm1.Y + bary.beta * norm2.Y + bary.gamma * norm3.Y;
            double dz = bary.alfa * norm1.Z + bary.beta * norm2.Z + bary.gamma * norm3.Z;

            var a = triangle.vertexOne.worldCoordinates;
            var b = triangle.vertexTwo.worldCoordinates;
            var c = triangle.vertexThree.worldCoordinates;

            var xV = bary.alfa * a.X + bary.beta * b.X + bary.gamma * c.X;
            var yV = bary.alfa * a.Y + bary.beta * b.Y + bary.gamma * c.Y;
            var zV = bary.alfa * a.Z + bary.beta * b.Z + bary.gamma * c.Z;


            var normal = Vector3.Normalize(new Vector3((float)dx, (float)dy, (float)dz));

            var point = new Vector4(
                (float)xV,
                (float)yV,
                (float)zV,
                1);
            return phongLight.CalculateLight(point, normal);
        }

        public void StartNewTriangle(Triangle triangle)
        {
            this.triangle = triangle;
        }
    }
}
