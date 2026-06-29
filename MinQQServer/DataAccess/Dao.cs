using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinQQServer.DataAccess
{
    public class Dao
    {
        public static string strCon = "Data Source=.;Initial Catalog=QQDB;User ID=sa;Password=20051224";

        #region sql的增删改方法
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
        #endregion

        #region sql的增删改方法(参数化)
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
        #endregion

        #region 查询单元格第一行第一个单元格的数据
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
        #endregion

        #region 查询单元格第一行第一个单元格的数据(参数化)
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
        #endregion

        #region 断层式读取
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
        #endregion

        #region 断层式读取(参数化)
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
        #endregion

        #region 非断层式读取(参数化)
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
        #endregion

        #region 非断层式读取
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
        #endregion
    }
}
