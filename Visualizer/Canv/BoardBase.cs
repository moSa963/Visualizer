using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Visualizer.Models;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace Visualizer.Canv
{
    public abstract class BoardBase
    {
        public Canvas Canv { get; }
        public int X { get; }
        public int Y { get; }
        protected double OffsetX;
        protected double OffsetY;
        public BoardElement[,] Elements;
        public double Scale { get; private set; }

        public BoardBase(Canvas canv, int x, int y)
        {
            Canv = canv;
            X = x;
            Y = y;
            OffsetX = 0;
            OffsetY = 0;
            Scale = 1;
            Init();
        }

        private void Init()
        {
            Canv.Children.Clear();
            Elements = new BoardElement[Y, X];
            OffsetX = Canv.Width / X;
            OffsetY = Canv.Height / Y;


            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y; ++j)
                {
                    Rectangle rect = CreateRect(new Point2i(i, j));
                    _ = AnimationBuilder.Create()
                        .CenterPoint(new Vector3((float)OffsetX / 2, (float)OffsetY / 2, 0), null, null, TimeSpan.FromMilliseconds(1))
                        .Scale(new Vector3(0.95f, 0.95f, 0), null, null, TimeSpan.FromMilliseconds(1))
                        .StartAsync(rect);

                    Canv.Children.Add(rect);
                    Elements[j, i] = new BoardElement(new Point2i(i, j), rect, ElementChanged);
                }
            }

            //create the grid vertical lines
            for (int i = 0; i <= X; ++i)
            {
                Line l = new Line()
                {
                    StrokeThickness = .5,
                    X1 = OffsetX * i,
                    Y1 = 0,
                    X2 = OffsetX * i,
                    Y2 = Canv.Height,
                    Stroke = BoardElement.GetColor(100, 0, 0, 0)
                };

                Canv.Children.Add(l);
            }

            //create the grid horizontal lines
            for (int i = 0; i <= Y; ++i)
            {
                Line l = new Line()
                {
                    StrokeThickness = 0.5,
                    X1 = 0,
                    Y1 = OffsetY * i,
                    X2 = Canv.Width,
                    Y2 = OffsetY * i,
                    Stroke = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0))
                };

                Canv.Children.Add(l);
            }
        }

        private Rectangle CreateRect(Point2i pos)
        {
            Rectangle rect = new Rectangle()
            {
                Width = OffsetX,
                Height = OffsetY,
            };

            Canvas.SetLeft(rect, OffsetX * pos.X);
            Canvas.SetTop(rect, OffsetY * pos.Y);

            return rect;
        }

        protected abstract void ElementChanged(BoardElement element, BoardElement.ElementType oldType);

        public void Clear()
        {
            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y; ++j)
                {
                    //Elements[j, i].UiElement.Fill = BoardElement.DefaultBrush(BoardElement.ElementType.Wall);
                    //StartAnimation(Elements[j, i].UiElement, 1);
                    Elements[j, i].Type = BoardElement.ElementType.Empty;
                }
            }
        }

        public void ClearLastRun()
        {
            for (int i = 0; i < X; ++i)
            {
                for (int j = 0; j < Y; ++j)
                {
                    if (Elements[j, i].Type != BoardElement.ElementType.Start
                        && Elements[j, i].Type != BoardElement.ElementType.Wall
                        && Elements[j, i].Type != BoardElement.ElementType.Target)
                    {
                        Elements[j, i].Type = BoardElement.ElementType.Empty;
                    }
                }
            }
        }

        //get a cell index from a point on the board
        public Point2i PointToIndex(Point p)
        {
            int newX = Math.Max(Math.Min((int)Math.Floor(p.X / OffsetX), X - 1), 0);
            int newY = Math.Max(Math.Min((int)Math.Floor(p.Y / OffsetY), Y - 1), 0);
            Point2i index = new Point2i(newX, newY);
            return index;
        }
    }
}
