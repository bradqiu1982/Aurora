using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.IO;
using System.Data;

namespace Aurora.Models
{
    public class DBUtility
    {
        private static void logthdinfo(string info)
        {
            var filename = "d:\\log\\sqlexception-" + DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(filename))
            {
                var content = System.IO.File.ReadAllText(filename);
                content = content + "\r\n" + DateTime.Now.ToString() + " : " + info;
                System.IO.File.WriteAllText(filename, content);
            }
            else
            {
                System.IO.File.WriteAllText(filename, DateTime.Now.ToString() + " : " + info);
            }
        }

        private static bool IsDebug()
        {
            bool debugging = false;
#if DEBUG
            debugging = true;
#else
            debugging = false;
#endif
            return debugging;
        }

        private static SqlConnection GetLocalConnector()
        {
            var conn = new SqlConnection();
            try
            {
                if (IsDebug())
                {
                    conn.ConnectionString = "Server=wux-d80008792;User ID=dbg;Password=dbgpwd;Database=DebugDB;Connection Timeout=120;";
                }
                else
                {
                    conn.ConnectionString = "Server=wuxinpi;User ID=NebulaNPI;Password=abc@123;Database=NebulaTrace;Connection Timeout=120;";
                }

                //conn.ConnectionString = "Data Source = (LocalDb)\\MSSQLLocalDB; AttachDbFilename = ~\\App_Data\\Nebula.mdf; Integrated Security = True";
                //conn.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\Nebula.mdf") + ";Integrated Security=True;";
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                logthdinfo("open connect exception: " + ex.Message + "\r\n");
                CloseConnector(conn);
                return null;
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                logthdinfo("open connect exception: " + ex.Message + "\r\n");
                CloseConnector(conn);
                return null;
            }
        }

