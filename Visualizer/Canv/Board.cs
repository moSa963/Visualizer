using System;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Visualizer.Models;
using Microsoft.Toolkit.Uwp.UI.Animations;


namespace Visualizer.Canv
{
    public class Board : BoardBase
    {
        private Point2i Target { get; set; }
        public Point2i Start { get; private set; }
        private BoardElement highlight;

        public BoardElement Highlight
        {
            get => highlight;
            set
            {
                if (highlight != null)
                {
                    highlight.SetBrushAsync();
                }

                highlight = value;

                if (highlight != null)
                {
                    highlight.SetBrushAsync(255, 255, 0, 0);
                }
            }
        }

        public Board(Canvas canv, int x, int y) : base(canv, x, y)
        {
            Start = null;
            Target = null;
        }

        public async void Animate(BoardElement element, int milis = 300)
        {
            if (element.UiElement.Dispatcher.HasThreadAccess)
            {
                await AnimationBuilder.Create()
                   .Scale(new Vector3(0.95f, 0.95f, 0), new Vector3(0.3f, 0.3f, 0), null, TimeSpan.FromMilliseconds(milis))
                   .StartAsync(element.UiElement);
            }
            else
            {
                await element.UiElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    AnimationBuilder.Create()
                       .Scale(new Vector3(0.95f, 0.95f, 0), new Vector3(0.3f, 0.3f, 0), null, TimeSpan.FromMilliseconds(milis))
                       .Start(element.UiElement);
                });
            }
        }

        protected override void ElementChanged(BoardElement element, BoardElement.ElementType oldType)
        {
            if (oldType == BoardElement.ElementType.Start)
            {
                Start = null;
            }
            else if (oldType == BoardElement.ElementType.Target)
            {
                Target = null;
            }

            //if the new type is start or target delete the old start or target
            //so there cant be two element of these types on the board at the same time
            if (element.Type == BoardElement.ElementType.Start)
            {
                if (Start != null)
                {
                    Elements[Start.Y, Start.X].Type = BoardElement.ElementType.Empty;
                }
                Start = element.Position;
            }
            else if (element.Type == BoardElement.ElementType.Target)
            {
                if (Target != null)
                {
                    Elements[Target.Y, Target.X].Type = BoardElement.ElementType.Empty;
                }
                Target = element.Position;
            }
        }
    }
}
