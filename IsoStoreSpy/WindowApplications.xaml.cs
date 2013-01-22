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
using IsoStoreSpy.ViewModels;
using System.Threading;

namespace IsoStoreSpy
{
    /// <summary>
    /// Interaction logic for WindowApplications.xaml
    /// </summary>
    public partial class WindowApplications : Window
    {
        public WindowApplications()
        {
            InitializeComponent();

            this.ApplicationViewModel = new ApplicationViewModel();
            this.DataContext = this;

            this.Loaded += new RoutedEventHandler(WindowApplications_Loaded);
        }

        /// <summary>
        /// Chargement des applications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void WindowApplications_Loaded(object sender, RoutedEventArgs e)
        {
            // permet de positionner la selection dans la combobox
            this.ApplicationViewModel.SelectedDevice = ApplicationViewModel.SelectedDevice;
        }

        /// <summary>
        /// Application ViewModel
        /// </summary>

        public ApplicationViewModel ApplicationViewModel
        {
            get { return (ApplicationViewModel)GetValue(ApplicationViewModelProperty); }
            set { SetValue(ApplicationViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ApplicationViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationViewModelProperty =
            DependencyProperty.Register("ApplicationViewModel", typeof(ApplicationViewModel), typeof(WindowApplications), new UIPropertyMetadata(null));

        /// <summary>
        /// Selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationViewModel.SelectedRemoteApplication == null)
            {
                App.ShowMessageBox("You must select an application !", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            this.DialogResult = true;
            this.Close();
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

        /// <summary>
        /// Rafraichissement des applications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RefreshApplication_Click(object sender, RoutedEventArgs e)
        {
            RefreshApplication();
        }

        /// <summary>
        /// Rafraichissement des applications
        /// </summary>

        private void RefreshApplication()
        {
            this.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            ApplicationViewModel.SelectedRemoteApplication = null;

            new Thread(new ThreadStart(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        ApplicationViewModel.RefreshRemoteApplications();
                    }
                    catch (Exception ex)
                    {
                        App.ShowError(ex.Message);
                    }
                    finally
                    {
                        this.IsEnabled = true;
                        this.Cursor = Cursors.Arrow;
                    }
                }));

            }
            )).Start();
        }

        /// <summary>
        /// Changement de device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RefreshApplication();
        }
    }
}
