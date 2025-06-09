using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DotNetPad32
{
    internal class Recents
    {
        public System.Collections.Specialized.StringCollection MyRecentsList { get; set; } = [];

        public Recents()
        {
            MyRecentsList = Settings.Default.MyRecents;
        }

        public bool LoadRecents(MenuItem m)
        {
            MenuItem MyMenu = m;
            MyMenu.Items.Clear();

            MessageBox.Show(MyRecentsList.Count.ToString());

            if (MyRecentsList == null || MyRecentsList.Count == 0)
            {
                // MyRecentsList is empty, use default Recents submenu
                MenuItem NoItems = new MenuItem() { Header = "No recent files", IsEnabled = false };
                MyMenu.Items.Add(NoItems);
            }
            else
            {
                // Load the recents list and populate the submenu
                for (int x = 0; x < MyRecentsList.Count; x++)
                {
                    MenuItem newItem = new();
                    newItem.Header = (Path.GetFileNameWithoutExtension(MyRecentsList[x]));
                    newItem.Tag = MyRecentsList[x];
                    newItem.Click += new RoutedEventHandler(RecentMenuItem_Click);
                    MyMenu.Items.Add(newItem);
                }
                MyMenu.Items.Add(new Separator());

                MenuItem ClearListMenuItem = new MenuItem();
                ClearListMenuItem.Header = "Clear list";
                ClearListMenuItem.Click += new RoutedEventHandler(ClearListMenuItem_Click);
                MyMenu.Items.Add(ClearListMenuItem);
            }
            return true;
        }

        public void AddRecent(string f, MenuItem m)
        {
            if (MyRecentsList.Contains(f))
            {
                MyRecentsList.Remove(f);
            }
            MyRecentsList.Insert(0, f);
            // Limit to 10 recent files
            if (MyRecentsList.Count > 10)
            {
                MyRecentsList.RemoveAt(10);
            }
            
            Settings.Default.MyRecents = MyRecentsList;
            Settings.Default.Save();
        }

        public void RecentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            Document d = mw.d;
            MenuItem? mi = sender as MenuItem;
            string s = mi.Tag.ToString();

            d.OpenDocument(mw.TextBox1, s);
        }

        public void ClearListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)Application.Current.MainWindow;
            MenuItem MyMenu = (MenuItem)mw.RecentMenu;
            MyMenu.Items.Clear();
            Settings.Default.MyRecents.Clear();
            LoadRecents(MyMenu);

            Settings.Default.MyRecents = MyRecentsList;
            Settings.Default.Save();
        }
    }
}