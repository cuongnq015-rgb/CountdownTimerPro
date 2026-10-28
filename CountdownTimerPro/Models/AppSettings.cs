using System;

namespace CountdownTimerPro.Models
{
    [Serializable]
    public class AppSettings
    {
        // Countdown
        public int Minutes { get; set; } = 5;

        public int Seconds { get; set; } = 0;

        // Window
        public bool TopMost { get; set; } = true;

        public bool FullScreen { get; set; } = false;

        public bool MiniMode { get; set; } = false;

        // Effects
        public bool EnableBeep { get; set; } = true;

        public bool EnableBlink { get; set; } = true;

        // Countdown
        public bool AllowOvertime { get; set; } = false;

        // Window Position
        public double WindowLeft { get; set; } = 100;

        public double WindowTop { get; set; } = 100;

        public double WindowWidth { get; set; } = 760;

        public double WindowHeight { get; set; } = 500;
    }
}