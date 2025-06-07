using System.Windows;
using System.Windows.Controls;

namespace DotNetPad32
{
    public partial class FindReplace : UserControl
    {
        public MainWindow mw = ((MainWindow)App.Current.MainWindow);
        public Document d = (Document)((MainWindow)App.Current.MainWindow).d;

        public FindReplace()
        {
            InitializeComponent();

            ReplaceTextBox.Visibility = Visibility.Collapsed;
            ReplaceButton.Visibility = Visibility.Collapsed;
            ReplaceAllButton.Visibility = Visibility.Collapsed;

            // Configure default event handler
            FindNextButton.IsDefault = true;
            ReplaceButton.IsDefault = false;
        }

        public void ShowPanel()
        {
            // FindTextIndex(mw.TextBox1.SelectionStart, false);
            FindTheText();

            if (FindReplacePanel.Visibility != Visibility.Visible)
            {
                FindReplacePanel.Visibility = Visibility.Visible;
            }

            // Finally, select and focus the Find text box
            FindTextBox.SelectAll();
            FindTextBox.Focus();
        }

        public void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            d.FindTextString = FindTextBox.Text;
            d.FindLastIndexFound = mw.TextBox1.SelectionStart;

            if (d.FindTextString != "")
            {
                FindTextIndex(d.FindLastIndexFound + mw.TextBox1.SelectionLength, false);
                FindTheText();
            }
        }

        public void FindPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            d.FindTextString = FindTextBox.Text;
            d.FindLastIndexFound = mw.TextBox1.SelectionStart;

            if (FindReplacePanel.Visibility == Visibility.Visible)
            {
                d.FindTextString = FindTextBox.Text;
            }

            if (d.FindTextString != "")
            {
                FindTextIndex(d.FindLastIndexFound, true);
                FindTheText();
            }
        }

        public void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // If a Replace control isn't visible, display Find/Replace with Replace controls
            if (ReplaceTextBox.Visibility == Visibility.Collapsed)
            {
                DisplayReplaceControls();
            }
            else
            {
                d.FindTextString = FindTextBox.Text;
                d.FindLastIndexFound = mw.TextBox1.SelectionStart;

                // Find text from current cursor position
                FindTextIndex(mw.TextBox1.SelectionStart, false);

                if (d.FindTextString.Length != 0 && ReplaceTextBox.Text.Length != 0)
                {
                    if (d.FindLastIndexFound > -1)
                    {
                        mw.TextBox1.Text = mw.TextBox1.Text.Substring(0, d.FindLastIndexFound) + ReplaceTextBox.Text
                            + mw.TextBox1.Text.Substring(d.FindLastIndexFound + d.FindTextString.Length);
                        mw.TextBox1.Focus();
                        mw.TextBox1.SelectionStart = d.FindLastIndexFound;
                    }
                    else
                    {
                        FindFailed();
                    }
                }
            }
        }

        public void DisplayReplaceControls()
        {
            // First, make sure the panel is visible
            // ?
            
            // Unhide the Replace controls
            ReplaceTextBox.Visibility = Visibility.Visible;
            ReplaceButton.Visibility = Visibility.Visible;
            ReplaceAllButton.Visibility = Visibility.Visible;

            // Configure default event handler
            FindNextButton.IsDefault = false;
            ReplaceButton.IsDefault = true;

            // Finally, select and focus the appropriate text box
            if (FindTextBox.Text != "")
            {
                ReplaceTextBox.SelectAll();
                ReplaceTextBox.Focus();
            }
        }

        public void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            // If a Replace control isn't visible, display Find/Replace with Replace controls
            if (ReplaceTextBox.Visibility == Visibility.Collapsed)
            {
                DisplayReplaceControls();
            }
            else
            {
                d.FindTextString = FindTextBox.Text;
                d.FindLastIndexFound = mw.TextBox1.SelectionStart;

                // Find text from current cursor position
                FindTextIndex(mw.TextBox1.SelectionStart, false);

                if (FindTextBox.Text.Length != 0 && ReplaceTextBox.Text.Length != 0)
                {
                    FindTextIndex(0, false);

                    if (d.FindLastIndexFound > -1)
                    {
                        string? NewText = Microsoft.VisualBasic.Strings.Replace(mw.TextBox1.Text, d.FindTextString,
                            ReplaceTextBox.Text, 1);
                        mw.TextBox1.Text = NewText;
                    }
                    else
                    {
                        FindFailed();
                    }
                }
            }
        }

        private void CloseFindReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the Replace controls
            ReplaceTextBox.Visibility = Visibility.Collapsed;
            ReplaceButton.Visibility = Visibility.Collapsed;
            ReplaceAllButton.Visibility = Visibility.Collapsed;

            FindReplacePanel.Visibility = Visibility.Collapsed;
            ((MainWindow)App.Current.MainWindow).TextBox1.Focus();
        }

        private void FindFailed()
        {
            //MessageBoxDialog mbd = new(".NETpad", "Cannot find " + (char)34 +
            //    dts[0].FindTextString + (char)34)
            //{
            //    Owner = this
            //};

            //mbd.ShowDialog();
        }

        private void FindTheText()
        {
            if (d.FindLastIndexFound > -1)
            {
                // Select the text
                mw.TextBox1.Select(d.FindLastIndexFound, d.FindTextString.Length);
                // Navigate to that place in the text if needed
                mw.TextBox1.ScrollToLine(mw.TextBox1.GetLineIndexFromCharacterIndex((d.FindLastIndexFound)));
                // Then, give the focus to the text box so you can see the selection
                mw.TextBox1.Focus();
            }
            else
            {
                FindFailed();
            }
        }

        private void FindTextIndex(int FindFromIndex, bool FindPreviousIndex)
        {
            if (FindPreviousIndex == false)
            {
                d.FindLastIndexFound = mw.TextBox1.Text.IndexOf(d.FindTextString, FindFromIndex);
                if (d.FindLastIndexFound <= 0)
                {
                    // If text is not found, try searching from the beginning
                    d.FindLastIndexFound = mw.TextBox1.Text.IndexOf(d.FindTextString, 0);
                }
            }
            else
            {
                d.FindLastIndexFound = mw.TextBox1.Text.LastIndexOf(d.FindTextString, FindFromIndex);
                if (d.FindLastIndexFound <= -1)
                {
                    //  If text is not found, try searching from the end
                    d.FindLastIndexFound = mw.TextBox1.Text.LastIndexOf(d.FindTextString, mw.TextBox1.Text.Length - 1);
                }
            }
        }
    }
}
