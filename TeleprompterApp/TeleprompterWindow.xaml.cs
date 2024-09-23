using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace TeleprompterApp
{
    public partial class TeleprompterWindow : Window
    {
        private DispatcherTimer timer;
        public double ScrollingSpeed { get; set; }
        private bool isPaused = false;

        public TeleprompterWindow()
        {
            InitializeComponent();
            LoadUserSettings();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            this.KeyDown += Window_KeyDown;
        }

        public void LoadScript(Stream scriptStream)
        {
            TextRange range = new TextRange(ScriptDocument.ContentStart, ScriptDocument.ContentEnd);
            range.Load(scriptStream, DataFormats.Rtf);
        }

        public void StartScrolling()
        {
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + ScrollingSpeed);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                isPaused = !isPaused;
            }
            else if (e.Key == Key.Up)
            {
                ScrollingSpeed += 0.1;
            }
            else if (e.Key == Key.Down)
            {
                ScrollingSpeed = Math.Max(0, ScrollingSpeed - 0.1);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void LoadUserSettings()
        {
            if (Properties.Settings.Default.TeleprompterWindowWidth > 0)
            {
                this.Width = Properties.Settings.Default.TeleprompterWindowWidth;
                this.Height = Properties.Settings.Default.TeleprompterWindowHeight;
                this.Left = Properties.Settings.Default.TeleprompterWindowLeft;
                this.Top = Properties.Settings.Default.TeleprompterWindowTop;
            }
            else
            {
                this.Width = 800;
                this.Height = 600;
                this.Left = 100;
                this.Top = 100;
            }
        }

        private void SaveUserSettings()
        {
            Properties.Settings.Default.TeleprompterWindowWidth = this.Width;
            Properties.Settings.Default.TeleprompterWindowHeight = this.Height;
            Properties.Settings.Default.TeleprompterWindowLeft = this.Left;
            Properties.Settings.Default.TeleprompterWindowTop = this.Top;
            Properties.Settings.Default.Save();
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveUserSettings();
            base.OnClosed(e);
        }

        public void SetFontSize(double fontSize)
        {
            ScriptDocument.FontSize = fontSize;
        }
    }
}
