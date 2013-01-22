using System;
using System.Data.SqlServerCe;

namespace IsoStoreSpy.Plugins
{
    public class SqlServerCeHelper
    {
        public static string Filename
        {
            get;
            set;
        }

        /// <summary>
        /// Execution d'une commande
        /// </summary>
        /// <param name="sqlRequest"></param>
        /// <param name="executeAction"></param>

        public static int? ExecuteNonQuery(string sqlRequest)
        {
            if (Filename != null)
            {
                string connectionString = "Data Source=" + Filename + ";Persist Security Info=false;";

                using (SqlCeConnection con = new SqlCeConnection(connectionString))
                {
                    con.Open();
                    // Read in all values in the table.
                    using (SqlCeCommand com = new SqlCeCommand(sqlRequest, con))
                    {
                        return  com.ExecuteNonQuery();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="sqlRequest"></param>
        /// <returns></returns>

        public static void ExecuteSqlRequest(string sqlRequest, Action<SqlCeDataReader> readAction)
        {
            if (Filename != null)
            {
                string connectionString = "Data Source=" + Filename + ";Persist Security Info=false;";

                using (SqlCeConnection con = new SqlCeConnection(connectionString))
                {
                    con.Open();
                    // Read in all values in the table.
                    using (SqlCeCommand com = new SqlCeCommand(sqlRequest, con))
                    {
                        SqlCeDataReader reader = com.ExecuteReader();

                        try
                        {
                            if (readAction != null)
                            {
                                readAction(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                            throw;
                        }
                    }
                }
            }
        }
    }
}
