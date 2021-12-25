using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizer.Models;

namespace Visualizer.Algorithms.DijkstraSp
{
    public class Node : IComparable
    {
        public double Distance { get; set; }
        public Node Previous { get; set; }
        public Point2i Position { get; set; }

        public Node(int y, int x)
        {
            Position = new Point2i(x, y);
            Distance = double.MaxValue;
            Previous = null;
        }

        public int CompareTo(object obj)
        {
            return Distance.CompareTo(((Node)obj).Distance);
        }
    }
}
