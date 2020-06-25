using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEPTONSync
{
    internal class OracleOperations
    {
        private OracleConnection conn;
        private MoodleObj active_moodle;
        private static Random random = new Random();

        public OracleOperations(MoodleObj m)
        {
            this.conn = DBConnections.getConnection();
            this.active_moodle = m;
        }

        public List<OperationObjectStudent> getChanges()
        {
            List<OperationObjectStudent> operations = new List<OperationObjectStudent>();

            using (var cnx = DBConnections.getConnection())
            {
                cnx.Open();

                OracleCommand cmd = cnx.CreateCommand();

                cmd.CommandText = @"SELECT *
                                    FROM ACADORGA.WEB_CALLS
                                    WHERE DESTINATION_SYSTEM ='NEPTON'
                                    AND EXECUTED_FLAG=0";

                OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    OperationObjectStudent obj = new OperationObjectStudent();

                    obj.id = Int32.Parse(reader["id"].ToString());
                    obj.student_id = reader["INQUIRY_ID"].ToString();

                    JObject studentData = JObject.Parse(reader["WEB_CALL"].ToString());

                    obj.first_name = studentData["first_name"].ToString();
                    obj.last_name = studentData["last_name"].ToString();
                    obj.e_mail = $"{obj.student_id}@unicstudent.unic.ac.cy";
                    obj.password = RandomString(6);

                    operations.Add(obj);

                }

                cnx.Close();

            }
            return operations;
        }

        public bool UpdateRow(int rowid)
        {
            using (var cnx = DBConnections.getConnection())
            {
                cnx.Open();

                OracleCommand cmd;
                cmd = cnx.CreateCommand();

                cmd.CommandText = @"UPDATE ACADORGA.WEB_CALLS SET EXECUTED_FLAG=-1, DATE_EXECUTED=SysDate WHERE id=:id";

                cmd.Parameters.Add(":id", rowid);

                int rowsaffected = cmd.ExecuteNonQuery();

                if (rowsaffected > 0)
                    return true;
            }

            return false;
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}