        public static void WriteDBWithTable(System.Data.DataTable dt, string tablename)
        {
            if (dt.Rows.Count > 0)
            {
                var targetcon = DBUtility.GetLocalConnector();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(targetcon))
                {
                    bulkCopy.DestinationTableName = tablename;
                    bulkCopy.BulkCopyTimeout = 180;

                    try
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        bulkCopy.WriteToServer(dt);
                        dt.Clear();
                    }
                    catch (Exception ex) { logthdinfo("WriteDBWithTable exception: " + ex.Message + "\r\n"); }

                }//end using
                DBUtility.CloseConnector(targetcon);
            }//end if
        }

        //"Server=CN-CSSQL;uid=SHG_Read;pwd=shgread;Database=InsiteDB;Connection Timeout=30;"
        public static SqlConnection GetConnector(string constr)
        {
            var conn = new SqlConnection();
            try
            {
                conn.ConnectionString = constr;
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static bool ExeSqlNoRes(SqlConnection conn, string sql)
        {
            if (conn == null)
                return false;

            try
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<List<object>> ExeSqlWithRes(SqlConnection conn, string sql)
        {
            var ret = new List<List<object>>();
            SqlDataReader sqlreader = null;
            try
            {
                if (conn == null)
                    return ret;

                var command = conn.CreateCommand();
                command.CommandText = sql;
                sqlreader = command.ExecuteReader();
                if (sqlreader.HasRows)
                {

                    while (sqlreader.Read())
                    {
                        var newline = new List<object>();
                        for (var i = 0; i < sqlreader.FieldCount; i++)
                        {
                            newline.Add(sqlreader.GetValue(i));
                        }
                        ret.Add(newline);
                    }
                }

                sqlreader.Close();
                CloseConnector(conn);
                return ret;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                if (sqlreader != null)
                {
                    sqlreader.Close();
                }
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                ret.Clear();
                return ret;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                if (sqlreader != null)
                {
                    sqlreader.Close();
                }
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                ret.Clear();
                return ret;
            }
        }
        public static DataTable ExecuteSqlReturnTable(SqlConnection conn, string sql)
        {
            try
            {
                var dt = new DataTable();
                SqlDataAdapter myAd = new SqlDataAdapter(sql, conn);
                myAd.SelectCommand.CommandTimeout = 0;
                myAd.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static void CloseConnector(SqlConnection conn)
        {
            if (conn == null)
                return;

            try
            {
                conn.Close();
            }
            catch (SqlException ex)
            {
                logthdinfo("close conn exception: " + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                logthdinfo("close conn exception: " + ex.Message + "\r\n");
            }
        }

        public static bool ExeLocalSqlNoRes(string sql, Dictionary<string, string> parameters = null)
        {
            var conn = GetLocalConnector();
            if (conn == null)
                return false;

            try
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        SqlParameter parameter = new SqlParameter();
                        parameter.ParameterName = param.Key;
                        parameter.SqlDbType = SqlDbType.NVarChar;
                        parameter.Value = param.Value;
                        command.Parameters.Add(parameter);
                    }
                }
                command.ExecuteNonQuery();
                CloseConnector(conn);
                return true;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }


        public static DataTable ExecuteLocalQueryReturnTable(string sql)
        {
            var conn = GetLocalConnector();
            try
            {
                var dt = new DataTable();
                SqlDataAdapter myAd = new SqlDataAdapter(sql, conn);
                myAd.SelectCommand.CommandTimeout = 0;
                myAd.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static List<List<object>> ExeLocalSqlWithRes(string sql, Dictionary<string, string> parameters = null)
        {
            var ret = new List<List<object>>();
            var conn = GetLocalConnector();
            SqlDataReader sqlreader = null;
            try
            {
                if (conn == null)
                    return ret;

                var command = conn.CreateCommand();
                command.CommandText = sql;
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        SqlParameter parameter = new SqlParameter();
                        parameter.ParameterName = param.Key;
                        parameter.SqlDbType = SqlDbType.NVarChar;
                        parameter.Value = param.Value;
                        command.Parameters.Add(parameter);
                    }
                }

                sqlreader = command.ExecuteReader();
                if (sqlreader.HasRows)
                {

                    while (sqlreader.Read())
                    {
                        var newline = new List<object>();
                        for (var i = 0; i < sqlreader.FieldCount; i++)
                        {
                            newline.Add(sqlreader.GetValue(i));
                        }
                        ret.Add(newline);
                    }
                }

                sqlreader.Close();
                CloseConnector(conn);
                return ret;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                if (sqlreader != null)
                {
                    sqlreader.Close();
                }
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                ret.Clear();
                return ret;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                if (sqlreader != null)
                {
                    sqlreader.Close();
                }
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                ret.Clear();
                return ret;
            }
        }

        private static SqlConnection GetMESConnector()
        {
            var conn = new SqlConnection();
            try
            {
                conn.ConnectionString = "Server=CN-CSSQL;uid=SHG_Read;pwd=shgread;Database=InsiteDB;Connection Timeout=30;";
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static bool ExeMESSqlNoRes(string sql)
        {
            var conn = GetMESConnector();
            if (conn == null)
                return false;

            try
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CloseConnector(conn);
                return true;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public static List<List<object>> ExeMESSqlWithRes(string sql)
        {
            var ret = new List<List<object>>();
            var conn = GetMESConnector();
            try
            {
                if (conn == null)
                    return ret;

                var command = conn.CreateCommand();
                command.CommandTimeout = 60;
                command.CommandText = sql;
                var sqlreader = command.ExecuteReader();
                if (sqlreader.HasRows)
                {

                    while (sqlreader.Read())
                    {
                        var newline = new List<object>();
                        for (var i = 0; i < sqlreader.FieldCount; i++)
                        {
                            newline.Add(sqlreader.GetValue(i));
                        }
                        ret.Add(newline);
                    }
                }

                sqlreader.Close();
                CloseConnector(conn);
                return ret;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
        }

        private static SqlConnection GetTraceConnector()
        {
            var conn = new SqlConnection();
            try
            {
                conn.ConnectionString = "Server=wuxinpi;User ID=NPI;Password=NPI@NPI;Database=NPITrace;Connection Timeout=120;";
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static bool ExeTraceSqlNoRes(string sql)
        {
            var conn = GetTraceConnector();
            if (conn == null)
                return false;

            try
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CloseConnector(conn);
                return true;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                CloseConnector(conn);
                //System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public static List<List<object>> ExeTraceSqlWithRes(string sql)
        {
            var ret = new List<List<object>>();
            var conn = GetTraceConnector();
            try
            {
                if (conn == null)
                    return ret;

                var command = conn.CreateCommand();
                command.CommandTimeout = 60;
                command.CommandText = sql;
                var sqlreader = command.ExecuteReader();
                if (sqlreader.HasRows)
                {

                    while (sqlreader.Read())
                    {
                        var newline = new List<object>();
                        for (var i = 0; i < sqlreader.FieldCount; i++)
                        {
                            newline.Add(sqlreader.GetValue(i));
                        }
                        ret.Add(newline);
                    }
                }

                sqlreader.Close();
                CloseConnector(conn);
                return ret;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
        }

        private static SqlConnection GetMESReportConnector()
        {
            var conn = new SqlConnection();
            try
            {
                conn.ConnectionString = @"Server=wux-prod02\mes_report;uid=Active_NPI;pwd=Active@123;Database=PDMS;Connection Timeout=30;";
                //conn.ConnectionString = "Data Source=cn-csrpt;Initial Catalog=SummaryDB;Integrated Security=True;Connection Timeout=30;";
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                logthdinfo("fail to connect to the mes report pdms database:" + ex.Message);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                logthdinfo("fail to connect to the mes report pdms database" + ex.Message);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static List<List<object>> ExeMESReportSqlWithRes(string sql)
        {
            //var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var ret = new List<List<object>>();
            var conn = GetMESReportConnector();
            try
            {
                if (conn == null)
                    return ret;

                var command = conn.CreateCommand();
                command.CommandTimeout = 60;
                command.CommandText = sql;
                var sqlreader = command.ExecuteReader();
                if (sqlreader.HasRows)
                {

                    while (sqlreader.Read())
                    {
                        var newline = new List<object>();
                        for (var i = 0; i < sqlreader.FieldCount; i++)
                        {
                            newline.Add(sqlreader.GetValue(i));
                        }
                        ret.Add(newline);
                    }
                }

                sqlreader.Close();
                CloseConnector(conn);
                return ret;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
        }


        private static SqlConnection GetSumConnector()
        {
            var conn = new SqlConnection();
            try
            {
                conn.ConnectionString = "Server=shg-csrpt;uid=Active_NPI;pwd=Active@NPI;Database=SummaryDB;Connection Timeout=30;";
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                logthdinfo("fail to connect to the mes report pdms database:" + ex.Message);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
            catch (Exception ex)
            {
                logthdinfo("fail to connect to the mes report pdms database" + ex.Message);
                //System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static List<List<object>> ExeSumSqlWithRes(string sql)
        {
            //var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var ret = new List<List<object>>();
            var conn = GetSumConnector();
            try
            {
                if (conn == null)
                    return ret;

                var command = conn.CreateCommand();
                command.CommandTimeout = 60;
                command.CommandText = sql;
                var sqlreader = command.ExecuteReader();
                if (sqlreader.HasRows)
                {

                    while (sqlreader.Read())
                    {
                        var newline = new List<object>();
                        for (var i = 0; i < sqlreader.FieldCount; i++)
                        {
                            newline.Add(sqlreader.GetValue(i));
                        }
                        ret.Add(newline);
                    }
                }

                sqlreader.Close();
                CloseConnector(conn);
                return ret;
            }
            catch (SqlException ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
            catch (Exception ex)
            {
                logthdinfo("execute exception: " + sql + "\r\n" + ex.Message + "\r\n");
                //System.Windows.MessageBox.Show(ex.ToString());
                CloseConnector(conn);
                ret.Clear();
                return ret;
            }
        }

    }
}