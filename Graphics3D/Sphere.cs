using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    class Sphere
    {
        public double Radius { get; set; }
        public int DivX { get; set; }
        public int DivY { get; set; }

        public List<Triangle> triangles;

        public List<MyVertex> vertices;

        MyVertex bottomVertex;

        MyVertex[,] vert;

        public Sphere(double radius, int divX, int divY)
        {
            Radius = radius;
            DivX = divX;
            DivY = divY;
            bottomVertex = null;
            vertices = new List<MyVertex>();
            GenerateSphere();
        }

        private void GenerateSphere()
        {
            double dx = 2 * Math.PI / DivX;
            double dy = Math.PI / DivY;
            vert = new MyVertex[DivY, DivX];
  
            // Tworzenie punktow sfery
            for(int j = 0; j < DivY; ++j)
            {
                for(int i = 0; i < DivX; ++i)
                {
                    var x = Radius * Math.Sin(j * dy) * Math.Cos(i * dx);
                    var y = Radius * Math.Cos(j * dy);
                    var z = Radius * Math.Sin(j * dy) * Math.Sin(i * dx);
                    vert[j, i] = new MyVertex(x, y, z);
                    vert[j, i].normal = Vector3.Normalize(new Vector3((float)x, (float)y, (float)z));
                    vertices.Add(vert[j, i]);
                }                    
            }

            if (vertices.Last().oryginalCoordinates.Y != -Radius)
            {
                bottomVertex = new MyVertex(0, -Radius, 0);
                bottomVertex.normal = Vector3.Normalize(new Vector3(0, -(float)Radius, 0));
                vertices.Add(bottomVertex);
            }


        }

        public void GenerateTriangles()
        {
            // Tworzenie trojkatow dla tej sfery
            triangles = new List<Triangle>();

            for (int j = 0; j < DivY - 1; ++j)
            {
                for (int i = 0; i < DivX; ++i)
                {
                    int nextX = (i + 1) % DivX;
                    int prevX = i - 1 < 0 ? DivX - 1 : i - 1;
                    triangles.Add(new Triangle(vert[j, i], vert[j, nextX], vert[j + 1, i])); // i prawo skos
                    triangles.Add(new Triangle(vert[j, i], vert[j + 1, i], vert[j + 1, prevX])); // i dol lewo       
                }
            }

            if(bottomVertex != null)
            {
                int lastRow = DivY - 1;

                for (int i = 0; i < DivX; ++i)
                {
                    int nextX = (i + 1) % DivX;

                    triangles.Add(new Triangle(vert[lastRow, i], vert[lastRow, nextX], bottomVertex)); // i prawo skos
                }
            }

            
        }

    }
}
