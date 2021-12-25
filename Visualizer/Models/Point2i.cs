using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer.Models
{
    public class Point2i
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2i()
        {
            X = 0;
            Y = 0;
        }

        public Point2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point2i p1, Point2i p2)
        {
            return p1?.X == p2?.X && p1?.Y == p2?.Y;
        }

        public static bool operator !=(Point2i p1, Point2i p2)
        {
            return p1?.X != p2?.X || p1?.Y != p2?.Y;
        }

        public override bool Equals(object p)
        {
            return p != null && (ReferenceEquals(this, p) || ((X == ((Point2i)p).X) && (Y == ((Point2i)p).Y)));
        }

        public override int GetHashCode()
        {
            return (X, Y).GetHashCode();
        }
    }
}
