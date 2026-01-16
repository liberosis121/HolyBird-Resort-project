using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyBirdResort.DAO
{
    public class DBConnection
    {
        private string strConn = @"Data Source=DESKTOP-84TQEN7\SQLEXPRESS;Initial Catalog=DB_HolyBird;Integrated Security=True";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(strConn);
        }
    }
}
