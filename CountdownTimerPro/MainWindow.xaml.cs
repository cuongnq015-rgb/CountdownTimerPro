using CountdownTimerPro.Helpers;
using CountdownTimerPro.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CountdownTimerPro
{
    public partial class MainWindow : Window
    {
        #region ===== Fields =====

        // Countdown
        private CountdownManager countdown;

        // Stopwatch
        private StopwatchManager stopwatch;

        // Mini Window
        private MiniWindow miniWindow;

        // Tray Icon
        private TrayIconHelper tray;

        // Settings
        private AppSettings settings;

        // Trạng thái
        private bool isLoaded = false;

        private bool isStopwatchMode = false;


        // Blink cảnh báo
        private DispatcherTimer blinkTimer;

        private bool blinkState = false;

        // File cấu hình
        private readonly string settingFile =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "settings.json");

        #endregion

        #region ===== Constructor =====

        public MainWindow()
        {
            InitializeComponent();

            //--------------------------------
            // Load Settings
            //--------------------------------

            LoadSettings();

            //--------------------------------
            // Init Manager
            //--------------------------------

            InitManagers();

            //--------------------------------
            // Init Tray
            //--------------------------------

            InitTray();

            //--------------------------------
            // Init Mini Window
            //--------------------------------

            InitMiniWindow();

            //--------------------------------
            // Register Events
            //--------------------------------

            RegisterEvents();

            //--------------------------------
            // Apply Settings
            //--------------------------------

            ApplySettings();

            //--------------------------------
            // Refresh lần đầu
            //--------------------------------

            RefreshUI();

            //--------------------------------
            // Đánh dấu đã load xong
            //--------------------------------

            isLoaded = true;
        }

        #endregion
        #region ===== Initialize =====

        private void LoadSettings()
        {
            settings = JsonHelper.Load<AppSettings>(settingFile);

            if (settings == null)
            {
                settings = new AppSettings();

                JsonHelper.Save(settingFile, settings);
            }

            // Khôi phục vị trí cửa sổ
            if (settings.WindowWidth > 0)
                Width = settings.WindowWidth;

            if (settings.WindowHeight > 0)
                Height = settings.WindowHeight;

            Left = settings.WindowLeft;
            Top = settings.WindowTop;

            Topmost = settings.TopMost;
        }

        private void InitManagers()
        {
            countdown = new CountdownManager(
                settings.Minutes * 60 +
                settings.Seconds);

            countdown.AllowOvertime =
                settings.AllowOvertime;

            stopwatch = new StopwatchManager();

            // Timer nhấp nháy (chuẩn bị dùng ở HandleWarning)
            blinkTimer = new DispatcherTimer();

            blinkTimer.Interval =
                TimeSpan.FromMilliseconds(500);

            blinkTimer.Tick += BlinkTimer_Tick;
        }

        private void InitMiniWindow()
        {
            miniWindow = new MiniWindow(this);

            miniWindow.SetTopMost(settings.TopMost);

            miniWindow.SetMode(false);

            if (settings.MiniMode)
            {
                miniWindow.Show();
            }
        }

        private void InitTray()
        {
            tray = new TrayIconHelper();

            tray.StartClicked += delegate
            {
                Dispatcher.Invoke(delegate
                {
                    btnStart_Click(null, null);
                });
            };

            tray.StopClicked += delegate
            {
                Dispatcher.Invoke(delegate
                {
                    btnStop_Click(null, null);
                });
            };

            tray.ResetClicked += delegate
            {
                Dispatcher.Invoke(delegate
                {
                    btnReset_Click(null, null);
                });
            };

            tray.DoubleClicked += delegate
            {
                Dispatcher.Invoke(delegate
                {
                    Show();

                    WindowState =
                        WindowState.Normal;

                    Activate();
                });
            };

            tray.ExitClicked += delegate
            {
                Dispatcher.Invoke(delegate
                {
                    Close();
                });
            };
        }

        private void RegisterEvents()
        {
            countdown.Tick += Countdown_Tick;

            countdown.Finished += Countdown_Finished;

            stopwatch.Tick += Stopwatch_Tick;

            Closing += MainWindow_Closing;
        }

        #endregion
        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            blinkState = !blinkState;

            if (blinkState)
            {
                txtTime.Visibility = Visibility.Hidden;
            }
            else
            {
                txtTime.Visibility = Visibility.Visible;
            }
        }
        #region ===== Refresh UI =====

        private void ApplySettings()
        {
            // Chỉ áp dụng Setting
            countdown.AllowOvertime =
                settings.AllowOvertime;

            Topmost = settings.TopMost;

            if (miniWindow != null)
            {
                miniWindow.SetTopMost(settings.TopMost);
            }
        }

        private void RefreshUI()
        {
            UpdateTime();

            UpdateProgress();

            UpdateColor();

            UpdateStatus();

            UpdateMiniWindow();

            UpdateTray();
        }

        private void UpdateTime()
        {
            if (isStopwatchMode)
            {
                TimeSpan ts =
                    TimeSpan.FromSeconds(
                        stopwatch.ElapsedSeconds);

                txtTime.Text =
                    ts.ToString(@"hh\:mm\:ss");
            }
            else
            {
                int remain =
                    countdown.RemainingSeconds;

                TimeSpan ts =
                    TimeSpan.FromSeconds(
                        Math.Abs(remain));

                if (remain < 0)
                {
                    txtTime.Text =
                        "-" +
                        ts.ToString(@"mm\:ss");
                }
                else
                {
                    txtTime.Text =
                        ts.ToString(@"mm\:ss");
                }
            }
        }

        private void UpdateProgress()
        {
            if (isStopwatchMode)
            {
                progress.Value = 100;

                txtPercent.Text = "";

                progress.Visibility =
                    Visibility.Hidden;

                return;
            }

            progress.Visibility =
                Visibility.Visible;

            if (countdown.TotalSeconds <= 0)
            {
                progress.Value = 0;

                txtPercent.Text = "0%";

                return;
            }

            double percent =
                Math.Max(
                    countdown.RemainingSeconds,
                    0)
                * 100.0
                / countdown.TotalSeconds;

            progress.Value = percent;

            txtPercent.Text =
                ((int)percent).ToString() + "%";
        }

        private void UpdateColor()
        {
            txtTime.Visibility =
                Visibility.Visible;

            txtTime.FontSize = 92;

            if (isStopwatchMode)
            {
                txtTime.Foreground =
                    Brushes.LimeGreen;

                txtStatus.Foreground =
                    Brushes.LimeGreen;

                return;
            }

            int remain =
                countdown.RemainingSeconds;

            if (remain < 0)
            {
                txtTime.Foreground =
                    Brushes.MediumVioletRed;

                txtStatus.Foreground =
                    Brushes.MediumVioletRed;

                txtTime.FontSize = 100;

                return;
            }

            if (remain <= 10)
            {
                txtTime.Foreground =
                    Brushes.Red;

                txtStatus.Foreground =
                    Brushes.Red;

                txtTime.FontSize = 100;

                return;
            }

            if (remain <= 30)
            {
                txtTime.Foreground =
                    Brushes.Orange;

                txtStatus.Foreground =
                    Brushes.Orange;

                return;
            }

            txtTime.Foreground =
                Brushes.LimeGreen;

            txtStatus.Foreground =
                Brushes.LimeGreen;
        }

        private void UpdateStatus()
        {
            if (isStopwatchMode)
            {
                txtStatus.Text =
                    stopwatch.IsRunning
                    ? "RUNNING"
                    : "READY";

                return;
            }

            if (countdown.RemainingSeconds < 0)
            {
                txtStatus.Text =
                    "OVERTIME";

                return;
            }

            if (countdown.RemainingSeconds == 0 &&
                !countdown.AllowOvertime)
            {
                txtStatus.Text =
                    "FINISHED";

                return;
            }

            txtStatus.Text =
                countdown.IsRunning
                ? "RUNNING"
                : "READY";
        }

        private void UpdateMiniWindow()
        {
            if (miniWindow == null)
                return;

            miniWindow.SetMode(
                isStopwatchMode);

            if (isStopwatchMode)
            {
                miniWindow.UpdateStopwatch(
                    stopwatch.ElapsedSeconds,
                    stopwatch.IsRunning);
            }
            else
            {
                miniWindow.UpdateCountdown(
                    countdown.RemainingSeconds,
                    countdown.TotalSeconds,
                    countdown.IsRunning,
                    countdown.AllowOvertime);
            }
        }

        private void UpdateTray()
        {
            if (tray == null)
                return;

            tray.SetText(txtTime.Text);
        }

        #endregion
        #region ===== Countdown Event =====

        private void Countdown_Tick(int remain)
        {
            Dispatcher.Invoke(delegate
            {
                // Cập nhật giao diện
                RefreshUI();

                // Xử lý cảnh báo
                HandleWarning(remain);
            });
        }

        private void Countdown_Finished()
        {
            Dispatcher.Invoke(delegate
            {
                // Nếu cho phép Overtime thì không kết thúc
                if (countdown.AllowOvertime)
                {
                    return;
                }

                // Dừng nhấp nháy
                blinkTimer.Stop();

                txtTime.Visibility = Visibility.Visible;

                RefreshUI();
                

                // Âm thanh
                SystemSounds.Exclamation.Play();

                // Balloon
                if (tray != null)
                {
                    tray.ShowBalloon(
                        "Countdown Timer Pro",
                        "Đã hết thời gian.");
                }

                // Mini Window
                if (miniWindow != null)
                {
                    miniWindow.SetFinished();
                }

                MessageBox.Show(
                    "Đã hết thời gian.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            });
        }

        private void HandleWarning(int remain)
        {
            //------------------------------------------------
            // Stopwatch
            //------------------------------------------------

            if (isStopwatchMode)
            {
                blinkTimer.Stop();

                txtTime.Visibility = Visibility.Visible;

                return;
            }

            //------------------------------------------------
            // Overtime
            //------------------------------------------------

            if (remain < 0)
            {
                blinkTimer.Stop();

                txtTime.Visibility = Visibility.Visible;

                return;
            }

            //------------------------------------------------
            // 0 giây
            //------------------------------------------------

            if (remain == 0)
            {
                if (settings.EnableBlink)
                {
                    if (!blinkTimer.IsEnabled)
                    {
                        blinkState = false;

                        blinkTimer.Start();
                    }
                }

                return;
            }

            //------------------------------------------------
            // 1 -> 10 giây
            //------------------------------------------------

            if (remain <= 10)
            {
                if (settings.EnableBeep)
                {
                    SystemSounds.Beep.Play();
                }

                if (settings.EnableBlink)
                {
                    if (!blinkTimer.IsEnabled)
                    {
                        blinkState = false;

                        blinkTimer.Start();
                    }
                }

                return;
            }

            //------------------------------------------------
            // >10 giây
            //------------------------------------------------

            blinkTimer.Stop();

            txtTime.Visibility =
                Visibility.Visible;
        }

        #endregion
        #region ===== Stopwatch & Button Events =====

        private void Stopwatch_Tick(int second)
        {
            Dispatcher.Invoke(delegate
            {
                RefreshUI();
            });
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (isStopwatchMode)
            {
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                }
            }
            else
            {
                if (!countdown.IsRunning)
                {
                    countdown.Start();
                }
            }

            RefreshUI();
            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (isStopwatchMode)
            {
                stopwatch.Stop();
            }
            else
            {
                countdown.Stop();
            }

            blinkTimer.Stop();

            txtTime.Visibility = Visibility.Visible;

            RefreshUI();
            
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            blinkTimer.Stop();

            txtTime.Visibility = Visibility.Visible;

            if (isStopwatchMode)
            {
                stopwatch.Reset();

                txtTime.Text = "00:00:00";
            }
            else
            {
                countdown.Reset();

                txtTime.Text =
                    TimeSpan.FromSeconds(
                        countdown.RemainingSeconds)
                    .ToString(@"mm\:ss");
            }

            RefreshUI();
            
        }

        private void cmbMode_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!isLoaded)
                return;

            blinkTimer.Stop();

            txtTime.Visibility = Visibility.Visible;

            bool newMode =
                cmbMode.SelectedIndex == 1;

            if (newMode == isStopwatchMode)
                return;

            isStopwatchMode = newMode;

            if (isStopwatchMode)
            {
                //--------------------------------
                // Chuyển sang Stopwatch
                //--------------------------------

                countdown.Stop();

                stopwatch.Reset();

                txtModeTitle.Text =
                    "⏱ STOPWATCH";

                txtTime.Text =
                    "00:00:00";
            }
            else
            {
                //--------------------------------
                // Chuyển sang Countdown
                //--------------------------------

                stopwatch.Stop();

                countdown.Reset();

                txtModeTitle.Text =
                    "⏳ COUNTDOWN";
            }

            RefreshUI();
        }

        #endregion
        #region ===== Window Events =====

        private void Window_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMini_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (miniWindow == null)
                return;

            settings.MiniMode = true;

            JsonHelper.Save(settingFile, settings);

            miniWindow.SetMode(isStopwatchMode);

            if (isStopwatchMode)
            {
                miniWindow.UpdateStopwatch(
                    stopwatch.ElapsedSeconds,
                    stopwatch.IsRunning);
            }
            else
            {
                miniWindow.UpdateCountdown(
                    countdown.RemainingSeconds,
                    countdown.TotalSeconds,
                    countdown.IsRunning,
                    countdown.AllowOvertime);
            }

            miniWindow.Show();

            Hide();
        }

        private void btnClose_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region ===== Settings =====

        private void btnSetting_Click(
            object sender,
            RoutedEventArgs e)
        {
            bool wasRunningCountdown =
                countdown.IsRunning;

            bool wasRunningStopwatch =
                stopwatch.IsRunning;

            countdown.Stop();

            stopwatch.Stop();

            SettingsWindow win =
                new SettingsWindow(settings);

            win.Owner = this;

            bool? result =
                win.ShowDialog();

            if (result != true)
            {
                // Khôi phục trạng thái chạy nếu Cancel
                if (wasRunningCountdown)
                    countdown.Start();

                if (wasRunningStopwatch)
                    stopwatch.Start();

                RefreshUI();

                return;
            }

            settings = win.Settings;

            JsonHelper.Save(
                settingFile,
                settings);

            //----------------------------------
            // Áp dụng thời gian mới
            //----------------------------------

            countdown.SetTime(
                settings.Minutes,
                settings.Seconds);

            countdown.AllowOvertime =
                settings.AllowOvertime;

            //----------------------------------
            // TopMost
            //----------------------------------

            Topmost =
                settings.TopMost;

            if (miniWindow != null)
            {
                miniWindow.SetTopMost(
                    settings.TopMost);
            }

            //----------------------------------
            // Reset theo mode hiện tại
            //----------------------------------

            if (isStopwatchMode)
            {
                stopwatch.Reset();
            }
            else
            {
                countdown.Reset();
            }

            RefreshUI();
        }

        #endregion
        #region ===== Closing & Save =====

        private void MainWindow_Closing(
            object sender,
            CancelEventArgs e)
        {
            SaveSettings();

            if (blinkTimer != null)
            {
                blinkTimer.Stop();
            }

            if (countdown != null)
            {
                countdown.Stop();
            }

            if (stopwatch != null)
            {
                stopwatch.Stop();
            }

            if (miniWindow != null)
            {
                try
                {
                    miniWindow.Close();
                }
                catch
                {
                }
            }

            if (tray != null)
            {
                tray.Dispose();
            }
        }

        private void SaveSettings()
        {
            if (settings == null)
                return;

            settings.WindowLeft = Left;

            settings.WindowTop = Top;

            settings.WindowWidth = Width;

            settings.WindowHeight = Height;

            settings.TopMost = Topmost;

            settings.MiniMode =
                miniWindow != null &&
                miniWindow.IsVisible;

            JsonHelper.Save(
                settingFile,
                settings);
        }

        private void DisposeManagers()
        {
            if (blinkTimer != null)
            {
                blinkTimer.Stop();

                blinkTimer.Tick -= BlinkTimer_Tick;

                blinkTimer = null;
            }

            if (tray != null)
            {
                tray.Dispose();

                tray = null;
            }

            if (miniWindow != null)
            {
                try
                {
                    miniWindow.Close();
                }
                catch
                {
                }

                miniWindow = null;
            }
        }

        public void StartFromMini()
        {
            btnStart_Click(null, null);
        }

        public void StopFromMini()
        {
            btnStop_Click(null, null);
        }

        public void ResetFromMini()
        {
            btnReset_Click(null, null);
        }

        public void OpenSettingsFromMini()
        {
            btnSetting_Click(null, null);
        }

        public void ShowMainFromMini()
        {
            Show();

            WindowState = WindowState.Normal;

            Activate();

            if (miniWindow != null)
                miniWindow.Hide();
        }

        public void ExitFromMini()
        {
            Close();
        }
    }
    #endregion
}