using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    class MovingObject
    {
        public Vector4 position;

        public float c;

        private int steps;

        private int maxSteps;
        public MovingObject(float x, float y, float z)
        {
            position = new Vector4(x, y, z, 1);
            c = -0.1f;
            maxSteps = 10;
            steps = 10;
            
        }

        public void Move()
        {
            if (steps == 0)
            {
                c *= -1;
                steps = maxSteps;
            }
            else
            {                
                var y = position.Y += c;
                var z = position.Z;
                var x = position.X;

                position = new Vector4(x, y, z, 1);

                steps--;
            }
        }

    }
}
