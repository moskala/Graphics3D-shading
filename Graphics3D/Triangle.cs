using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    class Triangle
    {
        public MyVertex vertexOne;
        public MyVertex vertexTwo;
        public MyVertex vertexThree;

        public Triangle(MyVertex a, MyVertex b, MyVertex c)
        {
            vertexOne = a;
            vertexTwo = b;
            vertexThree = c;
        }

        public List<Point> GetPointsFromTriangle()
        {
            return new List<Point>()
            {
                vertexOne.GetPointFromVertex(),
                vertexTwo.GetPointFromVertex(),
                vertexThree.GetPointFromVertex()
            };
        }

        public Vector3 GetNormalToTraingle()
        {
            var edge1 = new Vector3(
                vertexTwo.worldCoordinates.X - vertexOne.worldCoordinates.X,
                vertexTwo.worldCoordinates.Y - vertexOne.worldCoordinates.Y,
                vertexTwo.worldCoordinates.Z - vertexOne.worldCoordinates.Z
                );

            var edge2 = new Vector3(
                vertexThree.worldCoordinates.X - vertexOne.worldCoordinates.X,
                vertexThree.worldCoordinates.Y - vertexOne.worldCoordinates.Y,
                vertexThree.worldCoordinates.Z - vertexOne.worldCoordinates.Z
                );

            if (edge1 == Vector3.Zero || edge2 == Vector3.Zero)
                return Vector3.Zero;

            return Vector3.Normalize(Vector3.Cross(Vector3.Normalize(edge1), Vector3.Normalize(edge2)));
        }

    }
}
