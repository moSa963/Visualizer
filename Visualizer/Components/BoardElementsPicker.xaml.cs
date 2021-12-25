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

namespace Visualizer.Components
{
    public sealed partial class BoardElementsPicker : UserControl
    {
        public BoardElementsPicker()
        {
            InitializeComponent();
            ActiveBoardElementBtn = Wall;
            SetButtonActive(Wall);
            BoardElementType = BoardElement.ElementType.Wall;
        }

        private Button ActiveBoardElementBtn;
        public BoardElement.ElementType BoardElementType { get; private set; }
        private bool disabled;

        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;
                Start.IsEnabled = !disabled;
                Target.IsEnabled = !disabled;
                Wall.IsEnabled = !disabled;
                Remove.IsEnabled = !disabled;
            }
        }

        private void SetButtonActive(Button btn)
        {
            ActiveBoardElementBtn.Background = BoardElement.GetColor(0, 0, 0, 0);
            btn.Background = BoardElement.GetColor(255, 230, 230, 230);
            ActiveBoardElementBtn = btn;
        }

        private void BoardElement_Click(object sender, RoutedEventArgs e)
        {
            SetButtonActive((Button)sender);

            switch (((Button)sender).Name)
            {
                case "Start": BoardElementType = BoardElement.ElementType.Start; break;
                case "Target": BoardElementType = BoardElement.ElementType.Target; break;
                case "Wall": BoardElementType = BoardElement.ElementType.Wall; break;
                case "Remove": BoardElementType = BoardElement.ElementType.Empty; break;
                default: return;
            }
        }
    }
}
