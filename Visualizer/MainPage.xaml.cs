using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Visualizer.Canv;
using Visualizer.Models;
using Visualizer.Algorithms;
using Visualizer.Algorithms.DijkstraSp;
using Visualizer.Algorithms.AStarSp;


namespace Visualizer
{

    public sealed partial class MainPage : Page
    {
        private Board Board { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Board = new Board(MainCanvas, 73, 37);
            SetAlgorithm("");
            ToolsBar.AlgorithmChanged = SetAlgorithm;
            ToolsBar.Clear = Clear;
        }

        private void SetElement(Point pos)
        {
            if (!ToolsBar.Disabled)
            {
                Point2i index = Board.PointToIndex(pos);
                Board.Elements[index.Y, index.X].Type = ToolsBar.BoardElementType;
                Board.Animate(Board.Elements[index.Y, index.X]);
            }
        }

        private void SetAlgorithm(string name)
        {
            Runnable runnable;

            switch (name)
            {
                case "Dijkstra": runnable = new Dijkstra(Board); break;
                case "AStar": runnable = new AStar(Board); break;
                default: runnable = new MazeGenerator(Board); break;
            };

            ToolsBar.Runnable = runnable;

            ToolsBar.Runnable.SetMessage = SetMessage;
        }

        private void Clear(bool all)
        {
            if (all)
            {
                Board.Clear();
            }
            else
            {
                Board.ClearLastRun();
            }
        }

        private async void SetMessage(string message)
        {
            await MessageBlock.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MessageBlock.Text = message;
            });
        }

        private void MainCanvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {

        }

        private void MainCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint((UIElement)sender).Properties.IsLeftButtonPressed)
            {
                SetElement(e.GetCurrentPoint((UIElement)sender).Position);
            }
        }

        private void MainCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint((UIElement)sender).Properties.IsLeftButtonPressed)
            {
                SetElement(e.GetCurrentPoint((UIElement)sender).Position);
                e.Handled = true;
            }
            else if (e.GetCurrentPoint((UIElement)sender).Properties.IsMiddleButtonPressed)
            {

            }
        }

    }
}
