using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    class Cube
    { 

        public List<Triangle> triangles;

        public List<MyVertex> vertices;

        public Cube()
        {
            triangles = new List<Triangle>();
            vertices = new List<MyVertex>();
            GenerateCube();
        }

        private void GenerateCube()
        {

            // Dolna sciana
            triangles.Add(new Triangle(new MyVertex(0, 0, 0), new MyVertex(1, 0, 0), new MyVertex(1, 0, 1)));
            triangles.Last().vertexOne.normal = new Vector3(0, -1, 0);
            triangles.Last().vertexTwo.normal = new Vector3(0, -1, 0);
            triangles.Last().vertexThree.normal = new Vector3(0, -1, 0);

            triangles.Add(new Triangle(new MyVertex(0, 0, 0), new MyVertex(1, 0, 1), new MyVertex(0, 0, 1)));
            triangles.Last().vertexOne.normal = new Vector3(0, -1, 0);
            triangles.Last().vertexTwo.normal = new Vector3(0, -1, 0);
            triangles.Last().vertexThree.normal = new Vector3(0, -1, 0);

            // Gorna sciana
            triangles.Add(new Triangle(new MyVertex(0, 1, 0), new MyVertex(1, 1, 1), new MyVertex(1, 1, 0)));
            triangles.Last().vertexOne.normal = new Vector3(0, 1, 0);
            triangles.Last().vertexTwo.normal = new Vector3(0, 1, 0);
            triangles.Last().vertexThree.normal = new Vector3(0, 1, 0);

            triangles.Add(new Triangle(new MyVertex(0, 1, 0), new MyVertex(0, 1, 1), new MyVertex(1, 1, 1)));
            triangles.Last().vertexOne.normal = new Vector3(0, 1, 0);
            triangles.Last().vertexTwo.normal = new Vector3(0, 1, 0);
            triangles.Last().vertexThree.normal = new Vector3(0, 1, 0);

            // Przednia sciana
            triangles.Add(new Triangle(new MyVertex(0, 1, 1), new MyVertex(1, 0, 1), new MyVertex(1, 1, 1)));
            triangles.Last().vertexOne.normal = new Vector3(0, 0, 1);
            triangles.Last().vertexTwo.normal = new Vector3(0, 0, 1);
            triangles.Last().vertexThree.normal = new Vector3(0, 0, 1);

            triangles.Add(new Triangle(new MyVertex(0, 1, 1), new MyVertex(0, 0, 1), new MyVertex(1, 0, 1)));
            triangles.Last().vertexOne.normal = new Vector3(0, 0, 1);
            triangles.Last().vertexTwo.normal = new Vector3(0, 0, 1);
            triangles.Last().vertexThree.normal = new Vector3(0, 0, 1);

            // Tylnia sciana
            triangles.Add(new Triangle(new MyVertex(0, 1, 0), new MyVertex(1, 1, 0), new MyVertex(1, 0, 0)));
            triangles.Last().vertexOne.normal = new Vector3(0, 0, -1);
            triangles.Last().vertexTwo.normal = new Vector3(0, 0, -1);
            triangles.Last().vertexThree.normal = new Vector3(0, 0, -1);

            triangles.Add(new Triangle(new MyVertex(0, 1, 0), new MyVertex(1, 0, 0), new MyVertex(0, 0, 0)));
            triangles.Last().vertexOne.normal = new Vector3(0, 0, -1);
            triangles.Last().vertexTwo.normal = new Vector3(0, 0, -1);
            triangles.Last().vertexThree.normal = new Vector3(0, 0, -1);

            // Prawa sciana
            triangles.Add(new Triangle(new MyVertex(1, 1, 1), new MyVertex(1, 0, 0), new MyVertex(1, 1, 0)));
            triangles.Last().vertexOne.normal = new Vector3(1, 0, 0);
            triangles.Last().vertexTwo.normal = new Vector3(1, 0, 0);
            triangles.Last().vertexThree.normal = new Vector3(1, 0, 0);

            triangles.Add(new Triangle(new MyVertex(1, 1, 1), new MyVertex(1, 0, 1), new MyVertex(1, 0, 0)));
            triangles.Last().vertexOne.normal = new Vector3(1, 0, 0);
            triangles.Last().vertexTwo.normal = new Vector3(1, 0, 0);
            triangles.Last().vertexThree.normal = new Vector3(1, 0, 0);

            // Lewa sciana
            triangles.Add(new Triangle(new MyVertex(0, 1, 0), new MyVertex(0, 0, 1), new MyVertex(0, 1, 1)));
            triangles.Last().vertexOne.normal = new Vector3(-1, 0, 0);
            triangles.Last().vertexTwo.normal = new Vector3(-1, 0, 0);
            triangles.Last().vertexThree.normal = new Vector3(-1, 0, 0);

            triangles.Add(new Triangle(new MyVertex(0, 1, 0), new MyVertex(0, 0, 0), new MyVertex(0, 0, 1)));
            triangles.Last().vertexOne.normal = new Vector3(-1, 0, 0);
            triangles.Last().vertexTwo.normal = new Vector3(-1, 0, 0);
            triangles.Last().vertexThree.normal = new Vector3(-1, 0, 0);


        }
    }
}
