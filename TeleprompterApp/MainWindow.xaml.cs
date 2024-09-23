using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using System.ComponentModel; // Needed for CancelEventArgs

namespace TeleprompterApp
{
    public partial class MainWindow : Window
    {
        private TeleprompterWindow teleprompterWindow;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Instantiate and show the TeleprompterWindow
                teleprompterWindow = new TeleprompterWindow();
                teleprompterWindow.Show();

                // Load user settings for MainWindow
                LoadUserSettings();

                // Initialize the TeleprompterWindow with current settings
                InitializeTeleprompter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during MainWindow Loaded: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeTeleprompter()
        {
            try
            {
                // Save the current script to a MemoryStream
                TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
                MemoryStream stream = new MemoryStream();
                range.Save(stream, DataFormats.Rtf);
                stream.Position = 0;

                // Load the script into the TeleprompterWindow
                teleprompterWindow.LoadScript(stream);
                teleprompterWindow.ScrollingSpeed = SpeedSlider.Value;
                teleprompterWindow.SetFontSize(FontSizeSlider.Value);
                teleprompterWindow.StartScrolling();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing Teleprompter: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (teleprompterWindow != null)
                {
                    // Save the current script to a MemoryStream
                    TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
                    MemoryStream stream = new MemoryStream();
                    range.Save(stream, DataFormats.Rtf);
                    stream.Position = 0;

                    // Reload the script in the TeleprompterWindow
                    teleprompterWindow.LoadScript(stream);

                    // Update scrolling speed and font size
                    teleprompterWindow.ScrollingSpeed = SpeedSlider.Value;
                    teleprompterWindow.SetFontSize(FontSizeSlider.Value);

                    // Restart scrolling from the beginning
                    teleprompterWindow.ResetScrolling();

                    // Activate the TeleprompterWindow
                    teleprompterWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing Teleprompter: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoldButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyTextPropertyToSelection(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying bold: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoldButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyTextPropertyToSelection(TextElement.FontWeightProperty, FontWeights.Normal);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing bold: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ItalicButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyTextPropertyToSelection(TextElement.FontStyleProperty, FontStyles.Italic);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying italic: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ItalicButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyTextPropertyToSelection(TextElement.FontStyleProperty, FontStyles.Normal);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing italic: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnderlineButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyTextDecorationToSelection(TextDecorations.Underline);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying underline: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnderlineButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RemoveTextDecorationFromSelection(TextDecorations.Underline);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing underline: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ScriptTextBox != null && ScriptTextBox.Selection != null)
                {
                    string colorName = ((ComboBoxItem)TextColorComboBox.SelectedItem).Content.ToString();
                    BrushConverter converter = new BrushConverter();
                    Brush colorBrush = (Brush)converter.ConvertFromString(colorName);
                    ApplyTextPropertyToSelection(TextElement.ForegroundProperty, colorBrush);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing text color: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ScriptTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScriptTextBox.Selection != null)
                {
                    object temp = ScriptTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
                    BoldButton.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));

                    temp = ScriptTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
                    ItalicButton.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));

                    temp = ScriptTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                    UnderlineButton.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating formatting buttons: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (teleprompterWindow != null)
                {
                    teleprompterWindow.ScrollingSpeed = SpeedSlider.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating scrolling speed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                ApplyFontSizeToRichTextBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating font size: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Rich Text Files (*.rtf)|*.rtf" };
                if (openFileDialog.ShowDialog() == true)
                {
                    TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
                    using (FileStream fStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        range.Load(fStream, DataFormats.Rtf);
                    }
                    ApplyFontSizeToRichTextBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading script: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "Rich Text Files (*.rtf)|*.rtf" };
                if (saveFileDialog.ShowDialog() == true)
                {
                    TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
                    using (FileStream fStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        range.Save(fStream, DataFormats.Rtf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving script: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyTextPropertyToSelection(DependencyProperty property, object value)
        {
            if (ScriptTextBox != null && ScriptTextBox.Selection != null)
            {
                ScriptTextBox.Selection.ApplyPropertyValue(property, value);
            }
            ScriptTextBox?.Focus();
        }

        private void ApplyTextDecorationToSelection(TextDecorationCollection decoration)
        {
            if (ScriptTextBox != null && ScriptTextBox.Selection != null)
            {
                ScriptTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, decoration);
            }
            ScriptTextBox?.Focus();
        }

        private void RemoveTextDecorationFromSelection(TextDecorationCollection decoration)
        {
            if (ScriptTextBox != null && ScriptTextBox.Selection != null)
            {
                TextDecorationCollection currentDecorations = (TextDecorationCollection)ScriptTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                if (currentDecorations != null)
                {
                    TextDecorationCollection newDecorations = currentDecorations.Clone();
                    foreach (TextDecoration td in decoration)
                    {
                        newDecorations.Remove(td);
                    }
                    ScriptTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, newDecorations);
                }
            }
            ScriptTextBox?.Focus();
        }

        private void ApplyFontSizeToRichTextBox()
        {
            if (ScriptTextBox != null)
            {
                TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
                range.ApplyPropertyValue(TextElement.FontSizeProperty, FontSizeSlider.Value);
            }
        }

        private void LoadUserSettings()
        {
            try
            {
                if (Properties.Settings.Default.MainWindowWidth > 0)
                {
                    this.Width = Properties.Settings.Default.MainWindowWidth;
                    this.Height = Properties.Settings.Default.MainWindowHeight;
                    this.Left = Properties.Settings.Default.MainWindowLeft;
                    this.Top = Properties.Settings.Default.MainWindowTop;
                }
                else
                {
                    this.Width = 800;
                    this.Height = 600;
                    this.Left = 100;
                    this.Top = 100;
                }

                if (FontSizeSlider != null)
                {
                    FontSizeSlider.Value = Properties.Settings.Default.FontSize > 0 ? Properties.Settings.Default.FontSize : 48;
                }

                if (SpeedSlider != null)
                {
                    SpeedSlider.Value = Properties.Settings.Default.ScrollingSpeed > 0 ? Properties.Settings.Default.ScrollingSpeed : 2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveUserSettings()
        {
            try
            {
                Properties.Settings.Default.MainWindowWidth = this.Width;
                Properties.Settings.Default.MainWindowHeight = this.Height;
                Properties.Settings.Default.MainWindowLeft = this.Left;
                Properties.Settings.Default.MainWindowTop = this.Top;

                if (FontSizeSlider != null)
                {
                    Properties.Settings.Default.FontSize = FontSizeSlider.Value;
                }

                if (SpeedSlider != null)
                {
                    Properties.Settings.Default.ScrollingSpeed = SpeedSlider.Value;
                }

                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving user settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                SaveUserSettings();
                teleprompterWindow?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during closing MainWindow: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            base.OnClosing(e);
        }
    }
}
