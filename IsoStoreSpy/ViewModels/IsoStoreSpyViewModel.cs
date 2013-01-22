using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.SmartDevice.Connectivity;
using Microsoft.Win32;
using IsoStoreSpy.Tools;
using System.IO;
using System.Collections;
using IsoStoreSpy.Plugins.Shared;
using Microsoft.SmartDevice.Connectivity.Interface;

namespace IsoStoreSpy.ViewModels
{
    public class IsoStoreSpyViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructeur
        /// </summary>

        public IsoStoreSpyViewModel()
        {
            RemoteFileInfoViewModel.IsoStoreSpyViewModel = this;
        }

        /// <summary>
        /// Fixer le plugin dans content
        /// </summary>
        /// <param name="?"></param>

        public void SetContentFromSelectedFile()
        {
            RemoteFileInfoViewModel fileInfo = this.SelectedFile;

            if (fileInfo != null)
            {
                // moins de 1 mega

                if (fileInfo.RemoteFileInfo.Length < 1024 * 1024)
                {
                    foreach (IPreviewPlugin plugin in this.Plugins)
                    {
                        if (plugin.CheckFileInfoIsSupported(fileInfo.RemoteFileInfo) == true)
                        {
                            if (this.SelectedFile != null)
                            {
                                RemoteFileInfoManager manager = new RemoteFileInfoManager( 
                                    this.ApplicationViewModel.SelectedRemoteApplication.Application,
                                    fileInfo.RemoteFileInfo,
                                    Path.GetTempPath()
                                    );

                                try
                                {
                                    this.SelectedFile.Content = plugin.Initialize(manager);
                                }
                                catch(Exception ex)
                                {
                                    App.ShowError(ex.Message);
                                    this.SelectedFile.Content = null;
                                    return;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Nom de la propriété Plugins
        /// </summary>

        public const string PluginsPropertyName = "Plugins";

        /// <summary>
        /// propriété Plugins :  
        /// </summary>

        public ObservableCollection<IPreviewPlugin> Plugins
        {
            get
            {
                return this._Plugins;
            }

            set
            {
                if (this._Plugins != value)
                {
                    this._Plugins = value;
                    this.RaisePropertyChanged(PluginsPropertyName);
                }
            }
        }

        private ObservableCollection<IPreviewPlugin> _Plugins = new ObservableCollection<IPreviewPlugin>();
        
        /// <summary>
        /// Nom de la propriété ApplicationViewModel
        /// </summary>

        public const string ApplicationViewModelPropertyName = "ApplicationViewModel";

        /// <summary>
        /// propriété ApplicationViewModel :  
        /// </summary>

        public ApplicationViewModel ApplicationViewModel
        {
            get
            {
                return this._ApplicationViewModel;
            }

            set
            {
                if (this._ApplicationViewModel != value)
                {
                    this._ApplicationViewModel = value;
                    this.RaisePropertyChanged(ApplicationViewModelPropertyName);
                
                    // on change d'application dont on veut obtenir les dossiers
                    this.Directories = new ObservableCollection<RemoteFileInfoViewModel>()
                    { 
                        // isoStore (racine)
                        new RemoteFileInfoViewModel(null) 
                    };
                    // On fixe le dossier selectionné
                    this.SelectedDirectory = this.Directories[0];
                    // Creation de l'arborescence
                    this.RefreshSelectedDirectory();
                    
                }
            }
        }

        private ApplicationViewModel _ApplicationViewModel = null;

        /// <summary>
        /// RefreshDirectory
        /// </summary>

        public void RefreshSelectedDirectory()
        {
            this.RefreshDirectory(this.SelectedDirectory);
        }

        /// <summary>
        /// Rafraichir le repertoire
        /// </summary>
        /// <param name="searchPattern"></param>

        private void RefreshDirectory(RemoteFileInfoViewModel directory )
        {
            if (directory == null || directory.IsDirectory == false)
            {
                return;
            }

            ObservableCollection<RemoteFileInfoViewModel> fileInfos = this.GetRemoteFileInfo(directory, false);
            ObservableCollection<RemoteFileInfoViewModel> directories = this.GetRemoteFileInfo(directory, true);

            directory.Children = directories;
            directory.Files = fileInfos;


            if (directory.Children != null && directory.Children.Count > 0)
            {
                foreach (RemoteFileInfoViewModel remoteFileInfo in directory.Children)
                {
                    this.RefreshDirectory(remoteFileInfo);
                }
            }
        }

        /// <summary>
        /// Repertoire
        /// </summary>

        public static string TempDirectory
        {
            get
            {
                if (tempDirectory == null)
                {
                    tempDirectory = Path.GetTempPath();
                }

                return tempDirectory;
            }
        }

        private static string tempDirectory = null;

        /// <summary>
        /// Nom de la propriété Directories
        /// </summary>

        public const string DirectoriesPropertyName = "Directories";

        /// <summary>
        /// propriété Directories :  
        /// </summary>

        public ObservableCollection<RemoteFileInfoViewModel> Directories
        {
            get
            {
                return this._Directories;
            }

            set
            {
                if (this._Directories != value)
                {
                    this._Directories = value;
                    this.RaisePropertyChanged(DirectoriesPropertyName);
                }
            }
        }

        private ObservableCollection<RemoteFileInfoViewModel> _Directories = null;
        
        /// <summary>
        /// Nom de la propriété SelectedDirectory
        /// </summary>

        public const string SelectedDirectoryPropertyName = "SelectedDirectory";

        /// <summary>
        /// propriété SelectedDirectory :  
        /// </summary>

        public RemoteFileInfoViewModel SelectedDirectory
        {
            get
            {
                return this._SelectedDirectory;
            }

            set
            {
                if (this._SelectedDirectory != value)
                {
                    this._SelectedDirectory = value;
                    this.RaisePropertyChanged(SelectedDirectoryPropertyName);
                    this.RaisePropertyChanged(HaveSelectedDirectoryPropertyName);
                }
            }
        }

        private RemoteFileInfoViewModel _SelectedDirectory = null;

        
        /// <summary>
        /// Nom de la propriété HaveSelectedDirectory
        /// </summary>

        public const string HaveSelectedDirectoryPropertyName = "HaveSelectedDirectory";

        /// <summary>
        /// propriété HaveSelectedDirectory :  
        /// </summary>

        public bool HaveSelectedDirectory
        {
            get
            {
                return this.SelectedDirectory != null;
            }
        }

        /// <summary>
        /// Nom de la propriété SelectedFiles
        /// </summary>

        public const string SelectedFilesPropertyName = "SelectedFiles";

        /// <summary>
        /// propriété SelectedFiles : SelectedFiles = SelectedDirectory.SelectedFiles
        /// </summary>

        public ObservableCollection<RemoteFileInfoViewModel> SelectedFiles
        {
            get
            {
                return this._SelectedFiles;
            }

            set
            {
                //if (this._SelectedFiles != value)
                //{
                    this._SelectedFiles = value;
                    this.RaisePropertyChanged(SelectedFilesPropertyName);
                    this.RaisePropertyChanged(HaveSelectedFilePropertyName);
                    this.RaisePropertyChanged(SelectedFilePropertyName);
                //}
            }
        }

        private ObservableCollection<RemoteFileInfoViewModel> _SelectedFiles = null;

        /// <summary>
        /// Nom de la propriété SelectedFiles
        /// </summary>

        public const string SelectedFilePropertyName = "SelectedFile";

        /// <summary>
        /// Premier fichier selectionné
        /// </summary>

        public RemoteFileInfoViewModel SelectedFile
        {
            get
            {
                if (this.SelectedFiles != null && this.SelectedFiles.Count > 0)
                {
                    return this.SelectedFiles[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Nom de la propriété HaveSelectedFile
        /// </summary>

        public const string HaveSelectedFilePropertyName = "HaveSelectedFile";

        /// <summary>
        /// propriété HaveSelectedFile :  
        /// </summary>

        public bool HaveSelectedFile
        {
            get
            {
                return this.SelectedFiles != null && this.SelectedFiles.Count > 0;
            }
        }

        /// <summary>
        /// Selected IsolatedStore
        /// </summary>

        private IRemoteIsolatedStorageFile SelectedIsolatedStore
        {
            get
            {
                if (this.ApplicationViewModel != null && this.ApplicationViewModel.SelectedRemoteApplication != null)
                {
                    return this.ApplicationViewModel.SelectedRemoteApplication.Application.GetIsolatedStore();
                }

                return null;
            }
        }

        /// <summary>
        /// Obtenir les fichier,repertoires seulement ou les deux (isdirectory = null)
        /// </summary>
        /// <returns></returns>

        private ObservableCollection<RemoteFileInfoViewModel> GetRemoteFileInfo(RemoteFileInfoViewModel parent, bool? isDirectoryPattern)
        {
            ObservableCollection<RemoteFileInfoViewModel> result = new ObservableCollection<RemoteFileInfoViewModel>();

            if (this.ApplicationViewModel != null && this.ApplicationViewModel.SelectedRemoteApplication != null)
            {
                string searchPattern = parent.GetSearchPattern(this.ApplicationViewModel);

                var remoteFileInfos = RemoteIsolatedStoreTools.GetRemoteFileInfos(this.ApplicationViewModel.SelectedRemoteApplication.Application, searchPattern, isDirectoryPattern);

                foreach (var remoteFileInfo in remoteFileInfos)
                {
                    result.Add(new RemoteFileInfoViewModel(parent)
                        {
                            Id = Guid.NewGuid().ToString(),
                            RemoteFileInfo = remoteFileInfo
                        }
                    );
                }
            }

            return result;
        }

        /// <summary>
        /// Création d'un répertoire
        /// </summary>

        public RemoteFileInfoViewModel CreateDirectory(string newDirectoryName)
        {
            if (string.IsNullOrWhiteSpace(newDirectoryName) == true)
            {
                return null;
            }

            if (this.ApplicationViewModel != null && this.ApplicationViewModel.SelectedRemoteApplication != null)
            {
                var application = this.ApplicationViewModel.SelectedRemoteApplication.Application;
                var directoryParent = this.SelectedDirectory.RemoteFileInfo;

                if (RemoteIsolatedStoreTools.Exists(application, directoryParent, newDirectoryName) == false)
                {
                    var newDirectory = RemoteIsolatedStoreTools.CreateDirectory(application, directoryParent, newDirectoryName);
                    var newDirectoryViewModel = new RemoteFileInfoViewModel(this.SelectedDirectory) { RemoteFileInfo = newDirectory };
                    
                    this.SelectedDirectory.Children.Add( newDirectoryViewModel );
                
                    return newDirectoryViewModel;
                }
            }

            return null;
        }

        /// <summary>
        /// Destruction du repertoire en cours
        /// </summary>

        public void DeleteSelectedDirectory()
        {
            if (this.ApplicationViewModel != null && this.ApplicationViewModel.SelectedRemoteApplication != null)
            {
                RemoteIsolatedStoreTools.DeleteDirectoryOrFile(this.ApplicationViewModel.SelectedRemoteApplication.Application, this.SelectedDirectory.RemoteFileInfo);

                RemoteFileInfoViewModel selectedDirectory = this.SelectedDirectory;
                RemoteFileInfoViewModel parent = selectedDirectory.Parent;

                parent.Children.Remove(selectedDirectory);

                this.SelectedDirectory = parent;
            }
        }

        public void DeleteSelectedFiles()
        {
            if (this.ApplicationViewModel != null && this.ApplicationViewModel.SelectedRemoteApplication != null)
            {
                List<RemoteFileInfoViewModel> selectedFiles = this.SelectedFiles.ToList();
                
                if (selectedFiles != null)
                {
                    foreach (RemoteFileInfoViewModel selectedFileInfo in selectedFiles)
                    {
                        var file = selectedFileInfo.RemoteFileInfo;

                        RemoteIsolatedStoreTools.DeleteDirectoryOrFile(this.ApplicationViewModel.SelectedRemoteApplication.Application, file);

                        this.SelectedDirectory.Files.Remove(selectedFileInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Telechargement d'un fichier ou d'un repertoire
        /// </summary>

        public void DownloadFromSelectedDirectory(string path)
        {
            RemoteIsolatedStoreTools.DownloadFileOrDirectoryFromDevice( 
                this.ApplicationViewModel.SelectedRemoteApplication.Application, 
                this.SelectedDirectory.RemoteFileInfo,
                path);
        }

        /// <summary>
        /// Download des fichiers selectionnées
        /// </summary>
        /// <param name="path"></param>

        public void DownloadFromSelectedFiles(string path)
        {
            if (this.SelectedFiles != null)
            {
                foreach (RemoteFileInfoViewModel fileInfo in this.SelectedFiles)
                {
                    RemoteIsolatedStoreTools.DownloadFileOrDirectoryFromDevice(
                        this.ApplicationViewModel.SelectedRemoteApplication.Application,
                        fileInfo.RemoteFileInfo,
                        path);
                }
            }
        }

        /// <summary>
        /// Upload d'un repertoire
        /// </summary>

        public void UploadToSelectedDirectory(string desktopFileOrDirectory, bool includeRootDirectory = true )
        {
            RemoteIsolatedStoreTools.UploadFileOrDirectoryToDevice(
                this.ApplicationViewModel.SelectedRemoteApplication.Application,
                this.SelectedDirectory.RemoteFileInfo,
                desktopFileOrDirectory, includeRootDirectory);

            this.RefreshSelectedDirectory();
                        
        }

        /// <summary>
        /// Upload d'une liste de fichier
        /// </summary>
        /// <param name="desktopFilenames"></param>

        public void UploadToSelectedFiles(string[] desktopFilenames)
        {
            foreach (string desktopFilename in desktopFilenames)
            {
                RemoteIsolatedStoreTools.UploadFileOrDirectoryToDevice(
                    this.ApplicationViewModel.SelectedRemoteApplication.Application,
                    this.SelectedDirectory.RemoteFileInfo,
                    desktopFilename, false);
            }

            this.RefreshSelectedDirectory();
        }
    }
}
