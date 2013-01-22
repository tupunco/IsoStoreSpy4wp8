using Microsoft.SmartDevice.Connectivity.Interface;

using System.IO;

namespace IsoStoreSpy.Plugins.Shared
{
    public class RemoteFileInfoManager
    {
        /// <summary>
        /// Nom du fichier
        /// </summary>

        public string Filename
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="fileInfo"></param>
        public RemoteFileInfoManager(IRemoteApplication application, IRemoteFileInfo fileInfo, string tempDirectory)
        {
            this.RemoteFileInfo = fileInfo;
            this.TempDirectory = tempDirectory;
            this.RemoteApplication = application;
        }

        /// <summary>
        /// Repertoire temporaire
        /// </summary>
        public string TempDirectory
        {
            get;
            private set;
        }

        /// <summary>
        /// RemoteFileInfo
        /// </summary>
        public IRemoteFileInfo RemoteFileInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// RemoteApplication
        /// </summary>
        public IRemoteApplication RemoteApplication
        {
            get;
            private set;
        }

        /// <summary>
        /// Chargement
        /// </summary>
        /// <returns></returns>

        public FileStream DonwloadFile()
        {
            string filename = Load();

            return File.OpenRead(filename);
        }

        /// <summary>
        /// Sauvegarde
        /// </summary>
        /// <param name="stream"></param>
        public void Save(string fileName)
        {
            IRemoteFileInfo directory = RemoteIsolatedStoreTools.GetParentDirectory(this.RemoteApplication, this.RemoteFileInfo);
            RemoteIsolatedStoreTools.UploadFileToDevice(this.RemoteApplication, directory, fileName);
        }

        public void Save()
        {
            this.Save(this.Filename);
        }

        /// <summary>
        /// Telechargement du fichier
        /// </summary>
        /// <returns></returns>
        public string Load()
        {
            RemoteIsolatedStoreTools.DownloadFileFromDevice(this.RemoteApplication, this.RemoteFileInfo, this.TempDirectory);

            string filename = Path.Combine(this.TempDirectory, RemoteIsolatedStoreTools.GetShortName(this.RemoteFileInfo));
            
            this.Filename = filename;

            return filename;
        }
    }
}
