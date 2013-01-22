using Microsoft.SmartDevice.Connectivity.Interface;

using System.IO;

namespace IsoStoreSpy.Plugins.Shared
{
    public abstract class PreviewBase
    {
        /// <summary>
        /// Stockage du manager
        /// </summary>

        public RemoteFileInfoManager Manager
        {
            get;
            set;
        }

        /// <summary>
        /// Le control
        /// </summary>

        public IPreviewControl Control
        {
            get;
            set;
        }

        /// <summary>
        /// Verifie une extension
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        protected bool CheckExtension(string extension, IRemoteFileInfo fileInfo)
        {
            string name = RemoteIsolatedStoreTools.GetShortName(fileInfo);

            return (Path.GetExtension(name).ToLower() == extension.ToLower());
        }

        /// <summary>
        /// Verifier les extensions
        /// </summary>
        /// <param name="extensions"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        protected bool CheckExtensions(string[] extensions, IRemoteFileInfo fileInfo)
        {
            foreach (string extension in extensions)
            {
                if (CheckExtension(extension, fileInfo) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
