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
using System.ComponentModel;
using System.Collections;

namespace StartMenuWin8
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region initialise
        public MainWindow()
        {
            InitializeComponent();
            this.Top = System.Windows.SystemParameters.WorkArea.Bottom - this.Height;
        }

        private void Startup_Menu_Loaded(object sender, RoutedEventArgs e)
        {
            #region StartMenu Customisation
            string userName = this.CurrentUserName;
            this.LabelUserName.Content = userName;
            this.GroupBoxUserFolders.Header = userName + Constants.folderTitle;
            this.tbButtonUser.Text = userName;
            #endregion StartMenu Customisation

            this.TreeViewStartMenuExplorer.ItemsSource = this.ContentOfStartMenu;

            // focus on cmd
            this.TextBoxExecute.Focus();
        }
        #endregion initialise

        #region HMI interact
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
            else if ((new[] { Key.Escape, Key.LWin, Key.RWin }).Contains(e.Key))
            {
                Application.Current.Shutdown();
            }
        }


        private void Grid_KeyDown_1(object sender, KeyEventArgs e)
        {
            if ((new[] { Key.Escape, Key.LWin, Key.RWin }).Contains(e.Key))
            {
                Application.Current.Shutdown();
            }
        }
        #endregion HMI interact

        #region Content Directory / SubDirectcory
        public IList ContentOfStartMenu
        {
            get
            {
                string fileProfilPath = Environment.ExpandEnvironmentVariables(Constants.startMenuPath);
                string fileCommonPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), Constants.startMenuPrograms);

                IList itemColl = new List<TreeViewItem>();
                this.FillContentFilesAndFolderIntoDirectory(new string[] { fileProfilPath, fileCommonPath }, itemColl);
                return itemColl;
            }
        }

        protected void FillContentFilesAndFolderIntoDirectory(string[] directoriesName, IList itemColl)
        {
            // file Part
            directoriesName
                .SelectMany(dirName => Directory.GetFiles(dirName).ToList<string>())
                // filter the file system and file hidden
                .Where(sProgInfo =>
                    ((new FileInfo(sProgInfo)).Attributes & (FileAttributes.System | FileAttributes.Hidden)) == 0
                        )
                // Get the absolute name and relative name
                .Select<string, KeyValuePair<string, string>>(str => new KeyValuePair<string, string>(str, System.IO.Path.GetFileNameWithoutExtension(str)))
                .OrderBy(strKeyVal => strKeyVal.Value)
                .ToList()
                .ForEach(sProg => AppendProgrammIntoItemCollection(sProg, itemColl));

            // folder part
            directoriesName
                .SelectMany(dirName => Directory.GetDirectories(dirName).ToList<string>())
                // Get the absolute name and relative name
                .Select<string, KeyValuePair<string, string>>(str => new KeyValuePair<string, string>(str, System.IO.Path.GetFileNameWithoutExtension(str)))
                .OrderBy(strKeyVal => strKeyVal.Value)
                .ToList()
                .ForEach(subDirectory => AppendFolderIntoItemCollection(subDirectory, itemColl));
        }

        protected void AppendProgrammIntoItemCollection(KeyValuePair<string, string> sProg, IList itemColl)
        {
            // check if the itemCollection is not already present
            if (itemColl.OfType<TreeViewItem>().Any(progExisting => progExisting.Header.Equals(sProg.Value)))
            {
                return;
            }

            TreeViewItem item = new TreeViewItem();
            item.Header = sProg.Value;
            item.Tag = sProg.Key;
            item.DataContext = new TreeViewItemExecutable(sProg.Value, sProg.Key);
            item.Style = TryFindResource("TreeViewItemExecutable") as Style;
            item.Selected += StartProgramEventHandler;
            itemColl.Add(item);
        }

        protected void AppendFolderIntoItemCollection(KeyValuePair<string, string> subDirectory, IList itemColl)
        {
            TreeViewItem item = null;

            // check if the itemCollection is not already present
            var itemExisting = itemColl.OfType<TreeViewItem>().Where(progExisting => progExisting.Header.Equals(subDirectory.Value));
            // get the folder if it's already present
            if (itemExisting.Any())
            {
                item = itemExisting.FirstOrDefault();
                item.DataContext = new[] { item.DataContext as TreeViewItemFolder,
                                                        new TreeViewItemFolder(subDirectory.Value, subDirectory.Key) };
            }
            else
            {
                item = new TreeViewItem();

                item.Header = subDirectory.Value;
                item.Tag = subDirectory.Key;
                item.DataContext = new TreeViewItemFolder(subDirectory.Value, subDirectory.Key);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                item.Style = TryFindResource("TreeViewItemFolder") as Style;

                itemColl.Add(item);
            }

            if (item != null)
            {
                this.FillContentFilesAndFolderIntoDirectory(new string[] { subDirectory.Key }, item.Items);
            }
        }

        #endregion Content Directory / SubDirectcory

        #region User event
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
                    this.FillContentFilesAndFolderIntoDirectory(new string[] { item.Tag.ToString() }, item.Items);
                }
            }
            catch (Exception) { }
        }
        #endregion User Event

        #region User
        public string CurrentUserName
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Environment.UserName); }
        }

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
