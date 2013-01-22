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
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Collections.Generic;

namespace IsoStoreSpy.Plugins
{
    public class PreviewDatabase : PreviewBase, IPreviewPlugin
    {
        /// <summary>
        /// Le FileInfo est-il supporté ?
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>

        public bool CheckFileInfoIsSupported(RemoteFileInfo fileInfo)
        {
            return this.CheckExtension(".sdf", fileInfo); 
        }

        /// <summary>
        /// Current instance
        /// </summary>

        public static PreviewDatabase Current
        {
            get
            {
                return _Current;
            }
        }

        private static PreviewDatabase _Current;

        /// <summary>
        /// Rafraichir la table
        /// </summary>

        public void RefreshTable()
        {
            List<string> tableNames = new List<string>();
            PreviewDatabaseControl control = this.Control as PreviewDatabaseControl;

            // execution
            SqlServerCeHelper.ExecuteSqlRequest("select table_name as TableName, TABLE_TYPE as TableType from INFORMATION_SCHEMA.Tables where TABLE_TYPE = 'TABLE'",

                new Action<SqlCeDataReader>((reader) =>
                {
                    while (reader.Read())
                    {
                        string tableName = reader["TableName"] as string;

                        tableNames.Add(tableName);
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        control.Tables = tableNames;
                    }));

                }
                    )
                );
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="manager"></param>

        public IPreviewControl Initialize(RemoteFileInfoManager manager )
        {
            _Current = this;

            this.Manager = manager;
            this.Control = new PreviewDatabaseControl();

            Thread thread = new Thread(new ThreadStart(() =>
            {
                string filename = null;

                try
                {
                    filename = manager.Load();
                }
                catch (FileLoadException ex)
                {
                    MessageBox.Show("This file is locked by the phone application. To access it, quit the phone application and retry to select the file. It's typical in the case of a database file (.sdf)");
                    //Affichage que le fichier est locké !
                    return;
                }

                SqlServerCeHelper.Filename = filename;

                this.RefreshTable();
            }
            ));

            thread.Start();
            
            return this.Control;
        }

        /// <summary>
        /// Rafraichir
        /// </summary>
        /// <param name="manager"></param>

        public void Refresh(RemoteFileInfoManager manager)
        {
            this.Manager = manager;
        }
    }
}
