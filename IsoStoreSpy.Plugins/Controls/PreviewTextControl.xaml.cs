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
using System.IO;

namespace IsoStoreSpy.Plugins.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PreviewTextControl : UserControl, IPreviewControl
    {
        /// <summary>
        /// Constructeur
        /// </summary>

        public PreviewTextControl()
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
        /// Text
        /// </summary>

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PreviewTextControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Sauvegarde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string text = this.Text;

            try
            {
                using (StreamWriter writer = new StreamWriter(this.Plugin.Manager.Filename))
                {
                    writer.Write(text);
                }

                this.Plugin.Manager.Save();

                MessageBox.Show("Text file saved !");            
            }
            catch
            {
                MessageBox.Show("Error during the saving of the file !");
            }
        }
    }
}
