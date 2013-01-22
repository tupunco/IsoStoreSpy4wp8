using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IsoStoreSpy.Plugins.Shared;

namespace IsoStoreSpy.Plugins.Controls
{
    /// <summary>
    /// Interaction logic for PreviewImageControl.xaml
    /// </summary>
    public partial class PreviewImageControl : UserControl, IPreviewControl
    {
        public PreviewImageControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Plugin
        /// </summary>

        public IPreviewPlugin Plugin
        {
            get;
            set;
        }

        /// <summary>
        /// ImageSource
        /// </summary>

        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(PreviewImageControl), new UIPropertyMetadata(null, OnImageSourceChanged));

        /// <summary>
        /// Changement de l'image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private static void OnImageSourceChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            PreviewImageControl control = sender as PreviewImageControl;
            
            BitmapImage image = e.NewValue as BitmapImage;

            if (image != null)
            {
                control.SizeString = string.Format("{0}x{1}", image.PixelWidth, image.PixelHeight);
            }
            else
            {
                control.SizeString = string.Empty;
            }
        }

        /// <summary>
        /// Taille de l'image
        /// </summary>

        public string SizeString
        {
            get { return (string)GetValue(SizeStringProperty); }
            set { SetValue(SizeStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SizeString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeStringProperty =
            DependencyProperty.Register("SizeString", typeof(string), typeof(PreviewImageControl), new UIPropertyMetadata(null));

        

    }
}
