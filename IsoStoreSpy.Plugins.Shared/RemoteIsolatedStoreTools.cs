using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SmartDevice.Connectivity;
using System.IO;

namespace IsoStoreSpy.Tools
{
    /// <summary>
    /// Type de device
    /// </summary>

    public enum DeviceTypes
    {
        None = -1,
        Emulator,
        Device
    }

    public class RemoteIsolatedStoreTools
    {
        /// <summary>
        /// LangId
        /// </summary>

        public static int LangId
        {
            get
            {
                return langId;
            }

            set
            {
                langId = value;
            }
        }

        private static int langId = 0x409;


        /// <summary>
        /// Obtenir les devices
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="langId"></param>
        /// <returns></returns>

        public static Device GetDevice(DeviceTypes deviceType)
        {
            if (deviceType == DeviceTypes.None)
                return null;

            DatastoreManager manager = new DatastoreManager(langId);
            
            foreach (Platform platform in manager.GetPlatforms())
            {
                foreach (Device device in platform.GetDevices())
                {
                    if ((deviceType == DeviceTypes.Emulator && device.IsEmulator()) || (deviceType == DeviceTypes.Device && !device.IsEmulator()))
                    {
                        return device;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtenir la liste des applications
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>

        public static List<RemoteApplication> GetAllApplications(DeviceTypes deviceType)
        {
            Device device = GetDevice( deviceType);
                
            if( device != null )
            {
                device.Connect();

                return device.GetInstalledApplications().ToList();
            }
            
            return new List<RemoteApplication>();
        }

        /// <summary>
        /// Obtenir les informations 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="searchPattern"></param>
        /// <param name="isDirectoryPattern"></param>
        /// <returns></returns>

        public static List<RemoteFileInfo> GetRemoteFileInfos(RemoteApplication application, string searchPattern, bool? isDirectoryPattern)
        {
            List<RemoteFileInfo> result = new List<RemoteFileInfo>();

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                List<RemoteFileInfo> remoteFileInfos = new List<RemoteFileInfo>();

                try
                {
                    remoteFileInfos = isolatedStore.GetDirectoryListing(searchPattern);
                }
                catch (FileNotFoundException)
                {
                }
                catch (DirectoryNotFoundException)
                {
                }

                foreach (RemoteFileInfo remoteFileInfo in remoteFileInfos)
                {
                    if (isDirectoryPattern.HasValue == true)
                    {
                        if (remoteFileInfo.IsDirectory() != isDirectoryPattern)
                        {
                            continue;
                        }
                    }

                    result.Add(remoteFileInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Obtenir le nom court d'un fichier/repertoire
        /// </summary>
        /// <param name="remoteFileInfo"></param>
        /// <returns></returns>

        public static string GetShortName(RemoteFileInfo remoteFileInfo)
        {
            if (remoteFileInfo != null)
            {
                FileInfo fileinfo = new FileInfo(remoteFileInfo.Name);
                return fileinfo.Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// Obtenir le chemin de base
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>

        public static string GetRootPath(RemoteApplication application)
        {
            return string.Format(@"\Applications\Data\{0}\Data\IsolatedStore\", application.ProductID);
        }

        /// <summary>
        /// Obtenir le parent
        /// </summary>
        /// <param name="application"></param>
        /// <param name="remoteFileInfo"></param>
        /// <returns></returns>

        public static string GetParentDirectoryString(RemoteApplication application, RemoteFileInfo remoteFileInfo)
        {
            string filename = GetSearchPattern(application, remoteFileInfo);

            string name = GetShortName( remoteFileInfo );

            return filename.Substring(0, filename.Length - name.Length);
        }

        /// <summary>
        /// Obtenir le repertoire parent
        /// </summary>
        /// <param name="application"></param>
        /// <param name="remoteFileInfo"></param>
        /// <returns></returns>

        public static RemoteFileInfo GetParentDirectory(RemoteApplication application, RemoteFileInfo remoteFileInfo)
        {
            if (application == null || remoteFileInfo == null)
                return null;

            string parent = GetParentDirectoryString( application, remoteFileInfo );           
            
            string parentName = new DirectoryInfo(parent).Name;
            
            string parentOfParent = Path.GetPathRoot( parent );

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                List<RemoteFileInfo> remoteFileInfos = isolatedStore.GetDirectoryListing(parentOfParent);

                foreach (RemoteFileInfo r in remoteFileInfos)
                {
                    string name = RemoteIsolatedStoreTools.GetShortName( r );

                    if (r.IsDirectory() && name == parentName )
                    {
                        return r;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Obtenir la search pattern à partir des informations d'un fichier
        /// </summary>
        /// <param name="application"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        public static string GetSearchPattern(RemoteApplication application, RemoteFileInfo fileInfo)
        {
            if (fileInfo == null)
                return string.Empty;

            string root = GetRootPath(application).ToLower();
            
            if (fileInfo.Name.ToLower().StartsWith(root) == true)
            {
                return fileInfo.Name.Substring(root.Length);
            }

            return null;
        }

        /// <summary>
        /// Obtenir l'isolatedStorage
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="productId"></param>
        /// <param name="langId"></param>
        /// <param name="debugOn"></param>
        /// <returns></returns>

        public static RemoteApplication GetApplication(DeviceTypes deviceType, Guid productId)
        {
            Device device = GetDevice(deviceType);

            if( device != null )
            {
                    device.Connect();

                    return device.GetApplication(productId);
            }
            
            return null;
        }

        /// <summary>
        /// Recherche d'un remoteFileInfo par son nom
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directoryParent"></param>
        /// <param name="nameOfFileOrDirectory"></param>
        /// <returns></returns>

        public static RemoteFileInfo SearchRemoteFileInfo( RemoteApplication application, RemoteFileInfo directoryParent, string nameOfFileOrDirectory )
        {
            if (application == null || string.IsNullOrWhiteSpace( nameOfFileOrDirectory ) == true )
            {
                return null;
            }

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string searchPattern = GetSearchPattern(application, directoryParent);

                List<RemoteFileInfo> result = GetRemoteFileInfos(application, searchPattern, null );
            
                return result.FirstOrDefault( r => GetShortName( r ) == nameOfFileOrDirectory );
            }

            return null;
        }

        /// <summary>
        /// le repertoire de nom Exist ou non
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directoryParent"></param>
        /// <param name="directoryOrFilename"></param>
        /// <returns></returns>

        public static bool Exists(RemoteApplication application, RemoteFileInfo directoryParent, string directoryOrFilename )
        {
            if (application == null || string.IsNullOrWhiteSpace(directoryOrFilename) == true)
            {
                return false;
            }

            bool result = false;

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string searchPattern = GetSearchPattern(application, directoryParent);
                string path = Path.Combine( searchPattern, directoryOrFilename );
                result = isolatedStore.DirectoryExists(path);

                if (result == false)
                {
                    result = isolatedStore.FileExists(path);
                }
            }

            return result;
        }

        /// <summary>
        /// Creation du répertoire
        /// </summary>
        /// <param name="fileInfo"></param>

        public static RemoteFileInfo CreateDirectory(RemoteApplication application, RemoteFileInfo directoryParent, string newDirectoryName)
        {
            if (application == null || string.IsNullOrWhiteSpace( newDirectoryName ) == true )
            {
                return null;
            }

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string path = Path.Combine(GetSearchPattern(application, directoryParent), newDirectoryName);

                isolatedStore.CreateDirectory(path);

                return SearchRemoteFileInfo( application, directoryParent, newDirectoryName );
            }

            return null;
        }

        /// <summary>
        /// Destruction 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directory"></param>

        public static void DeleteDirectoryOrFile(RemoteApplication application, RemoteFileInfo directoryOrFile)
        {
            GetParentDirectory(application, directoryOrFile); 


            if (application == null || directoryOrFile == null)
            {
                return;
            }

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string path = GetSearchPattern(application, directoryOrFile);

                if (directoryOrFile.IsDirectory() == true)
                {
                    isolatedStore.DeleteDirectory(path);
                }
                else
                {
                    isolatedStore.DeleteFile(path);
                }
            }
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="application"></param>
        /// <param name="fileToDownload"></param>
        /// <param name="desktopPath"></param>

        public static void DownloadFileFromDevice(RemoteApplication application, RemoteFileInfo fileToDownload, string desktopPath)
        {
            if (application == null || fileToDownload == null || desktopPath == null )
            {
                return;
            }

            if (fileToDownload.IsDirectory() == false)
            {
                RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

                if (isolatedStore != null)
                {
                    // on rajoute le nom du fichier
                    desktopPath = Path.Combine(desktopPath, GetShortName(fileToDownload));

                    string searchPattern = GetSearchPattern(application, fileToDownload);

                    isolatedStore.ReceiveFile(searchPattern, desktopPath, true);
                }
            }
        }

        /// <summary>
        /// Upload du fichier
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directoryToUpload"></param>
        /// <param name="desktopfilename"></param>

        public static void UploadFileToDevice(RemoteApplication application, RemoteFileInfo directoryToUpload, string desktopfilename)
        {
            if (application == null || desktopfilename == null)
            {
                return;
            }

            if (directoryToUpload == null || directoryToUpload.IsDirectory() == true )
            {
                RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

                if (isolatedStore != null)
                {
                    string searchPattern = GetSearchPattern(application, directoryToUpload);

                    // on rajoute le nom du fichier
                    string devicefilename = Path.Combine(searchPattern, new FileInfo(desktopfilename).Name );

                    isolatedStore.SendFile(desktopfilename, devicefilename, true);
                }
            }
        }

        /// <summary>
        /// Telechargement 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directoryOrFileToDownload"></param>
        /// <param name="desktopPath"></param>

        public static void DownloadFileOrDirectoryFromDevice(RemoteApplication application, RemoteFileInfo directoryOrFileToDownload, string desktopPath, bool includeRootDirectory = true)
        {
            if (application == null || desktopPath == null)
            {
                return;
            }

            RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                if (directoryOrFileToDownload == null || directoryOrFileToDownload.IsDirectory() == true)
                {
                    string searchPattern = GetSearchPattern(application, directoryOrFileToDownload);
                    List<RemoteFileInfo> fileInfos = GetRemoteFileInfos(application, searchPattern, null);

                    desktopPath = Path.Combine(desktopPath, GetShortName(directoryOrFileToDownload));

                    if (includeRootDirectory == true)
                    {
                        Directory.CreateDirectory(desktopPath);
                    }

                    foreach (RemoteFileInfo fileInfo in fileInfos)
                    {
                        if (fileInfo.IsDirectory() == true)
                        {
                            if (includeRootDirectory == false)
                            {
                                Directory.CreateDirectory(desktopPath);
                            }

                            // repertoire
                            DownloadFileOrDirectoryFromDevice(application, fileInfo, desktopPath, true);
                        }
                        else
                        {   // fichier
                            DownloadFileFromDevice(application, fileInfo, desktopPath);
                        }
                    }
                }
                else
                {
                    // fichier
                    DownloadFileFromDevice(application, directoryOrFileToDownload, desktopPath);
                }
            }
        }

        /// <summary>
        /// Upload d'un fichier ou d'un répertoire entier
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directoryOrFileToDownload"></param>
        /// <param name="desktopPath"></param>

        public static RemoteFileInfo UploadFileOrDirectoryToDevice(RemoteApplication application, RemoteFileInfo directoryToUpload, string desktopPathOrFilename, bool includeRootDirectory = false)
        {
            if (application == null || desktopPathOrFilename == null)
            {
                return null;
            }

            if (directoryToUpload == null || directoryToUpload.IsDirectory() == true)
            {
                RemoteIsolatedStorageFile isolatedStore = application.GetIsolatedStore();

                if (isolatedStore != null)
                {
                    bool isDirectory = Directory.Exists(desktopPathOrFilename);

                    string searchPattern = GetSearchPattern(application, directoryToUpload);

                    if (isDirectory == true)
                    {
                        string directoryName = new DirectoryInfo(desktopPathOrFilename).Name;

                        RemoteFileInfo newDirectory = directoryToUpload;

                        if (includeRootDirectory == true)
                        {
                            newDirectory = CreateDirectory(application, directoryToUpload, directoryName);
                        }

                        foreach (string directory in Directory.GetDirectories(desktopPathOrFilename))
                        {
                            string desktopDirectoryPath = Path.Combine(desktopPathOrFilename, directory);
                            UploadFileOrDirectoryToDevice(application, newDirectory, desktopDirectoryPath, true);
                        }

                        foreach (string file in Directory.GetFiles(desktopPathOrFilename))
                        {
                            string desktopFilename = Path.Combine(desktopPathOrFilename, file);
                            UploadFileToDevice(application, newDirectory, desktopFilename);
                        }

                        return newDirectory;
                    }
                    else
                    {
                        // fichier
                        UploadFileToDevice(application, directoryToUpload, desktopPathOrFilename);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Chargement d'un fichier de nom FileInfo
        /// </summary>
        /// <returns></returns>

        public static FileStream LoadFile(RemoteApplication application, RemoteFileInfo fileInfo, string desktopTempDirectory)
        {
            RemoteIsolatedStoreTools.DownloadFileFromDevice(application, fileInfo, desktopTempDirectory);

            string filename = Path.Combine(desktopTempDirectory, RemoteIsolatedStoreTools.GetShortName(fileInfo));

            return File.OpenRead(filename);
        }

        /// <summary>
        /// Sauvegarde d'un fichier
        /// </summary>
        /// <param name="desktopStream"></param>

        public static void SaveFile(RemoteApplication application, RemoteFileInfo directory, FileStream desktopStream)
        {
            RemoteIsolatedStoreTools.UploadFileToDevice(application, directory, desktopStream.Name);
        }

        /// <summary>
        /// Chargement d'un fichier texte
        /// </summary>
        /// <param name="application"></param>
        /// <param name="fileInfo"></param>
        /// <param name="tempDirectory"></param>
        /// <returns></returns>

        public static string LoadFileText(RemoteApplication application, RemoteFileInfo fileInfo, string tempDirectory)
        {
            using (FileStream stream = LoadFile(application, fileInfo, tempDirectory))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Sauvegarde d'un fichier texte dans un repertoire temporaire puis envoi vers le device
        /// </summary>
        /// <param name="stream"></param>

        public static void SaveFileText(RemoteApplication application, RemoteFileInfo directory, string tempFullFilename, string text)
        {
            using ( FileStream stream = new FileStream( tempFullFilename, FileMode.Create ) )
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(text);

                    SaveFile(application, directory, stream);
                }
            }
        }
    }
}
