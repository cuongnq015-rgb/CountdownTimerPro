using System;
using System.Windows.Threading;

namespace CountdownTimerPro.Helpers
{
    public class CountdownManager
    {
        private DispatcherTimer timer;

        public int TotalSeconds { get; private set; }

        public int RemainingSeconds { get; private set; }

        public bool AllowOvertime { get; set; }

        public bool IsRunning { get; private set; }

        public event Action<int> Tick;

        public event Action Finished;

        public CountdownManager(int seconds)
        {
            TotalSeconds = seconds;
            RemainingSeconds = seconds;
            AllowOvertime = false;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RemainingSeconds--;

            // Nếu không cho phép quá giờ
            if (!AllowOvertime && RemainingSeconds <= 0)
            {
                RemainingSeconds = 0;

                timer.Stop();

                IsRunning = false;

                Tick?.Invoke(RemainingSeconds);

                Finished?.Invoke();

                return;
            }

            Tick?.Invoke(RemainingSeconds);
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
            timer.Stop();
            IsRunning = false;
        }

        public void Reset()
        {
            Stop();

            RemainingSeconds = TotalSeconds;

            if (Tick != null)
                Tick(RemainingSeconds);
        }

        public void Resume()
        {
            Start();
        }

        public void SetTime(int minutes, int seconds)
        {
            Stop();

            TotalSeconds = minutes * 60 + seconds;

            RemainingSeconds = TotalSeconds;

            if (Tick != null)
                Tick(RemainingSeconds);
        }

        public void AddSeconds(int seconds)
        {
            if (!AllowOvertime)

                if (RemainingSeconds > TotalSeconds)
                RemainingSeconds = TotalSeconds;

            if (Tick != null)
                Tick(RemainingSeconds);
        }

        public void MinusSeconds(int seconds)
        {
            if (!AllowOvertime)

                if (RemainingSeconds < 0)
                RemainingSeconds = 0;

            if (Tick != null)
                Tick(RemainingSeconds);
        }
    }
}