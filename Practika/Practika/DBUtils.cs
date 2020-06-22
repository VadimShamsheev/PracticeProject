using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Practika.SqlConn
{
    class DBUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "91.191.231.76";
            int port = 3307;
            string database = "pr_is207_25";
            string username = "pr_is207_25";
            string password = "pr_is207_25";

            return DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }
    }
}
