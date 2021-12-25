using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizer.Canv;
using Visualizer.Models;
using System.Diagnostics;

namespace Visualizer.Algorithms.AStarSp
{
    public class AStar : Runnable
    {
        private Board Board { get; }
        private PriorityQueue<Node> Open { get; set; }
        private Point2i Target { get; set; }
        private FindTarget FindTarget { get; set; }

        public AStar(Board board) : base()
        {
            Board = board;
            FindTarget = new FindTarget(Board)
            {
                SetMessage = SetMessage,
                Speed = Speed,
            };
            FindTarget.Finshed = () =>
            {
                Run();
            };
        }

        public override void Run()
        {
            if (FindTarget.Target == null)
            {
                FindTarget.Run();
                return;
            }
            Target = FindTarget.Target;
            base.Run();
        }

        public override bool Pause()
        {
            return FindTarget.Pause() || base.Pause();
        }

        public override bool Stop()
        {
            return FindTarget.Stop() || base.Stop();
        }

        protected override bool Init()
        {
            if (Board.Start == null)
            {
                SetMessage("Please enter starting point");
                return false;
            }

            if (FindTarget?.Target == null)
            {
                SetMessage("Could not find the target");
                return false;
            }

            Board.ClearLastRun();


            Open = new PriorityQueue<Node>((n1, n2) => n1.CompareTo(n2));

            Open.Add(new Node(Board.Start)
            {
                Estimate = 0,
                Cost = 0,
            });

            return true;
        }

        protected override bool Tick(IProgress<Action> progress)
        {
            if (Open.Count > 0)
            {
                Node element = Open.PopFirst();

                if (element.Position == Target)
                {
                    for (Node c = element.Previous; c?.Previous != null; c = c.Previous)
                    {
                        Board.Elements[c.Position.Y, c.Position.X].Type = BoardElement.ElementType.Path;
                    }
                    SetMessage("Target successfully found");
                    return false;
                }

                List<Node> neighbors = GetNeighbors(element);

                foreach(Node node in neighbors)
                {
                    Node exisitOpen = Open.Find(n => n.Position == node.Position);

                    if (exisitOpen == null)
                    {
                        Open.Add(node);
                    }
                    else if (node.Cost < exisitOpen.Cost)
                    {
                        exisitOpen.Cost = node.Cost;
                        exisitOpen.Previous = node.Previous;
                        Open.PriorityChanged(exisitOpen);
                    }
                }
                if (Board.Elements[element.Position.Y, element.Position.X].Type != BoardElement.ElementType.Start)
                {
                    Board.Elements[element.Position.Y, element.Position.X].Type = BoardElement.ElementType.Visited;
                    double r = element.Cost / element.Estimate;

                    Board.Elements[element.Position.Y, element.Position.X].SetBrushAsync(200,
                        (byte)((element.Estimate > 30 ? 1 : element.Estimate / 30) * 255),
                        0,
                        (byte)((element.Cost > 30 ? 1 : element.Cost / 30) * 255));

                }

                return true;
            }
            return false;
        }


        private List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>(8);
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (i != 0 || j != 0)
                    {
                        int y = node.Position.Y + i;
                        int x = node.Position.X + j;

                        if (y >= 0 && x >= 0 && y < Board.Y && x < Board.X && (Board.Elements[y, x].Type == BoardElement.ElementType.Empty || Board.Elements[y, x].Type == BoardElement.ElementType.Target))
                        {
                            neighbors.Add(new Node(new Point2i(x, y))
                            {
                                Estimate = GetLength(new Point2i(x, y), Target),
                                Cost = GetLength(new Point2i(x, y), node.Position) + node.Cost,
                                Previous = node
                            });
                        }
                    }
                }
            }
            return neighbors;
        }

        protected override void SpeedChanged()
        {
            if (FindTarget != null)
            {
                FindTarget.Speed = Speed;
            }
        }

        protected override void Clean()
        {
            FindTarget.Target = null;
            base.Clean();
        }

        public static double GetLength(Point2i pos1, Point2i pos2)
        {
            return Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }
    }
}