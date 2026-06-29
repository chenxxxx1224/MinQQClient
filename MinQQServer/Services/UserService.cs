using System;
using MinQQServer.DataAccess;
using MinQQServer.Models;
using System.Data;

namespace MinQQServer.Services
{
    public class UserService
    {
        private readonly UserDao _userDao = new UserDao();

        public User Login(string account, string password)
        {
            var result = _userDao.GetByAccount(account, password);
            if (result != null && result.Rows.Count > 0)
            {
                return new User
                {
                    UserID = Convert.ToInt32(result.Rows[0]["UserID"]),
                    Username = result.Rows[0]["Username"].ToString()
                };
            }
            return null;
        }

        public (bool success, string message, int userId) Register(string account, string password)
        {
            if (_userDao.Exists(account))
            {
                return (false, "fail|账号已存在，请换一个用户名！", -1);
            }

            int newUserId = _userDao.Insert(account, password);
            if (newUserId > 0)
            {
                return (true, $"success|{newUserId}", newUserId);
            }
            return (false, "fail|注册失败，请重试！", -1);
        }
    }
}
