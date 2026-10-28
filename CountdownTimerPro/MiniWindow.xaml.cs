using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CountdownTimerPro
{
    public partial class MiniWindow : Window
    {
        private DispatcherTimer flashTimer;

        private bool flashOn = false;
        private bool isFinished = false;
        //--------------------------------------------------
        // Fields
        //--------------------------------------------------

        private MainWindow ownerWindow;

        private bool isStopwatchMode = false;

        //--------------------------------------------------
        // Constructor
        //--------------------------------------------------

        public MiniWindow(MainWindow owner)
        {
            InitializeComponent();

            ownerWindow = owner;

            flashTimer = new DispatcherTimer();

            flashTimer.Interval = TimeSpan.FromMilliseconds(500);

            flashTimer.Tick += FlashTimer_Tick;

            SetMode(false);

            SetReady();

        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            flashOn = !flashOn;

            if (flashOn)
            {
                txtTitle.Foreground = Brushes.Red;

                MainBorder.BorderBrush = Brushes.Red;

                txtStatus.Foreground = Brushes.Red;
            }
            else
            {
                txtTitle.Foreground = Brushes.Gold;

                MainBorder.BorderBrush = Brushes.Gold;

                txtStatus.Foreground = Brushes.Gold;
            }
        }

        //--------------------------------------------------
        // Cho MainWindow gọi
        //--------------------------------------------------

        public void SetTopMost(bool topMost)
        {
            Topmost = topMost;
        }

        //--------------------------------------------------
        // Chuyển chế độ
        //--------------------------------------------------

        public void SetMode(bool stopwatchMode)
        {
            isStopwatchMode = stopwatchMode;

            // Nếu đang hiển thị HẾT GIỜ thì không cho SetMode ghi đè
            if (isFinished)
            {
                if (isStopwatchMode)
                {
                    txtPercent.Visibility = Visibility.Collapsed;
                    progressFill.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtPercent.Visibility = Visibility.Visible;
                    progressFill.Visibility = Visibility.Visible;
                }

                return;
            }

            if (isStopwatchMode)
            {
                txtTitle.Text = "⏱ STOPWATCH";

                txtPercent.Visibility =
                    Visibility.Collapsed;

                progressFill.Visibility =
                    Visibility.Collapsed;
            }
            else
            {
                txtTitle.Text = "⏳ COUNTDOWN";

                txtPercent.Visibility =
                    Visibility.Visible;

                progressFill.Visibility =
                    Visibility.Visible;
            }

            ResetColor();
        }
        public void ClearFinished()
        {
            isFinished = false;

            flashTimer.Stop();

            txtTitle.Foreground = Brushes.White;

            txtStatus.Foreground = Brushes.LimeGreen;

            MainBorder.BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(0, 200, 83));

            SetMode(isStopwatchMode);
        }

        //--------------------------------------------------
        // Reset giao diện màu
        //--------------------------------------------------

        private void ResetColor()
        {
            Brush brush = Brushes.LimeGreen;

            txtMiniTime.Foreground = brush;

            txtStatus.Foreground = brush;

            MainBorder.BorderBrush = brush;

            progressFill.Background = brush;
        }

        //--------------------------------------------------
        // Countdown
        //--------------------------------------------------

        public void UpdateCountdown(int remain,int total,bool running,bool allowOvertime)
        {
            Dispatcher.Invoke(delegate
            {
                // Nếu đang ở trạng thái HẾT GIỜ thì không cập nhật nữa
                if (isFinished)
                    return;

                TimeSpan ts =
                    TimeSpan.FromSeconds(
                        Math.Abs(remain));

                string text =
                    ts.ToString(@"mm\:ss");

                if (remain < 0)
                {
                    text = "-" + text;
                }

                txtMiniTime.Text = text;

                UpdateProgress(remain, total);

                UpdateCountdownColor(remain);

                UpdateCountdownStatus(
                    remain,
                    running,
                    allowOvertime);
            });
        }

        //--------------------------------------------------
        // Stopwatch
        //--------------------------------------------------

        public void UpdateStopwatch(
            int elapsed,
            bool running)
        {
            Dispatcher.Invoke(delegate
            {
                if (isFinished)
                    return;
                TimeSpan ts =
                    TimeSpan.FromSeconds(elapsed);

                txtMiniTime.Text =
                    ts.ToString(@"hh\:mm\:ss");

                txtMiniTime.Foreground =
                    Brushes.LimeGreen;

                txtStatus.Foreground =
                    Brushes.LimeGreen;

                MainBorder.BorderBrush =
                    Brushes.LimeGreen;

                UpdateStopwatchStatus(running);
            });
        }

        //--------------------------------------------------
        // Progress
        //--------------------------------------------------

        private void UpdateProgress(
            int remain,
            int total)
        {
            if (isStopwatchMode)
                return;

            if (remain < 0)
            {
                progressFill.Width = 0;

                txtPercent.Text = "OVERTIME";

                return;
            }

            if (total <= 0)
            {
                progressFill.Width = 0;

                txtPercent.Text = "0%";

                return;
            }

            double percent =
                remain * 100.0 / total;

            if (percent < 0)
                percent = 0;

            if (percent > 100)
                percent = 100;

            txtPercent.Text =
                ((int)percent).ToString() + "%";

            double maxWidth = 224;

            progressFill.Width =
                maxWidth * percent / 100.0;
        }

        //--------------------------------------------------
        // Countdown Color
        //--------------------------------------------------

        private void UpdateCountdownColor(int remain)
        {
            Brush brush;

            if (remain < 0)
            {
                brush =
                    Brushes.MediumVioletRed;
            }
            else if (remain <= 10)
            {
                brush =
                    Brushes.Red;
            }
            else if (remain <= 30)
            {
                brush =
                    Brushes.Orange;
            }
            else
            {
                brush =
                    Brushes.LimeGreen;
            }

            txtMiniTime.Foreground =
                brush;

            txtStatus.Foreground =
                brush;

            MainBorder.BorderBrush =
                brush;

            progressFill.Background =
                brush;
        }

        //--------------------------------------------------
        // Countdown Status
        //--------------------------------------------------

        private void UpdateCountdownStatus(
            int remain,
            bool running,
            bool allowOvertime)
        {
            if (remain < 0)
            {
                txtStatus.Text = "OVERTIME";
                txtStatus.Foreground = Brushes.MediumVioletRed;
                return;
            }

            if (remain == 0 && !allowOvertime)
            {
                txtStatus.Text = "FINISHED";
                txtStatus.Foreground = Brushes.Red;
                return;
            }

            if (running)
            {
                txtStatus.Text = "RUNNING";
                return;
            }

            txtStatus.Text = "READY";
        }

        //--------------------------------------------------
        // Stopwatch Status
        //--------------------------------------------------

        private void UpdateStopwatchStatus(bool running)
        {
            if (running)
            {
                txtStatus.Text = "RUNNING";
            }
            else
            {
                txtStatus.Text = "READY";
            }
        }

        //--------------------------------------------------
        // Ready
        //--------------------------------------------------

        public void SetReady()
        {
            txtStatus.Text = "READY";

            txtStatus.Foreground =
                Brushes.LimeGreen;

            ResetColor();
        }

        //--------------------------------------------------
        // Finished
        //--------------------------------------------------

        public void SetFinished()
        {
            txtStatus.Text = "FINISHED";

            txtStatus.Foreground =
                Brushes.Red;

            txtMiniTime.Foreground =
                Brushes.Red;

            MainBorder.BorderBrush =
                Brushes.Red;

            progressFill.Background =
                Brushes.Red;
        }

        //--------------------------------------------------
        // Reset Mini Window
        //--------------------------------------------------

        public void Reset()
        {
            if (isStopwatchMode)
            {
                UpdateStopwatch(0, false);
            }
            else
            {
                UpdateCountdown(0, 1, false, false);
            }

            SetReady();
        }

        //--------------------------------------------------
        // Mouse
        //--------------------------------------------------

        private void Window_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ownerWindow.Show();

                ownerWindow.WindowState =
                    WindowState.Normal;

                ownerWindow.Activate();

                Hide();

                return;
            }

            DragMove();
        }

        private void btnMiniStart_Click(object sender, RoutedEventArgs e)
        {
            ownerWindow.StartFromMini();
        }

        private void btnMiniStop_Click(object sender, RoutedEventArgs e)
        {
            ownerWindow.StopFromMini();
        }

        private void btnMiniReset_Click(object sender, RoutedEventArgs e)
        {
            ownerWindow.ResetFromMini();
        }

        private void btnMiniSetting_Click(object sender, RoutedEventArgs e)
        {
            ownerWindow.OpenSettingsFromMini();
        }

        private void btnMiniMain_Click(object sender, RoutedEventArgs e)
        {
            ownerWindow.ShowMainFromMini();
        }

        private void btnMiniExit_Click(object sender, RoutedEventArgs e)
        {
            ownerWindow.ExitFromMini();
        }
    }
}

