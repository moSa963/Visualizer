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
using Windows.UI.Xaml.Media.Imaging;


namespace Visualizer.Components
{
    public sealed partial class RunController : UserControl
    {
        public enum States
        {
            Running = 1,
            Stopped = 0,
            Paused = 2,
        }

        private States state;

        public Action<States> StateChanged { get; set; }

        public States State
        {
            get => state;
            set
            {
                switch (value)
                {
                    case States.Stopped: Cancel(); break;
                    case States.Running: Run(); break;
                    case States.Paused: Pause(); break;
                    default: return;
                }

                StateChanged?.Invoke(state);
            }
        }

        public RunController()
        {
            InitializeComponent();
            state = States.Stopped;
            Btn1.Background = BoardElement.GetColor(50, 0, 255, 0);
            Btn2.Background = BoardElement.GetColor(50, 255, 0, 0);
            Btn2.IsEnabled = false;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            switch (ToolTipService.GetToolTip((Button)sender).ToString())
            {
                case "Cancel": State = States.Stopped; break;
                case "Resume":
                case "Run": State = States.Running; break;
                case "Pause": State = States.Paused; break;
                default: return;
            }
        }

        private void Cancel()
        {
            if (state != States.Stopped)
            {
                state = States.Stopped;
                ToolTipService.SetToolTip(Btn1, "Run");
                ((Image)Btn1.Content).Source = new BitmapImage(new Uri("ms-appx:///Assets/start.png"));
                Btn1.Background = BoardElement.GetColor(50, 0, 255, 0);
                Btn2.IsEnabled = false;
            }
        }

        private void Run()
        {
            if (state != States.Running)
            {
                state = States.Running;

                ToolTipService.SetToolTip(Btn1, "Pause");
                ((Image)Btn1.Content).Source = new BitmapImage(new Uri("ms-appx:///Assets/pause.png"));
                Btn1.Background = BoardElement.GetColor(0, 0, 0, 0);

                Btn2.IsEnabled = true;
            }
        }

        private void Pause()
        {
            if (state == States.Running)
            {
                state = States.Paused;

                ToolTipService.SetToolTip(Btn1, "Resume");
                ((Image)Btn1.Content).Source = new BitmapImage(new Uri("ms-appx:///Assets/start.png", UriKind.Absolute));
                Btn1.Background = BoardElement.GetColor(50, 0, 255, 0);

                Btn2.IsEnabled = true;
            }
        }

    }
}
