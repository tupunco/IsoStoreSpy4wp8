using Microsoft.SmartDevice.Connectivity.Interface;

namespace IsoStoreSpy.Plugins.Shared
{
    public interface IPreviewPlugin
    {
        bool CheckFileInfoIsSupported(IRemoteFileInfo fileInfo);

        /// <summary>
        /// Manager
        /// </summary>

        RemoteFileInfoManager Manager
        {
            get;
            set;
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="fileInfo"></param>
        
        IPreviewControl Initialize(RemoteFileInfoManager manager);

        /// <summary>
        /// Control à renvoyer
        /// </summary>

        IPreviewControl Control
        {
            get;
            set;
        }

        /// <summary>
        /// Demande de rafraichissement
        /// </summary>

        void Refresh(RemoteFileInfoManager manager);
    }
}
