using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IsoStoreSpy.Tools;
using Microsoft.SmartDevice.Connectivity;
using System.Windows.Controls;

namespace IsoStoreSpy.Plugins.Shared
{
    public interface IPreviewPlugin
    {
        bool CheckFileInfoIsSupported(RemoteFileInfo fileInfo);

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
