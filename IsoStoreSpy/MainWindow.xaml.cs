using IsoStoreSpy.Properties;
using IsoStoreSpy.ViewModels;

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Forms = System.Windows.Forms;

namespace IsoStoreSpy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings Settings
        {
            get
            {
                return IsoStoreSpy.Properties.Settings.Default;
            }
        }

        /// <summary>
        /// Constructeur
        /// </summary>

        public MainWindow()
        {
            InitializeComponent();

            this.IsoStoreSpyViewModel = App.IsoStoreSpyViewModel;
            this.DataContext = this;

            this.IsoStoreSpyViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(IsoStoreSpyViewModel_PropertyChanged);

            this.Folders.Opacity = 0;
            this.FilesAndPreview.Opacity = 0;
        }

        /// <summary>
        /// Changement de propriété
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void IsoStoreSpyViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case IsoStoreSpyViewModel.SelectedFilesPropertyName:

                    this.AnimateFileInformation();
                    break;

                case IsoStoreSpyViewModel.SelectedDirectoryPropertyName:

                    this.AnimateFileInformation();
                    break;
            }
        }

        /// <summary>
        /// Animation du panel d'information sur les fichiers selectionnés
        /// </summary>
        private void AnimateFileInformation()
        {
            bool newFile = false;

            if (this.IsoStoreSpyViewModel.SelectedDirectory != null)
            {
                if (this.IsoStoreSpyViewModel.SelectedDirectory.SelectedFiles != null && this.IsoStoreSpyViewModel.SelectedDirectory.SelectedFiles.Count > 0)
                {
                    newFile = true;
                    Storyboard storyboard = this.LayoutRoot.Resources["StoryboardOpenInformation"] as Storyboard;
                    storyboard.Begin();
                }
            }

            if (newFile == false)
            {
                Storyboard storyboard = this.LayoutRoot.Resources["StoryboardCloseInformation"] as Storyboard;
                storyboard.Begin();
            }
        }

        /// <summary>
        /// Animation de l'application
        /// </summary>

        private void AnimateApplication()
        {
            if (this.IsoStoreSpyViewModel.ApplicationViewModel.SelectedRemoteApplication == null)
            {
                Storyboard s = this.Resources["StoryboardClose"] as Storyboard;
                s.Begin();
            }
            else
            {
                Storyboard s = this.Resources["StoryboardOpen"] as Storyboard;
                s.Begin();
            }
        }

        /// <summary>
        /// DropBox
        /// </summary>

        public IsoStoreSpyViewModel IsoStoreSpyViewModel
        {
            get { return (IsoStoreSpyViewModel)GetValue(IsoStoreSpyViewModelProperty); }
            set { SetValue(IsoStoreSpyViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsoStoreSpyViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsoStoreSpyViewModelProperty =
            DependencyProperty.Register("IsoStoreSpyViewModel", typeof(IsoStoreSpyViewModel), typeof(MainWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Chargement de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ButtonApplicationsClick(object sender, RoutedEventArgs e)
        {
            WindowApplications windowApplications = new WindowApplications();
            windowApplications.Owner = this;
            if (windowApplications.ShowDialog() == true)
            {
                // Remplacement 
                App.IsoStoreSpyViewModel.ApplicationViewModel = windowApplications.ApplicationViewModel;
                this.AnimateApplication();
            }
        }

        /// <summary>
        /// Selection d'un repertoire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            App.IsoStoreSpyViewModel.SelectedDirectory = e.NewValue as RemoteFileInfoViewModel;
        }

        /// <summary>
        /// Rafraichir le repertoire selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RefreshDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.IsoStoreSpyViewModel.RefreshSelectedDirectory();
            }
            catch (Exception ex)
            {
                App.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Nouveau répertoire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void NewDirectory_Click(object sender, RoutedEventArgs e)
        {
            WindowFolderName window = new WindowFolderName();
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                RemoteFileInfoViewModel newDirectory = App.IsoStoreSpyViewModel.CreateDirectory(window.FolderName);

                if (newDirectory != null)
                {
                    TreeViewItem rootItem = this.TreeView.ItemContainerGenerator.ContainerFromItem(newDirectory.Parent) as TreeViewItem;

                    if (rootItem != null)
                    {
                        rootItem.IsExpanded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Destruction d'un repertoire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DeleteDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (App.ShowQuestion("Do you want to delete this directory ?") == MessageBoxResult.OK)
            {
                App.IsoStoreSpyViewModel.DeleteSelectedDirectory();
            }
        }

        /// <summary>
        /// Drop of file à partir de Windows Explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DropFileFromWindows(object sender, DragEventArgs e)
        {
            if (e.Data is System.Windows.DataObject && ((System.Windows.DataObject)e.Data).ContainsFileDropList())
            {
                this.StartLongOperation();

                try
                {
                    foreach (string fileOrDirectoryPath in ((System.Windows.DataObject)e.Data).GetFileDropList())
                    {
                        // Processing here    
                        IsoStoreSpyViewModel.UploadToSelectedDirectory(fileOrDirectoryPath);
                    }
                }
                finally
                {
                    this.StopLongOperation();
                }
            }
        }

        /// <summary>
        /// Download Directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DownloadDirectory_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog dialog = new Forms.FolderBrowserDialog();

            dialog.ShowNewFolderButton = true;
            dialog.Description = "Choose the desktop's folder where you want to download the selected folder of the phone.";
            dialog.SelectedPath = Settings.SelectedPath;

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                Settings.SelectedPath = dialog.SelectedPath;
                Settings.Save();

                this.StartLongOperation();

                new Thread(new ThreadStart(() =>
                {
                    App.IsoStoreSpyViewModel.DownloadFromSelectedDirectory(dialog.SelectedPath);
                    this.StopLongOperation();

                })).Start();

            }
        }

        /// <summary>
        /// UploadDirectory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void UploadDirectory_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog dialog = new Forms.FolderBrowserDialog();

            dialog.ShowNewFolderButton = true;
            dialog.Description = "Choose the desktop's folder to upload to the selected folder of the phone.";
            dialog.SelectedPath = Settings.SelectedPath;

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                Settings.SelectedPath = dialog.SelectedPath;
                Settings.Save();

                this.StartLongOperation();

                new Thread(new ThreadStart(() =>
                {
                    App.IsoStoreSpyViewModel.UploadToSelectedDirectory(dialog.SelectedPath, true);
                    this.StopLongOperation();

                })).Start();

            }
        }

        /// <summary>
        /// Upload 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void UploadFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Multiselect = true;
            dialog.InitialDirectory = Settings.SelectedPath;

            if (dialog.ShowDialog() == true)
            {
                Settings.SelectedPath = Path.GetDirectoryName(dialog.FileName);
                Settings.Save();

                this.StartLongOperation();

                new Thread(new ThreadStart(() =>
                {
                    App.IsoStoreSpyViewModel.UploadToSelectedFiles(dialog.FileNames);
                    this.StopLongOperation();

                })).Start();

            }
        }

        /// <summary>
        /// Destruction des fichiers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DeleteFiles_Click(object sender, RoutedEventArgs e)
        {
            if (App.ShowQuestion("Do you want to delete this file(s) ?") == MessageBoxResult.OK)
            {
                App.IsoStoreSpyViewModel.DeleteSelectedFiles();
            }
        }

        /// <summary>
        /// Téléchargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DownloadFiles_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog dialog = new Forms.FolderBrowserDialog();

            dialog.ShowNewFolderButton = true;
            dialog.Description = "Choose the desktop's folder where you want to download the selected files of the phone.";
            dialog.SelectedPath = Settings.SelectedPath;

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                Settings.SelectedPath = dialog.SelectedPath;
                Settings.Save();

                this.StartLongOperation();

                new Thread(new ThreadStart(() =>
                {
                    App.IsoStoreSpyViewModel.DownloadFromSelectedFiles(dialog.SelectedPath);
                    this.StopLongOperation();

                })).Start();
            }
        }

        /// <summary>
        /// Début de l'opération
        /// </summary>

        private void StartLongOperation()
        {
            this.IsEnabled = false;
        }

        /// <summary>
        /// Fin de l'opération
        /// </summary>

        private void StopLongOperation()
        {
            Dispatcher.Invoke(new Action(() =>
                    {
                        this.IsEnabled = true;
                    }
                )
            );
        }

        /// <summary>
        /// La souris bouge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Window_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (App.IsoStoreSpyViewModel.ApplicationViewModel == null)
            {
                this.AnimateEye(e, this.BigEye, this.ScaleBigEye);
            }

            this.AnimateEye(e, this.SmallEye, this.ScaleSmallEye);
        }

        /// <summary>
        /// Animation de l'oeil
        /// </summary>
        /// <param name="e"></param>
        /// <param name="eyeElement"></param>
        /// <param name="scale"></param>

        private void AnimateEye(System.Windows.Input.MouseEventArgs e, UIElement eyeElement, ScaleTransform scale)
        {
            Point p = e.GetPosition(eyeElement);

            if (p.X > 0)
            {
                scale.ScaleX = -1;
            }
            else
            {
                scale.ScaleX = 1;
            }
        }

        /// <summary>
        /// Ouvrir le dernier chemin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Explorer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", Settings.SelectedPath);
        }

        /// <summary>
        /// Ouvri le blog de Samuel Blanchard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BlogButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://blog.naviso.fr") { UseShellExecute = true });
        }

        /// <summary>
        /// Changement de selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ListBoxFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.IsoStoreSpyViewModel.SelectedFile != null)
            {
                App.IsoStoreSpyViewModel.SelectedFile.Content = null;
            }

            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            this.timer = new Timer((object s) =>
            {
                this.timer.Dispose();
                this.timer = null;

                RemoteFileInfoViewModel fileInfo = App.IsoStoreSpyViewModel.SelectedFile;

                if (fileInfo != null)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        App.IsoStoreSpyViewModel.SetContentFromSelectedFile();
                    }));
                }
            },
            null,
            1 * 1000,
            Timeout.Infinite
            );
        }

        Timer timer = null;
    }
}
