using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizer.Models;
using System.Diagnostics;
using Visualizer.Canv;

namespace Visualizer.Algorithms.DijkstraSp
{
    public class Dijkstra : Runnable
    {
        public Board Board { get; set; }
        private PriorityQueue<Node> Queue { get; set; }
        private Node[,] Distances { get; set; }
        private Node Target { get; set; }
        private Point2i Path { get; set; }

        public Dijkstra(Board Board) : base()
        {
            this.Board = Board;
        }

        protected override bool Init()
        {
            if (Board.Start == null)
            {
                SetMessage("Please enter starting point");
                return false;
            }

            Target = null;
            Queue = new PriorityQueue<Node>((e1, e2) => e1.CompareTo(e2));
            Distances = new Node[Board.Y, Board.X];

            for (int i = 0; i < Board.Y; ++i)
            {
                for (int j = 0; j < Board.X; ++j)
                {
                    Node n = new Node(i, j);
                    Distances[i, j] = n;
                }
            }

            Distances[Board.Start.Y, Board.Start.X].Distance = 0;
            Queue.Add(Distances[Board.Start.Y, Board.Start.X]);
            return true;
        }

        private bool ShowPath(IProgress<Action> progress)
        {

            Target = Distances[Target.Position.Y, Target.Position.X].Previous;
            Path = Target.Position;

            if (Path != Board.Start)
            {
                Board.Elements[Path.Y, Path.X].Type = BoardElement.ElementType.Path;
                Path = Distances[Path.Y, Path.X].Previous.Position;
                return true;
            } 
            else
            {
                Target = null;
                return false;
            }
        }

        private void Neighbors(Node node)
        {
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (i != 0 || j != 0)
                    {
                        int y = node.Position.Y + i;
                        int x = node.Position.X + j;

                        if (y >= 0 && x >= 0 && y < Board.Y && x < Board.X && Board.Elements[y, x].Type != BoardElement.ElementType.Wall)
                        {
                            double alt = node.Distance + GetLength(node.Position, Distances[y, x].Position);

                            if (alt < Distances[y, x].Distance)
                            {
                                Distances[y, x].Distance = alt;
                                Distances[y, x].Previous = node;

                                if (Queue.Find(e => e == Distances[y, x]) == null)
                                {
                                    Queue.Add(Distances[y, x]);
                                    Board.Animate(Board.Elements[y, x]);
                                }
                                else
                                {
                                    Queue.PriorityChanged(Distances[y, x]);
                                    Board.Animate(Board.Elements[y, x]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static double GetLength(Point2i pos1, Point2i pos2)
        {
            return Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }

        protected override bool Tick(IProgress<Action> progress)
        {
            if (Queue.Count > 0)
            {
                Node sml = Queue.PopFirst();

                if (Board.Elements[sml.Position.Y, sml.Position.X].Type == BoardElement.ElementType.Target)
                {
                    Target = sml;
                    Queue.Clear();
                    return true;
                }

                if (sml.Distance != 0)
                {
                    Board.Elements[sml.Position.Y, sml.Position.X].Type = BoardElement.ElementType.Visited;
                    //Board.Animate(Board.Elements[sml.Position.Y, sml.Position.X], 300);
                }

                Neighbors(sml);
                return true;
            }
            else if (Target != null)
            {
                return ShowPath(progress);
            }
            return false;
        }
    }
}
