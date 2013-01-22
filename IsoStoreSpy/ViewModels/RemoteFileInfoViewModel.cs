using IsoStoreSpy.Plugins.Shared;
using IsoStoreSpy.Tools;
using Microsoft.SmartDevice.Connectivity;
using Microsoft.SmartDevice.Connectivity.Interface;
using System.Collections.ObjectModel;
using System.IO;

namespace IsoStoreSpy.ViewModels
{
    public class RemoteFileInfoViewModel : BaseViewModel
    {
        /// <summary>
        /// IsoStoreViewModel
        /// </summary>

        public static IsoStoreSpyViewModel IsoStoreSpyViewModel
        {
            get;
            set;
        }

        /// <summary>
        /// Temporaire
        /// </summary>

        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="parent"></param>

        public RemoteFileInfoViewModel(RemoteFileInfoViewModel parent)
        {
            this.Parent = parent;
            this.SelectedFiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedFiles_CollectionChanged);
        }

        /// <summary>
        /// Changement dans la collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void SelectedFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.IsoStoreSpyViewModel.SelectedFiles = this.SelectedFiles;
        }

        /// <summary>
        /// Nom de la propriété RemoteFileInfo
        /// </summary>

        public const string RemoteFileInfoPropertyName = "RemoteFileInfo";

        /// <summary>
        /// propriété IRemoteFileInfo :  
        /// </summary>
        public IRemoteFileInfo RemoteFileInfo
        {
            get
            {
                return this._RemoteFileInfo;
            }

            set
            {
                if (this._RemoteFileInfo != value)
                {
                    this._RemoteFileInfo = value;
                    this.RaisePropertyChanged(RemoteFileInfoPropertyName);
                    this.RaisePropertyChanged(ImageSourcePropertyName);
                }
            }
        }

        private IRemoteFileInfo _RemoteFileInfo = null;


        /// <summary>
        /// Nom de la propriété Parent
        /// </summary>

        public const string ParentPropertyName = "Parent";

        /// <summary>
        /// propriété Parent :  
        /// </summary>

        public RemoteFileInfoViewModel Parent
        {
            get
            {
                return this._Parent;
            }

            set
            {
                if (this._Parent != value)
                {
                    this._Parent = value;
                    this.RaisePropertyChanged(ParentPropertyName);
                }
            }
        }

        private RemoteFileInfoViewModel _Parent = null;

        /// <summary>
        /// Telecharger
        /// </summary>
        /// <returns></returns>

        public FileStream Download()
        {
            return RemoteIsolatedStoreTools.LoadFile(IsoStoreSpyViewModel.ApplicationViewModel.SelectedRemoteApplication.Application, this.RemoteFileInfo, IsoStoreSpyViewModel.TempDirectory);
        }

        /// <summary>
        /// Est-ce un repertoire
        /// </summary>

        public bool IsDirectory
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    return this.RemoteFileInfo.IsDirectory();
                }

                return true;
            }
        }

        /// <summary>
        /// Nom court
        /// </summary>

        public string ShortName
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    return RemoteIsolatedStoreTools.GetShortName(this.RemoteFileInfo);
                }
                else
                {
                    return "IsoStore";
                }
            }
        }

        /// <summary>
        /// Nom de la propriété Size
        /// </summary>

        public const string SizePropertyName = "Size";

        /// <summary>
        /// propriété Size : 
        /// </summary>

        public string SizeString
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    return SizeConverter.FormatFileSize(this.RemoteFileInfo.Length);
                }

                return null;
            }

            // pour le Binding de TextBlock
            set
            {
            }
        }


        /// <summary>
        /// Nom de la propriété ModificationDateString
        /// </summary>

        public const string ModificationDateStringPropertyName = "ModificationDateString";

        /// <summary>
        /// propriété ModificationDateString :  
        /// </summary>

        public string ModificationDateString
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    //TODO ModificationDateString
                    //return DateConverter.FormatDate(this.RemoteFileInfo.LastWriteTime);
                }

                return null;
            }

            // pour le Binding de TextBlock
            set
            {
            }
        }

        /// <summary>
        /// Nom de la propriété AccessDateString
        /// </summary>

        public const string AccessDateStringPropertyName = "AccessDateString";

        /// <summary>
        /// propriété AccessDateString :  
        /// </summary>

        public string AccessDateString
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    //TODO AccessDateString
                    //return DateConverter.FormatDate(this.RemoteFileInfo.LastAccessTime);
                }

                return null;
            }

            // pour le Binding de TextBlock
            set
            {
            }
        }

        /// <summary>
        /// Nom de la propriété CreationDateString
        /// </summary>

        public const string CreationDateStringPropertyName = "CreationDateString";

        /// <summary>
        /// propriété CreationDateString :  
        /// </summary>

        public string CreationDateString
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    //TODO CreationDateString
                    //return DateConverter.FormatDate(this.RemoteFileInfo.CreationTime);
                }

                return null;
            }

            // pour le Binding de TextBlock
            set
            {
            }
        }

        /// <summary>
        /// Nom de la propriété ImageSource
        /// </summary>

        public const string ImageSourcePropertyName = " ImageSource";

        /// <summary>
        /// propriété  ImageSource :  
        /// </summary>

        public string ImageSource
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    if (this.IsDirectory == true)
                    {
                        return "Images\\Folder.png";
                    }
                    else
                    {
                        return "Images\\File.png";
                    }
                }
                else
                {
                    return "Images\\IsoStore.png";
                }
            }
        }


        /// <summary>
        /// Nom de la propriété BigImageSource
        /// </summary>

        public const string BigImageSourcePropertyName = " BigImageSource";

        /// <summary>
        /// propriété  BigImageSource :  
        /// </summary>

        public string BigImageSource
        {
            get
            {
                if (this.RemoteFileInfo != null)
                {
                    if (this.IsDirectory == true)
                    {
                        return "BigImages\\Folder.png";
                    }
                    else
                    {
                        string shortName = this.ShortName.Trim();
                        string extension = Path.GetExtension(shortName);

                        switch (extension)
                        {
                            case ".wav":
                            case ".mp3":
                                return "BigImages\\Sound.png";
                            case ".ico":
                            case ".bmp":
                            case ".gif":
                            case ".png":
                            case ".jpeg":
                            case ".jpg":
                                return "BigImages\\Image.png";
                            case ".txt":
                                return "BigImages\\Text.png";
                            case ".xml":
                                return "BigImages\\xml.png";
                        }

                        if (shortName == "__ApplicationSettings")
                        {
                            return "BigImages\\ApplicationSettings.png";
                        }

                        return "BigImages\\UnknownFile.png";
                    }
                }

                return null;
            }
        }


        /// <summary>
        /// Nom de la propriété Description
        /// </summary>

        public const string DescriptionPropertyName = "Description";

        /// <summary>
        /// propriété Description :  
        /// </summary>

        public string Description
        {
            get
            {

                if (this.RemoteFileInfo != null)
                {
                    if (this.IsDirectory == true)
                    {
                        return "Folder";
                    }
                    else
                    {
                        string shortName = this.ShortName.Trim();
                        string extension = Path.GetExtension(shortName);

                        switch (extension)
                        {
                            case ".wav":
                            case ".mp3":
                                return "Sound file";
                            case ".ico":
                            case ".bmp":
                            case ".gif":
                            case ".png":
                            case ".jpeg":
                            case ".jpg":
                                return "Image file";
                            case ".txt":
                                return "Text file";
                            case ".xml":
                                return "XML file";
                        }

                        if (shortName == "__ApplicationSettings")
                        {
                            return "Settings of application";
                        }

                        return "Unknown file";
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Nom de la propriété Children
        /// </summary>

        public const string ChildrenPropertyName = "Children";

        /// <summary>
        /// propriété Children :  
        /// </summary>

        public ObservableCollection<RemoteFileInfoViewModel> Children
        {
            get
            {
                return this._Children;
            }

            set
            {
                if (this._Children != value)
                {
                    this._Children = value;
                    this.RaisePropertyChanged(ChildrenPropertyName);
                }
            }
        }

        private ObservableCollection<RemoteFileInfoViewModel> _Children = new ObservableCollection<RemoteFileInfoViewModel>();


        /// <summary>
        /// Nom de la propriété FilesAndDirectories
        /// </summary>

        public const string FilesPropertyName = "Files";

        /// <summary>
        /// propriété Files :  
        /// </summary>

        public ObservableCollection<RemoteFileInfoViewModel> Files
        {
            get
            {
                return this._Files;
            }

            set
            {
                if (this._Files != value)
                {
                    this._Files = value;
                    this.RaisePropertyChanged(FilesPropertyName);
                }
            }
        }

        private ObservableCollection<RemoteFileInfoViewModel> _Files = new ObservableCollection<RemoteFileInfoViewModel>();

        /// <summary>
        /// Nom de la propriété SelectedFiles
        /// </summary>

        public const string SelectedFilesPropertyName = "SelectedFiles";

        /// <summary>
        /// propriété SelectedFiles :  
        /// </summary>

        public ObservableCollection<RemoteFileInfoViewModel> SelectedFiles
        {
            get
            {
                return this._SelectedFiles;
            }

            set
            {
                if (this._SelectedFiles != value)
                {
                    this._SelectedFiles = value;

                    this.RaisePropertyChanged(SelectedFilesPropertyName);

                    RemoteFileInfoViewModel.IsoStoreSpyViewModel.SelectedFiles = value;
                }
            }
        }

        private ObservableCollection<RemoteFileInfoViewModel> _SelectedFiles = new ObservableCollection<RemoteFileInfoViewModel>();

        /// <summary>
        /// Obtenir un chemin relatif à la racine
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public string GetSearchPattern(ApplicationViewModel application)
        {
            if (application != null && application.SelectedRemoteApplication != null)
            {
                return RemoteIsolatedStoreTools.GetSearchPattern(application.SelectedRemoteApplication.Application, this.RemoteFileInfo);
            }

            return null;
        }


        /// <summary>
        /// Contenu
        /// </summary>

        public const string ContentPropertyName = "Content";

        /// <summary>
        /// propriété Content : 
        /// </summary>

        public object Content
        {
            get
            {
                return this._Content;
            }

            set
            {
                if (this._Content != value)
                {
                    this._Content = value;
                    this.RaisePropertyChanged(ContentPropertyName);
                }
            }
        }

        private object _Content = null;

    }
}
