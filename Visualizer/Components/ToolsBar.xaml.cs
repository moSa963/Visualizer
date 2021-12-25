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
using System.Diagnostics;

namespace Visualizer.Components
{
    public sealed partial class ToolsBar : UserControl
    {
        public BoardElement.ElementType BoardElementType => BoardElementsPicker.BoardElementType;
        private Runnable runnable;
        public Action<string> AlgorithmChanged { get; set; }
        public Action<bool> Clear { get; set; }

        public Runnable Runnable
        {
            get => runnable;
            set
            {
                runnable = value;
                runnable.Speed = (int)SpeedPicker.Value;
                runnable.Finshed = () =>
                {
                    Disabled = false;
                    RunCont.State = RunController.States.Stopped;
                };
            }
        }

        private bool disabled;

        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;
                BoardElementsPicker.Disabled = disabled;
                AlgorithmCombo.IsEnabled = !disabled;
                ClearBtn.IsEnabled = !disabled;
                ClearLastRunBtn.IsEnabled = !disabled;
            }
        }

        public ToolsBar()
        {
            InitializeComponent();
            RunCont.StateChanged = StateChanged;
        }

        private void StateChanged(RunController.States state)
        {
            switch (state)
            {
                case RunController.States.Running: Disabled = true; Runnable?.Run(); break;
                case RunController.States.Paused: _ = Runnable?.Pause(); break;
                case RunController.States.Stopped: Disabled = false; _ = Runnable?.Stop(); break;
                default:
                    break;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear?.Invoke(true);
        }

        private void ClearLastRun_Click(object sender, RoutedEventArgs e)
        {
            Clear?.Invoke(false);
        }

        private void AlgorithmCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlgorithmChanged?.Invoke(((ComboBoxItem)AlgorithmCombo.SelectedItem).Name);
        }

        private void SpeedPicker_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (runnable != null)
            {
                runnable.Speed = (int)e.NewValue;
            }
        }
    }
}
