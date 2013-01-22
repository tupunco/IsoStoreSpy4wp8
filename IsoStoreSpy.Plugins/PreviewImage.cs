using System;
using IsoStoreSpy.Plugins.Shared;
using Microsoft.SmartDevice.Connectivity;
using IsoStoreSpy.Plugins.Controls;
using System.Windows.Controls;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace IsoStoreSpy.Plugins
{
    public class PreviewImage : PreviewBase, IPreviewPlugin
    {
        /// <summary>
        /// Le FileInfo est-il supporté ?
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        public bool CheckFileInfoIsSupported(RemoteFileInfo fileInfo)
        {
            return this.CheckExtensions(new string[]{".jpg",".png",".gif",".bmp",".ico"}, fileInfo); 
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="manager"></param>

        public IPreviewControl Initialize(RemoteFileInfoManager manager )
        {
            this.Manager = manager;

            PreviewImageControl control = null;

            control = new PreviewImageControl();

            this.Control = control as IPreviewControl;

            Thread thread = new Thread( new ThreadStart( () =>
            {
                try
                {
                    MemoryStream memory = null;

                    using (FileStream reader = manager.DonwloadFile())
                    {
                        byte[] array = new byte[reader.Length];
                        reader.Read(array, 0, array.Length);

                        memory = new MemoryStream(array);
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            BitmapImage source = new BitmapImage();

                            source.BeginInit();

                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.StreamSource = memory;

                            source.EndInit();

                            memory.Close();
                            memory.Dispose();

                            control.ImageSource = source;
                        }
                        catch
                        {
                        }
                    }
                    ));
                }
                catch
                {
                }
            }
            ));

            thread.Start();

            return this.Control;
        }


        public void Refresh(RemoteFileInfoManager manager)
        {
            this.Manager = manager;
        }
    }
}
