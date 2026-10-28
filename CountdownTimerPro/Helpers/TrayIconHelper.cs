using System;
using System.Drawing;
using System.Windows.Forms;

namespace CountdownTimerPro.Helpers
{
    public class TrayIconHelper : IDisposable
    {
        private NotifyIcon notifyIcon;

        public event Action StartClicked;
        public event Action StopClicked;
        public event Action ResetClicked;
        public event Action DoubleClicked;
        public event Action ExitClicked;

        public TrayIconHelper()
        {
            notifyIcon = new NotifyIcon();

            string iconPath =System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Assets","logocd.ico");

            notifyIcon.Icon =new Icon(iconPath);

            notifyIcon.Text = "Countdown Timer Pro";

            notifyIcon.Visible = true;

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            CreateMenu();
        }

        private void CreateMenu()
        {
            ContextMenuStrip menu =
                new ContextMenuStrip();

            ToolStripMenuItem start =
                new ToolStripMenuItem("Start");

            ToolStripMenuItem stop =
                new ToolStripMenuItem("Stop");

            ToolStripMenuItem reset =
                new ToolStripMenuItem("Reset");

            ToolStripSeparator sep =
                new ToolStripSeparator();

            ToolStripMenuItem exit =
                new ToolStripMenuItem("Exit");

            start.Click += delegate
            {
                if (StartClicked != null)
                    StartClicked();
            };

            stop.Click += delegate
            {
                if (StopClicked != null)
                    StopClicked();
            };

            reset.Click += delegate
            {
                if (ResetClicked != null)
                    ResetClicked();
            };

            exit.Click += delegate
            {
                if (ExitClicked != null)
                    ExitClicked();
            };

            menu.Items.Add(start);
            menu.Items.Add(stop);
            menu.Items.Add(reset);
            menu.Items.Add(sep);
            menu.Items.Add(exit);

            notifyIcon.ContextMenuStrip = menu;
        }

        private void NotifyIcon_DoubleClick(
            object sender,
            EventArgs e)
        {
            if (DoubleClicked != null)
                DoubleClicked();
        }

        public void SetText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return;

            if (text.Length > 63)
                text = text.Substring(0, 63);

            notifyIcon.Text = text;
        }

        public void ShowBalloon(
            string title,
            string message)
        {
            notifyIcon.BalloonTipTitle = title;

            notifyIcon.BalloonTipText = message;

            notifyIcon.BalloonTipIcon =
                ToolTipIcon.Info;

            notifyIcon.ShowBalloonTip(3000);
        }

        public void Dispose()
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;

                notifyIcon.Dispose();

                notifyIcon = null;
            }
        }
    }
}