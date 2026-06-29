using System;
using System.Data;

namespace MinQQServer.DataAccess
{
    public class UserDao
    {
        public DataTable GetByAccount(string account, string password)
        {
            string sql = $"SELECT UserID, Username FROM [User] WHERE Username = '{account}' AND Password = '{password}'";
            return Dao.getData(sql);
        }

        public bool Exists(string account)
        {
            string sql = $"SELECT COUNT(*) FROM [User] WHERE Username = '{account}'";
            var result = Dao.getData(sql);
            return result != null && Convert.ToInt32(result.Rows[0][0]) > 0;
        }

        public int Insert(string account, string password)
        {
            string sql = $"INSERT INTO [User] (Username, Password, Status) VALUES ('{account}', '{password}', 1); SELECT SCOPE_IDENTITY();";
            var result = Dao.getData(sql);
            if (result != null && result.Rows.Count > 0)
            {
                return Convert.ToInt32(result.Rows[0][0]);
            }
            return -1;
        }
    }
}
