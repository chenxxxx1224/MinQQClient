using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinQQClient
{
    public class Dao
    {
        public static string strCon = "Data Source=.;Initial Catalog=QQ_DB;User ID=sa;Password=20051224";//连接串


        #region sql的增删改方法
        /// <summary>
        /// sql的增删改方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
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
        /// <summary>
        /// sql的增删改方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
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
        /// <summary>
        /// sql查询方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>查询单元格第一行第一个单元格的数据</returns>
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
        /// <summary>
        /// sql查询方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>查询单元格第一行第一个单元格的数据</returns>
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
        /// <summary> 
        /// 断层式读取
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>返回整个查询表</returns>
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
        /// <summary> 
        /// 断层式读取
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>返回整个查询表</returns>
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

        /// <summary>
        /// 非断层式读取，适用于数据量较大的查询结果，可以逐行读取数据，节省内存
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IDataReader GetDataReader(string sql, List<SqlParameter> list)
        {

            SqlConnection con = new SqlConnection(strCon);
            con.Open();

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {

                cmd.Parameters.AddRange(list.ToArray());


                // 核心关键：使用 CommandBehavior.SequentialAccess
                // 这会告诉数据库：不要一次性把结果集加载到内存，而是让客户端一条条拉取
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);

                return reader;

            }
        }
        #endregion


        #region 非断层式读取

        /// <summary>
        /// 非断层式读取，适用于数据量较大的查询结果，可以逐行读取数据，节省内存
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IDataReader GetDataReader(string sql)
        {

            SqlConnection con = new SqlConnection(strCon);
            con.Open();

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {

                // 核心关键：使用 CommandBehavior.SequentialAccess
                // 这会告诉数据库：不要一次性把结果集加载到内存，而是让客户端一条条拉取
                IDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection);

                return reader;

            }
        }
        #endregion

    }
}
