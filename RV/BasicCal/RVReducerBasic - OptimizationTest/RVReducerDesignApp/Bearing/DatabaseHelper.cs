using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace WPFBearings
{
    public class DatabaseHelper
    {
        private string connectionString;
        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public DataTable GetDataTable(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data from database: " + ex.Message);
            }
            return dataTable;
        }
    }
}
