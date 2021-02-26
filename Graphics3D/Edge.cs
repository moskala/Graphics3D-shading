using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics3D
{
    class Edge : IComparable<Edge>
    {
        Point p;
        Point q;
        public decimal currX;
        public decimal mParameter { get; set; }

        public Edge(Point p, Point q)
        {
            this.p = p;
            this.q = q;
            currX = p.X;
            mParameter = setM();
        }

        public void updateX()
        {
            if (mParameter == 0) return;
            currX += 1 / mParameter;
        }

        public decimal setM()
        {
            if (p.X == q.X) return 0;
            return (decimal)(p.Y - q.Y) / (decimal)(p.X - q.X);
        }

        public bool SameEdge(Point p1, Point p2)
        {
            if (p == p1 && q == p2) return true;
            if (p == p2 && q == p1) return true;
            return false;
        }

        // Default comparer for Edge type.
        public int CompareTo(Edge compareEdge)
        {
            return this.currX.CompareTo(compareEdge.currX);
        }

    }
}
