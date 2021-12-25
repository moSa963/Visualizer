using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizer.Models;

namespace Visualizer.Algorithms.AStarSp
{
    public class Node : IComparable<Node>
    {
        public double F => Cost + Estimate;

        public double Cost { get; set; }
        public double Estimate { get; set; }

        public Node Previous { get; set; }
        public Point2i Position { get; set; }

        public Node(Point2i pos)
        {
            Position = pos;
            Cost = 0;
            Estimate = 0;
            Previous = null;
        }

        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }
    }
}
