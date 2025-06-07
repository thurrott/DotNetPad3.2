using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DotNetPad32
{
    public partial class MainWindow : Window
    {
        // Create a Document, AppState, and AppSettings
        public Document d { get; } = new();
        AppState appState = new();
        AppSettings appSettings = new();

        // Create a timer for Auto save
        private static System.Timers.Timer AutoSaveTimer = new();

        public MainWindow()
        {
            // Fix menus
            SetDropDownMenuToBeRightAligned();

            InitializeComponent();

            // Set DataContext for bindings
            DataContext = this;

            Title = Path.GetFileNameWithoutExtension(d.FileName);

            // Get user settings and recent documents
            appSettings.LoadSettings(AppWindow, appState, AutoSaveTimer);

            // Set up Auto Save timer
            SetUpTimer();

            // ------- FIX THIS
            // appState.FontSizeInSettings = TextBox1.FontSize;

            // Build out the Recents sub-menu
            Recents recents = new(RecentMenu);

            // Get app information
            AppInfo();

            // Select the text box so the user can start typing
            TextBox1.Focus();
        }

        private void SetUpTimer()
        {
            AutoSaveTimer = new System.Timers.Timer(30000)   // 30 seconds
            {
                AutoReset = true
            };

            AutoSaveMenu.IsChecked = appState.AutoSave;
            AutoSaveToggleButton.IsChecked = appState.AutoSave;
            if (appState.AutoSave)
            {
                AutoSaveTimer.Enabled = true;
                AutoSaveText.Text = "Auto save: On";
                AutoSaveToggleButton.Content = "On";
                AutoSaveTimer.Start();
            }
            else
            {
                AutoSaveTimer.Enabled = true;
                AutoSaveText.Text = "Auto save: Off";
                AutoSaveToggleButton.Content = "Off";
                AutoSaveTimer.Stop();
            }
            AutoSaveTimer.Elapsed += OnTimerElapsed;
        }

        private static void SetDropDownMenuToBeRightAligned()
        {
            // Fix menu alignment on touch-screen PCs
            // Per https://www.answeroverflow.com/m/1134790256609206333

            var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            Action setAlignmentValue = () =>
            {
                if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
            };

            setAlignmentValue();

            SystemParameters.StaticPropertyChanged += (sender, e) =>
            {
                setAlignmentValue();
            };
        }

        private void AppInfo()
        {
            AppNameLabel.Content = appState.AppName;
            // AppNameLabel.Content = Assembly.GetExecutingAssembly().GetName().Name;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            VersionLabel.Content = "Version " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString()
                                   + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            CopyrightLabel.Content = "Copyright © 2025 Paul Thurrott";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable timer so a Save as window won't appear
            AutoSaveTimer.Enabled = false;

            // 2025 add: Disable other actions related to commands
            // Save, Save as, New, Open, etc.

            // Clear the Outer grid column definitions
            OuterGrid.ColumnDefinitions.Clear();

            // Select the correct item in the Font style combo box
            // Not clear why this is necessary
            if (TextBox1.FontStyle == FontStyles.Normal)
            {
                FontStyleComboBox.SelectedIndex = 0;
                FontExampleLabel.FontStyle = FontStyles.Normal;
                FontExampleLabel.FontWeight = FontWeights.Normal;
            }
            if (TextBox1.FontStyle == FontStyles.Italic)
            {
                FontStyleComboBox.SelectedIndex = 1;
                FontExampleLabel.FontStyle = FontStyles.Italic;
                FontExampleLabel.FontWeight = FontWeights.Normal;
            }
            if (TextBox1.FontWeight == FontWeights.Bold)
            {
                FontStyleComboBox.SelectedIndex = 2;
                FontExampleLabel.FontStyle = FontStyles.Normal;
                FontExampleLabel.FontWeight = FontWeights.Bold;
            }
            if (TextBox1.FontWeight == FontWeights.Bold && TextBox1.FontStyle == FontStyles.Italic)
            {
                FontStyleComboBox.SelectedIndex = 3;
                FontExampleLabel.FontStyle = FontStyles.Italic;
                FontExampleLabel.FontWeight = FontWeights.Bold;
            }

            // Hide the Main grid, and display the Back button and the Settings grid
            // Add: Re-enable the commands we will disable above too
            AppGrid.Visibility = Visibility.Hidden; 
            SettingsHeaderGrid.Visibility = Visibility.Visible;
            SettingsScrollViewer.Visibility = Visibility.Visible;
            SettingsGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the Outer grid column definitions on exit
            OuterGrid.ColumnDefinitions.Clear();

            appSettings.SaveSettings(AppWindow, appState, AutoSaveTimer);

            // Unexpand all the Expander controls on exit
            AppThemeExpander.IsExpanded = false;
            FontExpander.IsExpanded = false;

            // Re-enable the Auto save timer on exit
            AutoSaveTimer.Enabled = true;

            // Hide the Back button and the Settings grid on exit and display the Main grid
            SettingsHeaderGrid.Visibility = Visibility.Collapsed;
            SettingsScrollViewer.Visibility = Visibility.Hidden;
            SettingsGrid.Visibility = Visibility.Hidden;
            AppGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            AppGrid.Visibility = Visibility.Visible;

            TextBox1.Focus();
        }

        private void TextBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (d.TextHasChanged == false)
            {
                d.TextHasChanged = true;
            }
            DisplayCount();

            ChangePositionText();
        }

        private void TextBox1_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ChangePositionText();
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            ChangePositionText();
        }

        public void DisplayCount()
        {
            if (appState.WordCount)
            {
                int Count = System.Text.RegularExpressions.Regex.Matches(TextBox1.Text, @"[\S]+").Count;
                WordCountText.Text = Count.ToString() + " word";
                if (Count != 1)
                    WordCountText.Text += "s";
            }
            else
            {
                int Count = System.Text.RegularExpressions.Regex.Matches(TextBox1.Text, @".").Count;
                WordCountText.Text = Count.ToString() + " character";
                if (Count != 1)
                    WordCountText.Text += "s";
            }
        }

        private void ChangePositionText()
        {
            var lineColumn = (0, 0);
            lineColumn.Item1 = TextBox1.GetLineIndexFromCharacterIndex(TextBox1.CaretIndex);
            lineColumn.Item2 = TextBox1.CaretIndex - TextBox1.GetCharacterIndexFromLineIndex(lineColumn.Item1);
            PositionText.Text = "Ln " + (lineColumn.Item1 + 1).ToString() + ", Col " + (lineColumn.Item2 + 1).ToString();
        }

        private void NewCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (d.NeedsToBeSaved(TextBox1.Text))
            {
                d.NewDocument(AppWindow);
            }
        }

        private void OpenCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (d.NeedsToBeSaved(TextBox1.Text))
            {
                d.OpenDocument(TextBox1);
                Title = Path.GetFileNameWithoutExtension(d.FileName);
            }
        }

        private void NewWindowMenu_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.GetFileName(System.Environment.ProcessPath)!);
        }

        private void SaveCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (d.DocumentIsSaved)
            {
                d.SaveDocument(TextBox1.Text);
            }
            else
            {
                d.SaveDocumentAs(TextBox1.Text);
            }
        }

        private void SaveAsCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            d.SaveDocumentAs(TextBox1.Text);
            Title = Path.GetFileNameWithoutExtension(d.FileName);
        }

        private void AutoSaveMenu_Click(object sender, RoutedEventArgs e)
        {
            AutoSaveDialog asd = new(appState.AutoSave)
            {
                Owner = this
            };

            bool? result = asd.ShowDialog();

            if (result != null && (bool)result == true)
            {
                AutoSaveToggle();
            }

            e.Handled = true;
        }

        private void AutoSaveToggle()
        {
            // Toggle appState.AutoSave
            appState.AutoSave = !appState.AutoSave;
            var AutoSaveContent = appState.AutoSave ? "On" : "Off";

            // Then, update the UI
            AutoSaveToggleButton.IsChecked = appState.AutoSave;
            AutoSaveToggleButton.Content = AutoSaveContent;
            AutoSaveText.Text = "Auto save: " + AutoSaveContent;
            AutoSaveMenu.IsChecked = appState.AutoSave;

            // Actually save the document
            if (appState.AutoSave)
            {
                if (d.DocumentIsSaved)
                {
                    d.SaveDocument(TextBox1.Text);
                }
                else
                {
                    d.SaveDocumentAs(TextBox1.Text);
                }
            }

            // Toggle timer, save the settings
            AutoSaveTimer.Enabled = appState.AutoSave;
            appSettings.Save_AutoSaveSettings(appState);
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Stop the timer, otherwise this will keep firing and could keep display more SaveAs dialogs
            AutoSaveTimer.Stop();

            // Required for saving an existing document (not Save as)
            // Otherwise, Save condition triggers a crash
            this.Dispatcher.Invoke(() =>
            {
                if (d.TextHasChanged)
                {

                    if (d.DocumentIsSaved)
                    {
                        d.SaveDocument(TextBox1.Text);
                    }
                    else
                    {
                        if(d.SaveDocumentAs(TextBox1.Text) == false)
                        {
                            return;
                        }
                    }
                }
            });

            // Restart the timer when we're done
            AutoSaveTimer.Start();
        }

        private void PrintCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog
            {
                PageRangeSelection = PageRangeSelection.AllPages,
                UserPageRangeEnabled = true
            };

            bool? print = printDialog.ShowDialog();
            if (print == true)
            {
                FlowDocument flowDocument = new FlowDocument(new Paragraph(new Run(TextBox1.Text)))
                {
                    ColumnWidth = printDialog.PrintableAreaWidth
                };
                IDocumentPaginatorSource iDocumentPaginatorSource = flowDocument;
                printDialog.PrintDocument(iDocumentPaginatorSource.DocumentPaginator, d.FileName);
            }
        }

        private void CloseCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void AppWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            appSettings.SaveSettings(AppWindow, appState, AutoSaveTimer);

            if (d.NeedsToBeSaved(TextBox1.Text))
            {
                e.Cancel = false;
            } 
        }

        void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    KeyEventArgs keyEventArgs = new(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Delete)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent
                    };
                    InputManager.Current.ProcessInput(keyEventArgs);
                }
            }
        }

        private void FontMenu_Click(object sender, RoutedEventArgs e)
        {
            FontExpander.IsExpanded = true;
            SettingsButton_Click(FontExpander, e);
        }

        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Set FindTextString to the text being searched for
            d.FindTextString = "";
            if (TextBox1.SelectionLength > 0)
            {
                d.FindTextString = TextBox1.SelectedText;
            }

            FindReplacePanel.FindTextBox.Text = d.FindTextString;

            // Display Find/Replace pane if necessary
            FindReplacePanel.ShowPanel();
            FindReplacePanel.FindTextBox.Focus();
        }

        private void FindNextCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplacePanel.FindNextButton_Click(sender, e);
        }

        private void FindPreviousCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplacePanel.FindPreviousButton_Click(sender, e);
        }

        private void ReplaceCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplacePanel.ReplaceButton_Click(sender, e);
        }

        private void ReplaceAllCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FindReplacePanel.ReplaceAllButton_Click(sender, e);
        }

        private void GoToCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Get the current line number in the text box
            d.CaretIndex = TextBox1.GetLineIndexFromCharacterIndex(TextBox1.CaretIndex) + 1;
            
            GoToLineDialog gt = new(d.CaretIndex)
            {
                Owner = this
            };

            if (gt.ShowDialog() == true)
            {
                try
                {
                    int LineNum = System.Convert.ToInt32(gt.GoToTextBox.Text);
                    if (LineNum <= TextBox1.LineCount)
                    {
                        TextBox1.SelectionStart = TextBox1.GetCharacterIndexFromLineIndex(LineNum - 1);
                        TextBox1.SelectionLength = 1;
                        TextBox1.ScrollToLine(LineNum - 1);
                        ChangePositionText();
                    }
                    else
                    {
                        MessageBoxDialog mbd = new(".NETpad - Go to line",
                            "The line number is beyond the total number of lines")
                        {
                            Owner = this
                        };

                        mbd.ShowDialog();
                        GoToCommandBinding_Executed(this, e);
                    }
                }
                catch (System.Exception)
                {
                    GoToCommandBinding_Executed(this, e);
                }
            }
        }

        private void TimeDateCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Obtain the time/date, insert it, and move the selection point to the end
            System.DateTime now = System.DateTime.Now;
            TextBox1.SelectedText = now.ToShortTimeString() + " " + now.ToShortDateString();
            TextBox1.SelectionStart += (now.ToShortTimeString() + " " + now.ToShortDateString()).Length;
            TextBox1.SelectionLength = 0;
        }

        private void ZoomCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Parameter)
            {
                case "In":
                    if (appState.ZoomValue <= 500)
                    {
                        appState.ZoomValue += 10;
                        TextBox1.FontSize = (appState.FontSizeInSettings * appState.ZoomValue) / 100;
                        ZoomText.Text = appState.ZoomValue.ToString() + "%";
                    }
                    break;
                case "Out":
                    if (appState.ZoomValue > 10)
                    {
                        appState.ZoomValue -= 10;
                        TextBox1.FontSize = (appState.FontSizeInSettings * appState.ZoomValue) / 100;
                        ZoomText.Text = appState.ZoomValue.ToString() + "%";
                    }
                    break;
                case "Restore":
                    TextBox1.FontSize = appState.FontSizeInSettings;
                    ZoomText.Text = "100%";
                    appState.ZoomValue = 100;
                    break;
            }
            appSettings.Save_ZoomSettings(appState);
        }

        private void StatusBarMenu_Click(object sender, RoutedEventArgs e)
        {
            if (StatusBar1.Visibility == Visibility.Collapsed)
            {
                StatusBar1.Visibility = Visibility.Visible;
                StatusBarMenu.IsChecked = true;
            }
            else
            {
                StatusBar1.Visibility = Visibility.Collapsed;
                StatusBarMenu.IsChecked = false;
            }

            appSettings.Save_StatusBarSettings(AppWindow);
        }

        private void WordWrapMenu_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox1.TextWrapping == TextWrapping.NoWrap)
            {
                TextBox1.TextWrapping = TextWrapping.Wrap;
                WordWrapMenu.IsChecked = true;
                WordWrapToggleButton.IsChecked = true;
                WordWrapToggleButton.Content = "On";
            }
            else
            {
                TextBox1.TextWrapping = TextWrapping.NoWrap;
                WordWrapMenu.IsChecked = false;
                WordWrapToggleButton.IsChecked = false;
                WordWrapToggleButton.Content = "Off";
            }

            appSettings.Save_WordWrapSettings(AppWindow);
        }

        private void LightThemeRadio_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.Light;
        }

        private void DarkThemeRadio_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.Dark;
        }

        private void SystemThemeRadio_Click(object sender, RoutedEventArgs e)
        {
            App.Current.ThemeMode = ThemeMode.System;
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SettingsGrid.Visibility == Visibility.Visible)
            {
                string? fontName = FontFamilyComboBox.SelectedItem.ToString();

                FontExampleLabel.FontFamily = new FontFamily(fontName);
                TextBox1.FontFamily = FontExampleLabel.FontFamily;
                appSettings.Save_FontSettings(AppWindow);
            }
        }

        private void FontStyleComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SettingsGrid.Visibility == Visibility.Visible)
            {
                switch (FontStyleComboBox.SelectedItem.ToString())
                {
                    case "Normal":
                        FontExampleLabel.FontStyle = FontStyles.Normal;
                        FontExampleLabel.FontWeight = FontWeights.Normal;
                        TextBox1.FontStyle = FontStyles.Normal;
                        TextBox1.FontWeight = FontWeights.Normal;
                        break;
                    case "Italic":
                        FontExampleLabel.FontStyle = FontStyles.Italic;
                        FontExampleLabel.FontWeight = FontWeights.Normal;
                        TextBox1.FontStyle = FontStyles.Italic;
                        TextBox1.FontWeight = FontWeights.Normal;
                        break;
                    case "Bold":
                        FontExampleLabel.FontStyle = FontStyles.Normal;
                        FontExampleLabel.FontWeight = FontWeights.Bold;
                        TextBox1.FontStyle = FontStyles.Normal;
                        TextBox1.FontWeight = FontWeights.Bold;
                        break;
                    case "Bold Italic":
                        FontExampleLabel.FontStyle = FontStyles.Italic;
                        FontExampleLabel.FontWeight = FontWeights.Bold;
                        TextBox1.FontStyle = FontStyles.Italic;
                        TextBox1.FontWeight = FontWeights.Bold;
                        break;
                    default:
                        FontExampleLabel.FontStyle = FontStyles.Normal;
                        FontExampleLabel.FontWeight = FontWeights.Normal;
                        TextBox1.FontStyle = FontStyles.Normal;
                        TextBox1.FontWeight = FontWeights.Normal;
                        break;
                }

                appSettings.Save_FontSettings(AppWindow);
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SettingsGrid.Visibility == Visibility.Visible)
            {
                string? size = FontSizeComboBox.SelectedItem.ToString();
                FontExampleLabel.FontSize = Convert.ToDouble(size);
                TextBox1.FontSize = Convert.ToDouble(size);
                appState.FontSizeInSettings = Convert.ToDouble(size);

                appSettings.Save_FontSettings(AppWindow);
            }
        }

        private void SpellCheckToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (appState.SpellCheck)
            {
                // Disable Spell check
                SpellCheckToggleButton.IsChecked = false;
                SpellCheckToggleButton.Content = "Off";
            }
            else
            {
                // Enable Spell check
                SpellCheckToggleButton.IsChecked = true;
                SpellCheckToggleButton.Content = "On";
            }
            appState.SpellCheck = !appState.SpellCheck;
            TextBox1.SpellCheck.IsEnabled = appState.SpellCheck;

            appSettings.Save_SpellingSettings(appState);
        }

        private void StatusBarCount_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            appState.WordCount = !appState.WordCount;
            DisplayCount();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            d.NewDocument(AppWindow);
        }
    }
}