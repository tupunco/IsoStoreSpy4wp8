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
using System.Data.SqlServerCe;

namespace IsoStoreSpy.Plugins
{
    /// <summary>
    /// Interaction logic for SqlWindow.xaml
    /// </summary>
    public partial class SqlWindow : Window
    {
        public SqlWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Loaded += new RoutedEventHandler(SqlWindow_Loaded);
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void SqlWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Execute();
        }

        /// <summary>
        /// SqlRequest
        /// </summary>

        public string SqlRequest
        {
            get { return (string)GetValue(SqlRequestProperty); }
            set { SetValue(SqlRequestProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SqlRequest.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SqlRequestProperty =
            DependencyProperty.Register("SqlRequest", typeof(string), typeof(SqlWindow), new UIPropertyMetadata(null));



        public string ResultCountString
        {
            get { return (string)GetValue(ResultCountStringProperty); }
            set { SetValue(ResultCountStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResultCountString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultCountStringProperty =
            DependencyProperty.Register("ResultCountString", typeof(string), typeof(SqlWindow), new UIPropertyMetadata(null));

        

        /// <summary>
        /// Execute la SqlRequest
        /// </summary>
        /// <param name="sqlRequest"></param>

        private void Execute()
        {
            if (this.SqlRequest != null)
            {
                string request = this.SqlRequest.Trim();

                if (string.IsNullOrWhiteSpace(request) == true)
                {
                    return;
                }

                try
                {
                    this.ResultCountString = null;
                    this.GridView.Columns.Clear();
                    this.ListView.ItemsSource = null;
                    
                    string requestLower = request.ToLower();

                    if (requestLower.StartsWith("select") == true)
                    {
                        this.ExecuteSelect(request);
                    }
                    else
                    {
                        this.ExecuteNonQuery(request);

                        if (requestLower.StartsWith("create") || requestLower.StartsWith("drop"))
                        {
                            PreviewDatabase.Current.RefreshTable();
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Execution d'un insert, update
        /// </summary>
        /// <param name="request"></param>

        private void ExecuteNonQuery(string request)
        {
            int? result = SqlServerCeHelper.ExecuteNonQuery(request);

            if (result.HasValue == true)
            {
                if (result > -1)
                {
                    MessageBox.Show(string.Format("{0} line(s) modified !", result));
                }
                else
                {
                    MessageBox.Show("Sql query completed !");
                }
            }
        }

        /// <summary>
        /// Execution d'un select
        /// </summary>
        /// <param name="request"></param>

        private void ExecuteSelect(string request)
        {
            SqlServerCeHelper.ExecuteSqlRequest(request,

                new Action<SqlCeDataReader>((reader) =>
                {
                    List<List<object>> columns = new List<List<object>>();

                    while (reader.Read())
                    {
                        List<object> line = new List<object>();

                        for (int index = 0; index < reader.FieldCount; index++)
                        {
                            // initialisation
                            if (this.GridView.Columns.Count <= index)
                            {
                                this.GridView.Columns.Add(new GridViewColumn()
                                {
                                    Header = reader.GetName(index),
                                    DisplayMemberBinding = new Binding("[" + index + "]"),
                                    Width = double.NaN
                                });
                            }

                            line.Add(reader[index]);
                        }

                        columns.Add(line);
                    }

                    if (columns.Count > 0)
                    {
                        this.ResultCountString = string.Format("{0} result(s)", columns[0].Count);
                    }
                    else
                    {
                        this.ResultCountString = "No result";
                    }

                    this.ListView.ItemsSource = columns;
                }
            ));
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExecuteSql_Click(object sender, RoutedEventArgs e)
        {
            this.Execute();
        }
    }
}
