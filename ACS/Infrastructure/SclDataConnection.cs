using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS.Infrastructure
{
    internal static class SclDataConnection
    {
        internal static SqlConnection _connection = null!;
        internal static void ConnectionConfigure()
        {
            var config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false).Build();
            _connection = new(config.GetConnectionString("DefaultConnection"));
        }

        internal static void OpenConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        internal static void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }


        internal static DataTable GetData(string query)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(query, _connection);
                
                cmd.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter?.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
        }
        static SclDataConnection()
        {
            ConnectionConfigure();
        }
    }
}
