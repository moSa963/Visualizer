using System;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Visualizer.Models;


namespace Visualizer.Canv
{
    public class BoardElement
    {
        public enum ElementType
        {
            Empty = 0,
            Target = 1,
            Start = 2,
            Wall = 3,
            Path = 4,
            Visited = 5,
        }

        private Action<BoardElement, ElementType> ElementChanged { get; }

        public Shape UiElement { get; }
        public Point2i Position { get; }
        private ElementType type;

        public ElementType Type
        {
            get => type;
            set
            {
                //if the new type not the same as the old one
                if (type != value)
                {
                    ElementType oldType = type;
                    type = value;

                    ElementChanged?.Invoke(this, oldType);

                    if (UiElement.Dispatcher.HasThreadAccess)
                    {
                        UiElement.Fill = DefaultBrush(type);
                    }
                    else
                    {
                        SetBrushAsync();
                    }
                }
            }
        }

        public async void SetBrushAsync()
        {
            await UiElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UiElement.Fill = DefaultBrush(type);
            });
        }

        public async void SetBrushAsync(byte a, byte r, byte g, byte b)
        {
            await UiElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UiElement.Fill = GetColor(a, r, g, b);
            });
        }

        public async void SetBrushAsync(string resaurceName)
        {
            await UiElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UiElement.Fill = GetImageBrush(resaurceName);
            });
        }

        public Brush Brush
        {
            get => UiElement.Fill;
            set => UiElement.Fill = value;
        }

        public BoardElement(Point2i pos, Shape uIElement, Action<BoardElement, ElementType> elementChanged)
        {
            Position = pos;
            UiElement = uIElement;
            Type = ElementType.Empty;
            ElementChanged = elementChanged;
        }

        //get a brush from argb
        public static Brush GetColor(byte a, byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static Brush GetImageBrush(string name)
        {
            return new ImageBrush() { ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/" + name)) };
        }

        //get the default brush of a specific type
        public static Brush DefaultBrush(ElementType type)
        {
            switch(type)
            {
                case ElementType.Empty: return GetColor(0, 0, 0, 0);
                case ElementType.Start: return GetImageBrush("arrow.png");
                case ElementType.Target: return GetImageBrush("target.png");
                case ElementType.Wall: return GetColor(255, 0, 0, 0);
                case ElementType.Path: return GetColor(255, 150, 150, 0);
                case ElementType.Visited: return GetColor(255, 0, 150, 150);
                default: return null;
            };
        }
    }
}
