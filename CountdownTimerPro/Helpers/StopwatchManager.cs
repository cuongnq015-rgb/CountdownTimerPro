using System;
using System.Windows.Threading;

namespace CountdownTimerPro.Helpers
{
    public class StopwatchManager
    {
        private DispatcherTimer timer;

        public int ElapsedSeconds
        {
            get;
            private set;
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        public event Action<int> Tick;

        public StopwatchManager()
        {
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ElapsedSeconds++;

            if (Tick != null)
                Tick(ElapsedSeconds);
        }

        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            timer.Start();
        }

        public void Stop()
        {
            IsRunning = false;

            timer.Stop();
        }

        public void Reset()
        {
            Stop();

            ElapsedSeconds = 0;

            if (Tick != null)
                Tick(ElapsedSeconds);
        }
    }
}