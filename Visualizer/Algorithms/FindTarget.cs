using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizer.Canv;
using Visualizer.Models;
using System.Diagnostics;

namespace Visualizer.Algorithms
{
    public class FindTarget : Runnable
    {
        private Queue<BoardElement> queue;
        private Board Board { get; set; }
        public Point2i Target { get; set; }

        public FindTarget(Board board) : base()
        {
            Board = board;
            Target = null;
            Speed = 100;
        }

        protected override bool Init()
        {
            if (Board.Start == null)
            {
                SetMessage("Please enter starting point");
                return false;
            }
            Target = null;
            queue = new Queue<BoardElement>();
            queue.Enqueue(Board.Elements[Board.Start.Y, Board.Start.X]);
            return true;
        }

        protected override bool Tick(IProgress<Action> progress)
        {
            if (queue.Count > 0)
            {
                BoardElement element = queue.Dequeue();
                
                return Neighbors(progress, element);
            }
            return false;
        }

        private bool Neighbors(IProgress<Action> progress, BoardElement node)
        {
            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (i != 0 || j != 0)
                    {
                        int y = node.Position.Y + i;
                        int x = node.Position.X + j;

                        if (y >= 0 && x >= 0 && y < Board.Y && x < Board.X)
                        {
                            if (Board.Elements[y, x].Type == BoardElement.ElementType.Target)
                            {
                                Target = new Point2i(x, y);
                                return false;
                            }
                            else if (Board.Elements[y, x].Type == BoardElement.ElementType.Empty)
                            {
                                queue.Enqueue(Board.Elements[y, x]);

                                progress.Report(() =>
                                {
                                    Board.Elements[y, x].Brush = BoardElement.DefaultBrush(BoardElement.ElementType.Visited);
                                });
                                Board.Elements[y, x].Type = BoardElement.ElementType.Visited;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
