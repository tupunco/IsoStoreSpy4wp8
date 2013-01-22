using System;
using IsoStoreSpy.Plugins.Shared;
using Microsoft.SmartDevice.Connectivity;
using IsoStoreSpy.Plugins.Controls;
using System.Windows.Controls;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using IsoStoreSpy.Tools;

namespace IsoStoreSpy.Plugins
{
    public class PreviewText : PreviewBase, IPreviewPlugin
    {
        /// <summary>
        /// Le FileInfo est-il supporté ?
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        public bool CheckFileInfoIsSupported(RemoteFileInfo fileInfo)
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
