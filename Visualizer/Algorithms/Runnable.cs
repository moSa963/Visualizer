using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace Visualizer.Algorithms
{
    public abstract class Runnable
    {
        public enum States
        {
            Stopped, Paused, Running
        }


        public States State { get; private set; } //state controlled by the class
        public States CurrentState { get; private set; } //state controled by the user and the class

        private int speed;
        public int Speed
        {
            get => speed;
            set
            {
                speed = value;
                SpeedChanged();
            }
        }

        //executed when the task is completed
        public Action Finshed;
        public Action<string> SetMessage { get; set; }

        public Runnable()
        {
            State = States.Stopped;
            CurrentState = States.Stopped;
            Speed = 100;
        }

        public virtual async void Run()
        {
            if (State != States.Running)
            {
                if (State != States.Paused)
                {
                    //if the initilation failes dont run
                    if (!Init())
                    {
                        State = States.Stopped;
                        CurrentState = States.Stopped;
                        Finshed?.Invoke();
                        return;
                    }
                }

                State = States.Running;
                CurrentState = States.Running;

                Progress<Action> progress = new Progress<Action>((a) => a.Invoke());
                await Task.Run(() => Worker(progress));

                //if the task ends with a pause request
                if (CurrentState == States.Paused)
                {
                    State = States.Paused;
                    return;
                }

                //if the task ends for any reason other than pause

                Clean();

                //if task stop but "CurrentState" still running means that the task reached the end
                if (CurrentState == States.Running)
                {
                    Finshed?.Invoke();
                }

                State = States.Stopped;
                CurrentState = States.Stopped;
            }
        }

        public virtual bool Stop()
        {
            if (State == States.Running)
            {
                CurrentState = States.Stopped;
                return true;
            }
            else if (State == States.Paused)
            {
                CurrentState = States.Stopped;
                State = States.Stopped;
                return true;
            }
            return false;
        }

        public virtual bool Pause()
        {
            if (State == States.Running)
            {
                CurrentState = States.Paused;
                return true;
            }
            return false;
        }

        private void Worker(IProgress<Action> progress)
        {
            DateTime now = DateTime.Now;
            int counter = 0;

            while (CurrentState == States.Running)
            {
                //if speed bigger than 25 just use normal sleep
                if (Speed >= 25)
                {
                    counter = 0;
                    Thread.Sleep(Speed);
                }
                else // sleep function for less than 25 usually is not guaranteed, so use timespan with sleep
                {
                    if (counter % (25 - Speed) == 0)
                    {
                        counter = 0;
                        Thread.Sleep(Speed);
                    }
                    else if (DateTime.Now - now < TimeSpan.FromMilliseconds(Speed))
                    {
                        now = DateTime.Now;
                        continue;
                    }

                    ++counter;
                }

                
                if (!Tick(progress))
                {
                    break;
                }
            }
        }

        //method executed before the loop start
        protected abstract bool Init();

        //function that run for each iteration
        protected abstract bool Tick(IProgress<Action> progress);

        //method executed after the loop ends
        protected virtual void Clean() { }

        //method executed when "speed" get changed
        protected virtual void SpeedChanged() { }
    }
}
