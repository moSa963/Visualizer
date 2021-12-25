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
    public class MazeGenerator : Runnable
    {
        private Stack<BoardElement> stack;
        public Board Board { get; }
        private Point2i start;

        public MazeGenerator(Board board) : base()
        {
            Board = board;
        }

        protected override bool Init()
        {
            if (Board.Start == null)
            {
                SetMessage("Please enter starting point");
                return false;
            }

            stack = new Stack<BoardElement>();
            start = Board.Start;

            Board.Clear();

            for (int y = 0; y < Board.Y; ++y)
            {
                for (int x = 0; x < Board.X; ++x)
                {
                    Board.Elements[y, x].Type = BoardElement.ElementType.Wall;
                }
            }

            Board.Elements[start.Y, start.X].Type = BoardElement.ElementType.Start;
            stack.Push(Board.Elements[start.Y, start.X]);
            return true;
        }

        protected override bool Tick(IProgress<Action> progressd)
        {
            if (stack.Count > 0)
            {
                BoardElement current = stack.Pop();

                Board.Highlight = current;

                List<BoardElement> neighbors = GetNeighbors(current);

                if (neighbors.Count != 0)
                {
                    stack.Push(current);

                    BoardElement rn = neighbors[new Random().Next(neighbors.Count)];

                    int x = rn.Position.X != current.Position.X ? (rn.Position.X > current.Position.X ? rn.Position.X - 1 : rn.Position.X + 1) : rn.Position.X;
                    int y = rn.Position.Y != current.Position.Y ? (rn.Position.Y > current.Position.Y ? rn.Position.Y - 1 : rn.Position.Y + 1) : rn.Position.Y;

                    Board.Elements[y, x].Type = BoardElement.ElementType.Empty;
                    rn.Type = BoardElement.ElementType.Empty;

                    stack.Push(rn);
                }

                return true;
            }
            return false;
        }

        protected override void Clean()
        {
            Board.Highlight = null;
        }

        private List<BoardElement> GetNeighbors(BoardElement node)
        {
            List<BoardElement> neighbors = new List<BoardElement>(4);

            for (int i = -2; i < 3; ++i)
            {
                for (int j = -2; j < 3; ++j)
                {
                    if ((Math.Abs(i) == 2 || Math.Abs(j) == 2) && !(Math.Abs(i) == 2 && Math.Abs(j) == 2) && !(Math.Abs(i) == 1 || Math.Abs(j) == 1))
                    {
                        int y = node.Position.Y + i;
                        int x = node.Position.X + j;

                        if (y >= 0 && x >= 0 && y < Board.Y && x < Board.X && Board.Elements[y, x].Type == BoardElement.ElementType.Wall)
                        {
                            Board.Animate(Board.Elements[y, x], Speed * 5);
                            neighbors.Add(Board.Elements[y, x]);
                        }
                    }
                }
            }
            return neighbors;
        }
    }
}
