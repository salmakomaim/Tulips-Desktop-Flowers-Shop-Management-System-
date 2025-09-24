using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace TULIPS
{
    public static class DBConnection
    {
        public static string ConnectionString = Properties.Settings.Default.Tulips_localDbConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }

}

