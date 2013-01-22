using Microsoft.SmartDevice.Connectivity.Interface;
using Microsoft.SmartDevice.MultiTargeting.Connectivity;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace IsoStoreSpy.Plugins.Shared
{
    /// <summary>
    /// Type de device
    /// </summary>
    public enum DeviceTypes
    {
        None = -1,
        /// <summary>
        /// WP8虚拟机
        /// </summary>
        Emulator,
        ///// <summary>
        ///// 遗留虚拟机
        ///// </summary>
        //LegacyEmulator,
        /// <summary>
        /// 设备
        /// </summary>
        Device
    }

    public class RemoteIsolatedStoreTools
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localeId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static ConnectableDevice GetDevice(int localeId, string deviceId)
        {
            var multiTargetingConnectivity = new MultiTargetingConnectivity(localeId);
            return multiTargetingConnectivity.GetConnectableDevice(deviceId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localeId"></param>
        /// <param name="bNoLegacyDevices"></param>
        /// <returns></returns>
        public static System.Collections.ObjectModel.Collection<ConnectableDevice> GetDevices(int localeId, bool bNoLegacyDevices = false)
        {
            var multiTargetingConnectivity = new MultiTargetingConnectivity(localeId);
            return multiTargetingConnectivity.GetConnectableDevices(bNoLegacyDevices);
        }
        /// <summary>
        /// Obtenir les devices
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private static ConnectableDevice GetDevice(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                return null;

            return GetDevice(CultureInfo.CurrentUICulture.LCID, deviceId);
        }

        /// <summary>
        /// Obtenir la liste des applications
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static List<IRemoteApplication> GetAllApplications(string deviceId)
        {
            var device = GetDevice(deviceId);

            if (device != null)
            {
                var idevice = device.Connect();
                return idevice.GetInstalledApplications().ToList();
            }

            return new List<IRemoteApplication>();
        }

        /// <summary>
        /// Obtenir les informations 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="searchPattern"></param>
        /// <param name="isDirectoryPattern"></param>
        /// <returns></returns>
        public static List<IRemoteFileInfo> GetRemoteFileInfos(IRemoteApplication application, string searchPattern, bool? isDirectoryPattern)
        {
            var result = new List<IRemoteFileInfo>();

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                var remoteFileInfos = new List<IRemoteFileInfo>();

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

                foreach (var remoteFileInfo in remoteFileInfos)
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
        public static string GetShortName(IRemoteFileInfo remoteFileInfo)
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
        public static string GetRootPath(IRemoteApplication application)
        {
            return string.Format(@"%FOLDERID_APPID_ISOROOT%\{0}\", application.ProductID.ToString("B"));
            //WP8 return string.Format(@"%FOLDERID_APPID_ISOROOT%\{{{0}}}\", application.ProductID);
            //WP7 return string.Format(@"\Applications\Data\{0}\Data\IsolatedStore\", application.ProductID);
        }

        /// <summary>
        /// Obtenir le parent
        /// </summary>
        /// <param name="application"></param>
        /// <param name="remoteFileInfo"></param>
        /// <returns></returns>
        public static string GetParentDirectoryString(IRemoteApplication application, IRemoteFileInfo remoteFileInfo)
        {
            string filename = GetSearchPattern(application, remoteFileInfo);

            string name = GetShortName(remoteFileInfo);

            return filename.Substring(0, filename.Length - name.Length);
        }

        /// <summary>
        /// Obtenir le repertoire parent
        /// </summary>
        /// <param name="application"></param>
        /// <param name="remoteFileInfo"></param>
        /// <returns></returns>
        public static IRemoteFileInfo GetParentDirectory(IRemoteApplication application, IRemoteFileInfo remoteFileInfo)
        {
            if (application == null || remoteFileInfo == null)
                return null;

            string parent = GetParentDirectoryString(application, remoteFileInfo);

            string parentName = new DirectoryInfo(parent).Name;

            string parentOfParent = Path.GetPathRoot(parent);

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                var remoteFileInfos = isolatedStore.GetDirectoryListing(parentOfParent);

                foreach (var r in remoteFileInfos)
                {
                    string name = RemoteIsolatedStoreTools.GetShortName(r);

                    if (r.IsDirectory() && name == parentName)
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
        public static string GetSearchPattern(IRemoteApplication application, IRemoteFileInfo fileInfo)
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
        public static IRemoteApplication GetApplication(string deviceId, Guid productId)
        {
            var device = GetDevice(deviceId);

            if (device != null)
            {
                var idevice = device.Connect();
                return idevice.GetApplication(productId);
            }

            return null;
        }

        /// <summary>
        /// Recherche d'un IRemoteFileInfo par son nom
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directoryParent"></param>
        /// <param name="nameOfFileOrDirectory"></param>
        /// <returns></returns>
        public static IRemoteFileInfo SearchRemoteFileInfo(IRemoteApplication application, IRemoteFileInfo directoryParent, string nameOfFileOrDirectory)
        {
            if (application == null || string.IsNullOrWhiteSpace(nameOfFileOrDirectory) == true)
            {
                return null;
            }

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string searchPattern = GetSearchPattern(application, directoryParent);

                var result = GetRemoteFileInfos(application, searchPattern, null);

                return result.FirstOrDefault(r => GetShortName(r) == nameOfFileOrDirectory);
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
        public static bool Exists(IRemoteApplication application, IRemoteFileInfo directoryParent, string directoryOrFilename)
        {
            if (application == null || string.IsNullOrWhiteSpace(directoryOrFilename) == true)
            {
                return false;
            }

            bool result = false;

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string searchPattern = GetSearchPattern(application, directoryParent);
                string path = Path.Combine(searchPattern, directoryOrFilename);
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
        public static IRemoteFileInfo CreateDirectory(IRemoteApplication application, IRemoteFileInfo directoryParent, string newDirectoryName)
        {
            if (application == null || string.IsNullOrWhiteSpace(newDirectoryName) == true)
            {
                return null;
            }

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string path = Path.Combine(GetSearchPattern(application, directoryParent), newDirectoryName);

                isolatedStore.CreateDirectory(path);

                return SearchRemoteFileInfo(application, directoryParent, newDirectoryName);
            }

            return null;
        }

        /// <summary>
        /// Destruction 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="directory"></param>
        public static void DeleteDirectoryOrFile(IRemoteApplication application, IRemoteFileInfo directoryOrFile)
        {
            GetParentDirectory(application, directoryOrFile);

            if (application == null || directoryOrFile == null)
            {
                return;
            }

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                string path = GetSearchPattern(application, directoryOrFile);

                if (directoryOrFile.IsDirectory() == true)
                {
                    isolatedStore.DeleteDirectory(path);
                }
                else
                {
                    //TODO isolatedStore.DeleteFile(path);
                }
            }
        }

        /// <summary>
        /// Download a file
        /// </summary>
        /// <param name="application"></param>
        /// <param name="fileToDownload"></param>
        /// <param name="desktopPath"></param>
        public static void DownloadFileFromDevice(IRemoteApplication application, IRemoteFileInfo fileToDownload, string desktopPath)
        {
            if (application == null || fileToDownload == null || desktopPath == null)
            {
                return;
            }

            if (fileToDownload.IsDirectory() == false)
            {
                var isolatedStore = application.GetIsolatedStore();

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
        public static void UploadFileToDevice(IRemoteApplication application, IRemoteFileInfo directoryToUpload, string desktopfilename)
        {
            if (application == null || desktopfilename == null)
            {
                return;
            }

            if (directoryToUpload == null || directoryToUpload.IsDirectory() == true)
            {
                var isolatedStore = application.GetIsolatedStore();

                if (isolatedStore != null)
                {
                    string searchPattern = GetSearchPattern(application, directoryToUpload);

                    // on rajoute le nom du fichier
                    string devicefilename = Path.Combine(searchPattern, new FileInfo(desktopfilename).Name);

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

        public static void DownloadFileOrDirectoryFromDevice(IRemoteApplication application, IRemoteFileInfo directoryOrFileToDownload, string desktopPath, bool includeRootDirectory = true)
        {
            if (application == null || desktopPath == null)
            {
                return;
            }

            var isolatedStore = application.GetIsolatedStore();

            if (isolatedStore != null)
            {
                if (directoryOrFileToDownload == null || directoryOrFileToDownload.IsDirectory() == true)
                {
                    string searchPattern = GetSearchPattern(application, directoryOrFileToDownload);
                    List<IRemoteFileInfo> fileInfos = GetRemoteFileInfos(application, searchPattern, null);

                    desktopPath = Path.Combine(desktopPath, GetShortName(directoryOrFileToDownload));

                    if (includeRootDirectory == true)
                    {
                        Directory.CreateDirectory(desktopPath);
                    }

                    foreach (IRemoteFileInfo fileInfo in fileInfos)
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

        public static IRemoteFileInfo UploadFileOrDirectoryToDevice(IRemoteApplication application, IRemoteFileInfo directoryToUpload, string desktopPathOrFilename, bool includeRootDirectory = false)
        {
            if (application == null || desktopPathOrFilename == null)
            {
                return null;
            }

            if (directoryToUpload == null || directoryToUpload.IsDirectory() == true)
            {
                var isolatedStore = application.GetIsolatedStore();

                if (isolatedStore != null)
                {
                    bool isDirectory = Directory.Exists(desktopPathOrFilename);

                    string searchPattern = GetSearchPattern(application, directoryToUpload);

                    if (isDirectory == true)
                    {
                        string directoryName = new DirectoryInfo(desktopPathOrFilename).Name;

                        IRemoteFileInfo newDirectory = directoryToUpload;

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

        public static FileStream LoadFile(IRemoteApplication application, IRemoteFileInfo fileInfo, string desktopTempDirectory)
        {
            RemoteIsolatedStoreTools.DownloadFileFromDevice(application, fileInfo, desktopTempDirectory);

            string filename = Path.Combine(desktopTempDirectory, RemoteIsolatedStoreTools.GetShortName(fileInfo));

            return File.OpenRead(filename);
        }

        /// <summary>
        /// Sauvegarde d'un fichier
        /// </summary>
        /// <param name="desktopStream"></param>

        public static void SaveFile(IRemoteApplication application, IRemoteFileInfo directory, FileStream desktopStream)
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

        public static string LoadFileText(IRemoteApplication application, IRemoteFileInfo fileInfo, string tempDirectory)
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

        public static void SaveFileText(IRemoteApplication application, IRemoteFileInfo directory, string tempFullFilename, string text)
        {
            using (FileStream stream = new FileStream(tempFullFilename, FileMode.Create))
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
