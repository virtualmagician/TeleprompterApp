using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel; // Needed for CancelEventArgs

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
            Loaded += TeleprompterWindow_Loaded;
        }

        private void TeleprompterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadUserSettings();
                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(20)
                };
                timer.Tick += Timer_Tick;
                this.KeyDown += Window_KeyDown;
                this.Activated += TeleprompterWindow_Activated;
                this.Deactivated += TeleprompterWindow_Deactivated;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing TeleprompterWindow: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadScript(Stream scriptStream)
        {
            try
            {
                TextRange range = new TextRange(ScriptDocument.ContentStart, ScriptDocument.ContentEnd);
                range.Load(scriptStream, DataFormats.Rtf);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading script: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StartScrolling()
        {
            try
            {
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting scrolling: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ResetScrolling()
        {
            try
            {
                timer.Stop();
                Scroller.ScrollToVerticalOffset(0);
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting scrolling: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                try
                {
                    // Only scroll if not at the end
                    if (Scroller.VerticalOffset < Scroller.ScrollableHeight)
                    {
                        Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + ScrollingSpeed);
                    }
                    else
                    {
                        timer.Stop(); // Stop scrolling at the end
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during scrolling: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error handling key press: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUserSettings()
        {
            try
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

                // Initialize default scrolling speed if not set
                if (Properties.Settings.Default.ScrollingSpeed > 0)
                {
                    ScrollingSpeed = Properties.Settings.Default.ScrollingSpeed;
                }
                else
                {
                    ScrollingSpeed = 2.0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading TeleprompterWindow settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveUserSettings()
        {
            try
            {
                Properties.Settings.Default.TeleprompterWindowWidth = this.Width;
                Properties.Settings.Default.TeleprompterWindowHeight = this.Height;
                Properties.Settings.Default.TeleprompterWindowLeft = this.Left;
                Properties.Settings.Default.TeleprompterWindowTop = this.Top;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving TeleprompterWindow settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                SaveUserSettings();
                timer?.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during closing TeleprompterWindow: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            base.OnClosing(e);
        }

        public void SetFontSize(double fontSize)
        {
            try
            {
                ScriptDocument.FontSize = fontSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting font size: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TeleprompterWindow_Activated(object sender, EventArgs e)
        {
            // Optional: Handle actions when the TeleprompterWindow becomes active
        }

        private void TeleprompterWindow_Deactivated(object sender, EventArgs e)
        {
            // Optional: Handle actions when the TeleprompterWindow loses focus
        }
    }
}
