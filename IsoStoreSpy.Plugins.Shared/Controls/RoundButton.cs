using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IsoStoreSpy.Plugins.Shared.Controls
{
    public class MetroButton : Button
    {
        static MetroButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroButton), new FrameworkPropertyMetadata(typeof(MetroButton)));
        }

        /// <summary>
        /// Icon du bouton
        /// </summary>

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(MetroButton), new UIPropertyMetadata(null));

        
    }
}
