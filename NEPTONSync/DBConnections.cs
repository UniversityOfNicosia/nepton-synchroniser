using Oracle.ManagedDataAccess.Client;

namespace NEPTONSync
{
    class DBConnections
    {
        public static OracleConnection getConnection()
        {
            OracleConnection conn = new OracleConnection("User Id=elearning_user;Password=elearning_user;Data Source=192.168.212.4:1521/UNIC");
            return conn;
        }

        
    }
}
