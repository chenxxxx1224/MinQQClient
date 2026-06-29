using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MinQQClient.HelperClass
{
    public class Dao
    {
        public static string strCon = "Data Source=.;Initial Catalog=QQ_DB;User ID=sa;Password=20051224";

        public static int CUD(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static int CUD(string sql, List<SqlParameter> list)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddRange(list.ToArray());
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static object getScalar(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static object getScalar(string sql, List<SqlParameter> list)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddRange(list.ToArray());
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getData(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable getData(string sql, List<SqlParameter> list)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(sql, con))
                    {
                        da.SelectCommand.Parameters.AddRange(list.ToArray());
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IDataReader GetDataReader(string sql, List<SqlParameter> list)
        {
            SqlConnection con = new SqlConnection(strCon);
            con.Open();
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddRange(list.ToArray());
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);
                return reader;
            }
        }

        public static IDataReader GetDataReader(string sql)
        {
            SqlConnection con = new SqlConnection(strCon);
            con.Open();
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);
                return reader;
            }
        }
    }
}
