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
using System.Windows.Shapes;

namespace IsoStoreSpy
{
    /// <summary>
    /// Interaction logic for WindowFolderName.xaml
    /// </summary>
    public partial class WindowFolderName : Window
    {
        public WindowFolderName()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        /// <summary>
        /// Nom du dossier
        /// </summary>

        public string FolderName
        {
            get { return (string)GetValue(FolderNameProperty); }
            set { SetValue(FolderNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FolderName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderNameProperty =
            DependencyProperty.Register("FolderName", typeof(string), typeof(WindowFolderName), new UIPropertyMetadata(null));

        /// <summary>
        /// Selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (this.FolderName != null)
            {
                this.FolderName = this.FolderName.Trim();

                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    if (FolderName.Contains(c) == true)
                    {
                        App.ShowMessageBox(
                            string.Format("Invalid character '{0}' in the filename !", c),
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                            );

                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(this.FolderName) == false)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Annulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}
