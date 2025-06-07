using System.Windows;

namespace DotNetPad32
{
    public partial class GoToLineDialog : Window
    {
        public int LineNumber;

        public GoToLineDialog(int ln)
        {
            InitializeComponent();
            LineNumber = ln;
            GoToTextBox.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GoToTextBox.Text = LineNumber.ToString();
            GoToTextBox.Focus();
            GoToTextBox.SelectAll();

            // Prevent the window from being resized
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
        }
        private void GoToLineButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
