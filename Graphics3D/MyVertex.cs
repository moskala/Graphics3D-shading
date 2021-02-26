using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Graphics3D
{
    public class MyVertex
    {
        public Vector4 transformedCoordinates;

        public Vector4 oryginalCoordinates;

        public Vector4 worldCoordinates;

        public Vector3 normal;

        public MyVertex(double x, double y, double z)
        {
            oryginalCoordinates = new Vector4((float)x, (float)y, (float)z, 1);        
        }

        public MyVertex(MyVertex v)
        {
            oryginalCoordinates = new Vector4(v.oryginalCoordinates.X, v.oryginalCoordinates.Y, v.oryginalCoordinates.Z, 1);
            normal = new Vector3(v.normal.X, v.normal.Y, v.normal.Z);
        }

        public System.Drawing.Point GetPointFromVertex()
        {
            return new System.Drawing.Point((int)transformedCoordinates.X, (int)transformedCoordinates.Y);
        }
    }
}
