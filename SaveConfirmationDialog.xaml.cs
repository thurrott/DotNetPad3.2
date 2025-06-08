using System.Windows;

namespace DotNetPad32
{
    public partial class SaveConfirmationDialog : Window
    {
        public string Choice { get; private set; } = "Cancel";

        public SaveConfirmationDialog(string document)
        {
            InitializeComponent();

            ConfirmTextBlock.Text = "Do you want to save the changes to " + System.IO.Path.GetFileNameWithoutExtension(document) + "?";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfirmationSaveButton.Focus();

            // Prevent the window from being resized
            this.MinWidth = this.Width;
            this.MinHeight = this.Height;
            this.MaxWidth = this.Width;
            this.MaxHeight = this.Height;
        }

        private void ConfirmationSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Choice = "Save";
            Close();
        }

        private void ConfirmationDontSaveButton_Click(object sender, RoutedEventArgs e)
        {
            Choice = "Don't save";
            Close();
        }

        private void ConfirmationCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Choice = "Cancel";
            Close();
        }
    }
}
