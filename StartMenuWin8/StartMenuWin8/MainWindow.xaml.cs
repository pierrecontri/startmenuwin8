using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Security.Principal;

namespace StartMenuWin8
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private List<object> treeViewExplorer = new List<object>();

        public string CurrentUserName
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Environment.UserName); }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
        }

        private void Startup_Menu_Loaded(object sender, RoutedEventArgs e)
        {
            string fileProfilPath = Environment.ExpandEnvironmentVariables(Constants.startMenuPath);
            string fileCommonPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), Constants.startMenuPrograms);

            string userName = CurrentUserName;
            this.LabelUserName.Content = CurrentUserName;
            this.GroupBoxUserFolders.Header = CurrentUserName + Constants.folderTitle;

            this.tbButtonUser.Text = userName;
            /*
            FillContentDirectory(fileProfilPath, TreeViewStartMenuExplorer.Items);
            FillContentDirectory(fileCommonPath, TreeViewStartMenuExplorer.Items);
            FillContentSubDirectory(fileProfilPath, TreeViewStartMenuExplorer.Items);
            FillContentSubDirectory(fileCommonPath, TreeViewStartMenuExplorer.Items);
            */

            FillContentDirectory(fileProfilPath, TreeViewItemPersonnalShortcut.Items);
            FillContentDirectory(fileCommonPath, TreeViewItemCommonShortcut.Items);
            FillContentSubDirectory(fileProfilPath, TreeViewItemPersonnalShortcut.Items);
            FillContentSubDirectory(fileCommonPath, TreeViewItemCommonShortcut.Items);
            
            //this.TreeViewStartMenuExplorer.DataContext = this.treeViewExplorer;

            // focus on cmd
            this.TextBoxExecute.Focus();
        }

        void FillContentDirectory(string directoryName, ItemCollection itemColl)
        {
            foreach (string sProg in Directory.GetFiles(directoryName))
            {
                string sProg2 = System.IO.Path.GetFileNameWithoutExtension(sProg);

                FileInfo fileInfo = new FileInfo(sProg);
                if ((fileInfo.Attributes & (FileAttributes.System | FileAttributes.Hidden)) != 0)
                    continue;

                TreeViewItem item = new TreeViewItem();
                item.Header = sProg2;
                item.Tag = sProg;
                item.DataContext = new TreeViewExecutable(sProg2, sProg);
                item.Style = TryFindResource("TreeViewItemExecutable") as Style;
                item.Selected += StartProgramEventHandler;
                itemColl.Add(item);

                //treeViewExplorer.Add(new TreeViewExecutable(sProg2, sProg));
            }
        }

        void FillContentSubDirectory(string directoryName, ItemCollection itemColl)
        {
            foreach (string s in Directory.GetDirectories(directoryName))
            {
                string s2 = System.IO.Path.GetFileNameWithoutExtension(s);

                TreeViewItem item = new TreeViewItem();
                item.Header = s2;
                item.Tag = s;
                item.DataContext = new TreeViewItemDirectoryFolder(s2, s);
                item.Items.Add(s);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                item.Style = TryFindResource("TreeViewItemFolder") as Style;
                itemColl.Add(item);

                //treeViewExplorer.Add(new TreeViewItemDirectoryFolder(s2, s));
            }
        }

        void StartProgramEventHandler(object sender, RoutedEventArgs e)
        {
            string progPath = string.Empty;
            try
            {
                TreeViewItem item = sender as TreeViewItem;
                progPath = item.Tag as string;
                FileInfo fileInfo = new FileInfo(progPath);

                ProcessStartInfo runExplorer = new ProcessStartInfo();
                runExplorer.FileName = progPath;
                runExplorer.WorkingDirectory = fileInfo.DirectoryName;
                System.Diagnostics.Process.Start(runExplorer);

                Application.Current.Shutdown();
            }
            catch (Exception /*ex*/)
            {
                MessageBox.Show("Can not open '" + progPath + "' !");
            }
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)sender;

                if (item.Items.Count == 1 && item.Tag.ToString().Equals(item.Items[0]))
                {
                    item.Items.Clear();
                    FillContentDirectory(item.Tag.ToString(), item.Items);
                    FillContentSubDirectory(item.Tag.ToString(), item.Items);
                }
            }
            catch (Exception) { }
        }

        private void TextBoxExecute_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (e.Key == Key.Return)
            {
                try
                {
                    ProcessStartInfo runExplorer = new ProcessStartInfo();
                    runExplorer.FileName = txtBox.Text;
                    runExplorer.WorkingDirectory = Environment.GetEnvironmentVariable(Environment.SpecialFolder.UserProfile.ToString());
                    System.Diagnostics.Process.Start(runExplorer);

                    Application.Current.Shutdown();
                }
                catch (Exception)
                {
                }
            }
            else if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }


        private void Grid_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }

        #region User
        private void CmdUserFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdDocumentFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdPicturesFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdMusicFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));

                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Computer
        private void CmdComputer(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Constants.explorer, Constants.keyRegisterComputer);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdPanelConfig(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Environment.ExpandEnvironmentVariables(Constants.panelConfig));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdDeviceManagemenet(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Constants.deviceManagement);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Shutdown
        private void CmdShutdown(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Constants.cmdShutdown, StringEnumValue.GetStringValue(ShutdownParameter.Shutdown));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdHibernate(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Constants.cmdShutdown, StringEnumValue.GetStringValue(ShutdownParameter.Hibernate));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdRestart(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Constants.cmdShutdown, StringEnumValue.GetStringValue(ShutdownParameter.Restart));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdLogOff(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Constants.cmdShutdown, StringEnumValue.GetStringValue(ShutdownParameter.LogOff));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }

        private void CmdShutdownWithFasterStart(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Constants.cmdShutdown, StringEnumValue.GetStringValue(ShutdownParameter.ShutdownWithFasterStart));
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
