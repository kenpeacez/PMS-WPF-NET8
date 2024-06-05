using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace PMS_WPF_NET8
{
    public class DBConnection
    {
        private DBConnection()
        {
        }

        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public MySqlConnection Connection { get; set; }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool DBConnect()
        {
            if (Connection == null)
            {
                //if (String.IsNullOrEmpty(DatabaseName))
                  //  return false;
                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", 
                    Properties.Settings.Default.dbServerAddress,
                    Properties.Settings.Default.dbName,
                    Properties.Settings.Default.dbUserID,
                    Properties.Settings.Default.dbPassword);
                Connection = new MySqlConnection(connstring);
                Connection.Open();
            }

            return true;
        }

        public void Close()
        {
            Connection.Close();
        }
    }
}
