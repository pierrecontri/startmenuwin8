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
using System.Text.RegularExpressions;
using System.Reflection;

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
                #region userprofile
                string fileRootProfilPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                string fileProfilPath = System.IO.Path.Combine(fileRootProfilPath, Constants.startMenuPrograms);
                #endregion userprofile

                #region commonprofile
                string fileRootCommonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                string fileCommonPath = System.IO.Path.Combine(fileRootCommonPath, Constants.startMenuPrograms);
                #endregion commonprofile

                IList itemColl = new List<TreeViewItem>();
                this.FillContentFilesAndFolderIntoDirectory(new string[] { fileProfilPath, fileCommonPath, fileRootProfilPath, fileRootCommonPath }, itemColl);
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
                .Distinct()
                .ToList()
                .ForEach(sProg => AppendProgrammIntoItemCollection(sProg, itemColl));

            // check if there is inserted a child manually
            var filterDirectoryName = directoriesName.Where(tmpStr =>
                                            !directoriesName.Except(new string[] { tmpStr }, StringComparer.OrdinalIgnoreCase)
                                                .Where(tmpStr2 => Regex.IsMatch(tmpStr2, Regex.Escape(tmpStr) + ".+"))
                                                .Any()
                                            ).ToList();

            // folder part
            filterDirectoryName
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

                var dataContext = item.DataContext as IList<TreeViewItemFolder>;
                if (dataContext != null)
                {
                    var dataCtx = new List<TreeViewItemFolder>(dataContext);
                    dataCtx.Add(new TreeViewItemFolder(subDirectory.Value, subDirectory.Key));
                    item.DataContext = dataCtx;
                }
            }
            else
            {
                item = new TreeViewItem();

                item.Header = subDirectory.Value;
                item.Tag = subDirectory.Key;
                item.DataContext = new List<TreeViewItemFolder> { new TreeViewItemFolder(subDirectory.Value, subDirectory.Key) };
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                item.Selected += item_Selected;
                item.Style = TryFindResource("TreeViewItemFolder") as Style;

                itemColl.Add(item);
            }
        }

        #endregion Content Directory / SubDirectcory

        #region User event
        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
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
                Process.Start(runExplorer);
            }
            catch (Exception /*ex*/)
            {
                MessageBox.Show("Can not open '" + progPath + "' !");
            }
        }

        private void item_Selected(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                // explore the content of folder if it is not already done
                item.IsExpanded = !item.IsExpanded;
                // unselect the item after browse the content
                item.IsSelected = false;
            }
        }
        
        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)sender;
                if (!(item.IsSelected && item.IsExpanded))
                {
                    return;
                }

                string[] tmpPathFolders = null;

                // build the content of the folder
                // test the item.DataContext
                // get the path of folder content
                var dataCtx = item.DataContext;
                if (dataCtx is IList<TreeViewItemFolder>)
                {
                    // get the many folder of the context
                    var dataCtxArray = (dataCtx as IList<TreeViewItemFolder>);
                    tmpPathFolders = dataCtxArray.Select(tmpTreeViewItemFolder => tmpTreeViewItemFolder.Path).ToArray();
                }

                if (tmpPathFolders != null)
                {
                    item.Items.Clear();
                    this.FillContentFilesAndFolderIntoDirectory(tmpPathFolders, item.Items);
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
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            }
            catch (Exception)
            {
            }
        }

        private void CmdDocumentFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            catch (Exception)
            {
            }
        }

        private void CmdPicturesFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            }
            catch (Exception)
            {
            }
        }

        private void CmdMusicFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
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
                Process.Start(Constants.explorer, Constants.keyRegisterComputer);
            }
            catch (Exception)
            {
            }
        }

        private void CmdPanelConfig(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Environment.ExpandEnvironmentVariables(Constants.panelConfig));
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
                StartShutDown(StringEnumValue.GetStringValue(ShutdownParameter.Shutdown));
            }
            catch (Exception)
            {
            }
        }

        private void CmdHibernate(object sender, RoutedEventArgs e)
        {
            try
            {
                StartShutDown(StringEnumValue.GetStringValue(ShutdownParameter.Hibernate));
            }
            catch (Exception)
            {
            }
        }

        private void CmdRestart(object sender, RoutedEventArgs e)
        {
            try
            {
                StartShutDown(StringEnumValue.GetStringValue(ShutdownParameter.Restart));
            }
            catch (Exception)
            {
            }
        }

        private void CmdStandby(object sender, RoutedEventArgs e)
        {
            try
            {
                StartShutDown(StringEnumValue.GetStringValue(ShutdownParameter.StandBy));
            }
            catch (Exception)
            {
            }
        }

        private void CmdLogOff(object sender, RoutedEventArgs e)
        {
            try
            {
                StartShutDown(StringEnumValue.GetStringValue(ShutdownParameter.LogOff));
            }
            catch (Exception)
            {
            }
        }

        private void CmdShutdownWithFasterStart(object sender, RoutedEventArgs e)
        {
            try
            {
                StartShutDown(StringEnumValue.GetStringValue(ShutdownParameter.ShutdownWithFasterStart));
            }
            catch (Exception)
            {
            }
        }

        private static void StartShutDown(string param)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C " + Constants.cmdShutdown + " " + param;
            Process.Start(proc);
        }
        #endregion
    }
}
