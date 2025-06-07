using System.Windows;

namespace DotNetPad32
{
    public partial class AutoSaveDialog : Window
    {
        public AutoSaveDialog(bool AutoSave)
        {
            InitializeComponent();

            if (AutoSave == true)
            {
                // You are disabling Auto save
                AutoSaveTextBlock2.Visibility = Visibility.Collapsed;
                AutoSaveTextBlock.Text = "Do you want to disable Auto save?";
                ToggleAutoSaveButton.Content = "Disable";
            }
            else
            {
                // You are enabling Auto save
                AutoSaveTextBlock2.Visibility = Visibility.Visible;
                AutoSaveTextBlock.Text = "Do you want to enable Auto save?";
                ToggleAutoSaveButton.Content = "Enable";
            }
            ToggleAutoSaveButton.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleAutoSaveButton.Focus();

            // Prevent the window from being resized
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
        }

        private void ToggleAutoSaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void CancelAutoSaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
