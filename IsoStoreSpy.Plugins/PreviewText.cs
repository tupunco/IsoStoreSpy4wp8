using IsoStoreSpy.Plugins.Controls;
using IsoStoreSpy.Plugins.Shared;

using Microsoft.SmartDevice.Connectivity.Interface;

using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace IsoStoreSpy.Plugins
{
    public class PreviewText : PreviewBase, IPreviewPlugin
    {
        /// <summary>
        /// Le FileInfo est-il supporté ?
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool CheckFileInfoIsSupported(IRemoteFileInfo fileInfo)
        {
            string filename = RemoteIsolatedStoreTools.GetShortName(fileInfo);

            if (filename != null && filename.ToLower() == "__applicationsettings")
            {
                return true;
            }

            return this.CheckExtensions(new string[]{".txt",".csv",".tab",".xml",".settings"}, fileInfo); 
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="manager"></param>
        public IPreviewControl Initialize(RemoteFileInfoManager manager )
        {
            this.Manager = manager;

            PreviewTextControl control = null;

            control = new PreviewTextControl();

            control.Plugin = this;

            Thread thread = new Thread( new ThreadStart( () =>
            {
                try
                {
                    using (StreamReader reader = new StreamReader(manager.Load()))
                    {
                        string text = reader.ReadToEnd();
                        if (text.Length > 200)
                            text = text.Substring(0, 200) + "\r\n...";

                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            control.Text = text;
                        }
                        ));
                    }
                }
                catch
                {
                }
            }
            ));

            thread.Start();

            this.Control = control as IPreviewControl;
            
            return this.Control;
        }


        public void Refresh(RemoteFileInfoManager manager)
        {
            this.Manager = manager;
        }
    }
}
