using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace TeleprompterApp
{
    public partial class MainWindow : Window
    {
        private TeleprompterWindow teleprompterWindow;

        public MainWindow()
        {
            InitializeComponent();
            LoadUserSettings();
        }

        private void BoldButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplyTextPropertyToSelection(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void BoldButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyTextPropertyToSelection(TextElement.FontWeightProperty, FontWeights.Normal);
        }

        private void ItalicButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplyTextPropertyToSelection(TextElement.FontStyleProperty, FontStyles.Italic);
        }

        private void ItalicButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyTextPropertyToSelection(TextElement.FontStyleProperty, FontStyles.Normal);
        }

        private void UnderlineButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplyTextDecorationToSelection(TextDecorations.Underline);
        }

        private void UnderlineButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RemoveTextDecorationFromSelection(TextDecorations.Underline);
        }

        private void TextColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScriptTextBox.Selection != null)
            {
                string colorName = ((ComboBoxItem)TextColorComboBox.SelectedItem).Content.ToString();
                BrushConverter converter = new BrushConverter();
                Brush colorBrush = (Brush)converter.ConvertFromString(colorName);
                ApplyTextPropertyToSelection(TextElement.ForegroundProperty, colorBrush);
            }
        }

        private void ScriptTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = ScriptTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            BoldButton.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));

            temp = ScriptTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicButton.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));

            temp = ScriptTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineButton.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (teleprompterWindow != null)
            {
                teleprompterWindow.ScrollingSpeed = SpeedSlider.Value;
            }
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ApplyFontSizeToRichTextBox();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (teleprompterWindow == null || !teleprompterWindow.IsVisible)
            {
                teleprompterWindow = new TeleprompterWindow();
            }

            TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Rtf);
            stream.Position = 0;

            teleprompterWindow.LoadScript(stream);
            teleprompterWindow.ScrollingSpeed = SpeedSlider.Value;
            teleprompterWindow.SetFontSize(FontSizeSlider.Value);
            teleprompterWindow.Show();
            teleprompterWindow.StartScrolling();
        }

        private void ApplyTextPropertyToSelection(DependencyProperty property, object value)
        {
            if (ScriptTextBox.Selection != null)
            {
                ScriptTextBox.Selection.ApplyPropertyValue(property, value);
            }
            ScriptTextBox.Focus();
        }

        private void ApplyFontSizeToRichTextBox()
        {
            TextRange range = new TextRange(ScriptTextBox.Document.ContentStart, ScriptTextBox.Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.FontSizeProperty, FontSizeSlider.Value);
        }

        private void ApplyTextDecorationToSelection(TextDecorationCollection decoration)
        {
            if (ScriptTextBox.Selection != null)
            {
                ScriptTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, decoration);
            }
            ScriptTextBox.Focus();
        }

        private void RemoveTextDecorationFromSelection(TextDecorationCollection decoration)
        {
            if (ScriptTextBox.Selection != null)
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
            ScriptTextBox.Focus();
        }

        private void LoadUserSettings()
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
            FontSizeSlider.Value = Properties.Settings.Default.FontSize > 0 ? Properties.Settings.Default.FontSize : 48;
            SpeedSlider.Value = Properties.Settings.Default.ScrollingSpeed > 0 ? Properties.Settings.Default.ScrollingSpeed : 2;
        }

        private void SaveUserSettings()
        {
            Properties.Settings.Default.MainWindowWidth = this.Width;
            Properties.Settings.Default.MainWindowHeight = this.Height;
            Properties.Settings.Default.MainWindowLeft = this.Left;
            Properties.Settings.Default.MainWindowTop = this.Top;
            Properties.Settings.Default.FontSize = FontSizeSlider.Value;
            Properties.Settings.Default.ScrollingSpeed = SpeedSlider.Value;
            Properties.Settings.Default.Save();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveUserSettings();
            base.OnClosing(e);
        }
    } // Closing brace for the MainWindow class

} // **Added this closing brace for the namespace**
