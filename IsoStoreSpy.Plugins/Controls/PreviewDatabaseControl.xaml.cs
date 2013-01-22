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
    /// Interaction logic for PreviewDatabaseControl.xaml
    /// </summary>
    public partial class PreviewDatabaseControl : UserControl, IPreviewControl
    {
        public PreviewDatabaseControl()
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
        /// Changement dans les tables
        /// </summary>

        public List<string> Tables
        {
            get { return (List<string>)GetValue(TablesProperty); }
            set { SetValue(TablesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tables.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TablesProperty =
            DependencyProperty.Register("Tables", typeof(List<string>), typeof(PreviewDatabaseControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Table selection
        /// </summary>

        public string SelectedTable
        {
            get { return (string)GetValue(SelectedTableProperty); }
            set { SetValue(SelectedTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTableProperty =
            DependencyProperty.Register("SelectedTable", typeof(string), typeof(PreviewDatabaseControl), new UIPropertyMetadata(null));

        /// <summary>
        /// On click sur la vue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            SqlWindow window = new SqlWindow();

            if (string.IsNullOrWhiteSpace(this.SelectedTable) == false)
            {
                window.SqlRequest = "SELECT * FROM " + this.SelectedTable;
            }

            window.Show();
        }

        /// <summary>
        /// Sauver les boutons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Plugin.Manager.Save();

                MessageBox.Show("Database saved !");
            }
            catch
            {
                MessageBox.Show("Error during the saving of the file !");
            }
        }
    }
}
