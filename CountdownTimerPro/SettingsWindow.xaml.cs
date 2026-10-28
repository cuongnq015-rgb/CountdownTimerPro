using CountdownTimerPro.Models;
using CountdownTimerPro.Properties;
using System;
using System.Runtime;
using System.Windows;

namespace CountdownTimerPro
{
    public partial class SettingsWindow : Window
    {
        public AppSettings Settings { get; private set; }

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();

            Settings = settings;

            LoadSettings();
        }

        private void LoadSettings()
        {
            txtMinute.Text = Settings.Minutes.ToString();

            txtSecond.Text = Settings.Seconds.ToString();

            chkTopMost.IsChecked = Settings.TopMost;

            chkBeep.IsChecked = Settings.EnableBeep;

            chkOvertime.IsChecked = Settings.AllowOvertime;

            chkBlink.IsChecked = Settings.EnableBlink;

            chkFullScreen.IsChecked = Settings.FullScreen;
        }

        private bool ValidateInput(out int minute, out int second)
        {
            minute = 0;
            second = 0;

            if (!int.TryParse(txtMinute.Text, out minute))
            {
                MessageBox.Show(
                    "Phút không hợp lệ.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtMinute.Focus();
                return false;
            }

            if (!int.TryParse(txtSecond.Text, out second))
            {
                MessageBox.Show(
                    "Giây không hợp lệ.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtSecond.Focus();
                return false;
            }

            if (minute < 0)
            {
                MessageBox.Show(
                    "Phút phải lớn hơn hoặc bằng 0.");

                txtMinute.Focus();
                return false;
            }

            if (second < 0 || second > 59)
            {
                MessageBox.Show(
                    "Giây phải từ 0 đến 59.");

                txtSecond.Focus();
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int minute;
            int second;

            if (!ValidateInput(out minute, out second))
                return;

            Settings.Minutes = minute;

            Settings.Seconds = second;

            Settings.TopMost =
                chkTopMost.IsChecked == true;
            Settings.AllowOvertime =
                chkOvertime.IsChecked == true;

            Settings.EnableBeep =
                chkBeep.IsChecked == true;

            Settings.EnableBlink =
                chkBlink.IsChecked == true;

            Settings.FullScreen =
                chkFullScreen.IsChecked == true;

            DialogResult = true;

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}