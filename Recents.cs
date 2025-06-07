using System.IO;
using System.Windows.Controls;

namespace DotNetPad32
{
    internal class Recents
    {
        public System.Collections.Specialized.StringCollection MyRecents { get; set; } = [];

        public Recents(MenuItem m)
        {
            Separator s = new Separator();
            s = (Separator)m.Items[0];
            MenuItem MyMenu = m;

            // if (MyRecents[0] == "Empty")
            if (MyRecents.Count == 0)
            {
                // The recent documents collection is empty, so edit the submenu
                s.Visibility = System.Windows.Visibility.Collapsed;
                MyMenu = (MenuItem)m.Items[1];
                MyMenu.Header = "No recent files";
                MyMenu.IsEnabled = false;
            }
            else
            {
                // Add recents to the menu
                s.Visibility = System.Windows.Visibility.Visible;
                for (int x = 1; x < MyRecents.Count - 1; x++)
                {
                    // Populate recents submenu
                    MenuItem newMenuItem = new();
                    newMenuItem.Header = Path.GetFileNameWithoutExtension(Settings.Default.MyRecents[x]);
                    newMenuItem.Click += (sender, e) => 
                    {
                        // Handle click event for recent files
                        
                    };
                }
            }
        }

        public Boolean ResetRecents(MenuItem m)
        {
            
            return true;
        }
    }
}