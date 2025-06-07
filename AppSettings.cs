using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace DotNetPad32
{
    internal class AppSettings
    {
        public void LoadSettings(MainWindow mw, AppState aps, System.Timers.Timer mt)
        {
            Load_ThemeSettings(mw);
            Load_AppWindowMetrics();
            Load_FontSettings(mw);
            Load_SpellingSettings(mw, aps);
            Load_WordWrapSettings(mw);
            Load_StatusBarSettings(mw);
            Load_AutoSaveTimerSettings(mw, aps, mt);
            Load_ZoomSettings(mw, aps);
        }

        public void SaveSettings(MainWindow mw, AppState aps, System.Timers.Timer mt)
        {
            Save_ThemeSettings();
            Save_AppWindowMetrics();
            Save_FontSettings(mw);
            Save_SpellingSettings(aps);
            Save_WordWrapSettings(mw);
            Save_StatusBarSettings(mw);
            Save_AutoSaveSettings(aps);
            Save_ZoomSettings(aps);
        }

        public void Load_ThemeSettings(MainWindow mw)
        {
#pragma warning disable WPF0001
            // App theme
            string t = Settings.Default.MyTheme;
            Application.Current.ThemeMode = t switch
            {
                "System" => ThemeMode.System,
                "Light" => ThemeMode.Light,
                "Dark" => ThemeMode.Dark,
                _ => ThemeMode.System,
            };

            mw.LightThemeRadio.IsChecked = false;
            mw.DarkThemeRadio.IsChecked = false;
            mw.SystemThemeRadio.IsChecked = false;

            if (Application.Current.ThemeMode == ThemeMode.Light)
            {
                mw.LightThemeRadio.IsChecked = true;
            }
            else if (Application.Current.ThemeMode == ThemeMode.Dark)
            {
                mw.DarkThemeRadio.IsChecked = true;
            }
            else
            {
                mw.SystemThemeRadio.IsChecked = true;
            }
#pragma warning restore WPF0001
        }

        public void Load_AppWindowMetrics()
        {
            // App window location and size
            Application.Current.MainWindow.Left = Settings.Default.MyLocationX;
            Application.Current.MainWindow.Top = Settings.Default.MyLocationY;

            if (Settings.Default.MyMaximized == true)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;

            Application.Current.MainWindow.Width = Settings.Default.MyWidth;
            Application.Current.MainWindow.Height = Settings.Default.MyHeight;
        }

        public void Load_FontSettings(MainWindow mw)
        {
            string fontName = Settings.Default.MyFontFamily;

            // Font settings
            mw.TextBox1.FontFamily = new FontFamily(Settings.Default.MyFontFamily);
            if (Settings.Default.MyFontBold == true)
                mw.TextBox1.FontWeight = FontWeights.Bold;
            else
                mw.TextBox1.FontWeight = FontWeights.Normal;
            if (Settings.Default.MyFontItalic == true)
                mw.TextBox1.FontStyle = FontStyles.Italic;
            else
                mw.TextBox1.FontStyle = FontStyles.Normal;
            mw.TextBox1.FontSize = Settings.Default.MyFontSize;

            // Font example combo box in settings
            mw.FontExampleLabel.FontFamily = new System.Windows.Media.FontFamily(fontName);
            mw.FontExampleLabel.FontWeight = Settings.Default.MyFontBold ? FontWeights.Bold : FontWeights.Normal;
            mw.FontExampleLabel.FontStyle = Settings.Default.MyFontItalic ? FontStyles.Italic : FontStyles.Normal;
            mw.FontExampleLabel.FontSize = Settings.Default.MyFontSize;

            for (int x = 0; x <= Fonts.SystemFontFamilies.Count - 1; x++)
            {
                if (mw.FontFamilyComboBox.Items[x].ToString() == mw.FontExampleLabel.FontFamily.ToString())
                {
                    mw.FontFamilyComboBox.SelectedItem = mw.FontFamilyComboBox.Items[x];
                    break;
                }
            }

            List<string> fontStyle =
            [
                "Normal", "Italic", "Bold", "Bold Italic"
            ];
            mw.FontStyleComboBox.DataContext = fontStyle;

            if (mw.FontExampleLabel.FontStyle == FontStyles.Normal)
                mw.FontSizeComboBox.SelectedIndex = 0;
            if (mw.FontExampleLabel.FontStyle == FontStyles.Italic)
                mw.FontSizeComboBox.SelectedIndex = 1;
            if (mw.FontWeight == FontWeights.Bold)
                mw.FontSizeComboBox.SelectedIndex = 2;
            if (mw.FontExampleLabel.FontWeight == FontWeights.Bold && mw.FontExampleLabel.FontStyle == FontStyles.Italic)
                mw.FontSizeComboBox.SelectedIndex = 3;

            List<double> fontSize =
            [
                8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72
            ];
            mw.FontSizeComboBox.DataContext = fontSize;

            for (int x = 0; x < fontSize.Count; x++)
                if (fontSize[x].ToString() == mw.FontExampleLabel.FontSize.ToString())
                {
                    mw.FontSizeComboBox.SelectedIndex = x;
                    break;
                }
        }

        public void Load_SpellingSettings(MainWindow mw, AppState aps)
        {
            // Spelling settings
            aps.SpellCheck = Settings.Default.MySpellCheck;
            mw.SpellCheckToggleButton.IsChecked = aps.SpellCheck;
            // Application.Current.Properties["SpellCheck"] = (bool)Settings.Default.MySpellCheck;
        }

        public void Load_WordWrapSettings(MainWindow mw)
        {
            // Word wrap
            if (Settings.Default.MyWordWrap == true)
            {
                mw.WordWrapMenu.IsChecked = true;
                mw.WordWrapToggleButton.IsChecked = true;
                mw.WordWrapToggleButton.Content = "On";
                mw.TextBox1.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                mw.WordWrapMenu.IsChecked = false;
                mw.WordWrapToggleButton.IsChecked = false;
                mw.WordWrapToggleButton.Content = "Off";
                mw.TextBox1.TextWrapping = TextWrapping.NoWrap;
            }
        }

        public void Load_StatusBarSettings(MainWindow mw)
        {
            // Status bar
            if (Settings.Default.MyStatusBar == true)
            {
                mw.StatusBar1.Visibility = Visibility.Visible;
                mw.StatusBarMenu.IsChecked = true;
            }
            else
            {
                mw.StatusBar1.Visibility = Visibility.Collapsed;
                mw.StatusBarMenu.IsChecked = false;
            }
        }

        public void Load_ZoomSettings(MainWindow mw, AppState aps)
        {
            // Zoom
            aps.ZoomValue = Settings.Default.MyZoom;
            mw.ZoomText.Text = aps.ZoomValue.ToString() + "%";
            if (aps.ZoomValue != 100)
                mw.TextBox1.FontSize = (aps.FontSizeInSettings * aps.ZoomValue) / 100;
        }

        public void Load_AutoSaveTimerSettings(MainWindow mw, AppState aps, System.Timers.Timer mt)
        {
            // Auto save with timer
            // 2025 change: Only start the timer is AutoSave is true
            aps.AutoSave = Settings.Default.MyAutoSave;
        }

        public void Save_ThemeSettings()
        {
            // App theme
            Settings.Default.MyTheme = Application.Current.ThemeMode.ToString();
            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_AppWindowMetrics()
        {
            // Window location and size
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                Settings.Default.MyMaximized = true;
            else
                Settings.Default.MyMaximized = false;

            Settings.Default.MyLocationX = Application.Current.MainWindow.Left;
            Settings.Default.MyLocationY = Application.Current.MainWindow.Top;

            Settings.Default.MyWidth = Application.Current.MainWindow.Width;
            Settings.Default.MyHeight = Application.Current.MainWindow.Height;

            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_FontSettings(MainWindow mw)
        {
            // Font settings
            Settings.Default.MyFontFamily = mw.TextBox1.FontFamily.ToString();
            Settings.Default.MyFontBold = mw.TextBox1.FontWeight == FontWeights.Bold;
            Settings.Default.MyFontItalic = mw.TextBox1.FontStyle == FontStyles.Italic;
            Settings.Default.MyFontSize = mw.TextBox1.FontSize;

            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_SpellingSettings(AppState aps)
        {
            // Spelling settings
            Settings.Default.MySpellCheck = aps.SpellCheck;
            // Settings.Default.MySpellCheck = (bool)Application.Current.Properties["SpellCheck"];

            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_WordWrapSettings(MainWindow mw)
        {
            // Word wrap
            Settings.Default.MyWordWrap = mw.WordWrapMenu.IsChecked == true;

            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_StatusBarSettings(MainWindow mw)
        {
            // Status bar
            Settings.Default.MyStatusBar = mw.StatusBar1.Visibility == Visibility.Visible;

            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_AutoSaveSettings(AppState aps)
        {
            // Auto save
            Settings.Default.MyAutoSave = aps.AutoSave;

            // Save changes to app settings
            Settings.Default.Save();
        }

        public void Save_ZoomSettings(AppState aps)
        {
            // Zoom
            Settings.Default.MyZoom = aps.ZoomValue;

            // Save changes to app settings
            Settings.Default.Save();
        }
    }
}
