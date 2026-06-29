using System;
using MinQQServer.DataAccess;
using MinQQServer.Models;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MinQQServer.Services
{
    public class FriendService
    {
        private readonly FriendDao _friendDao = new FriendDao();
        private readonly FriendRequestDao _requestDao = new FriendRequestDao();

        public string GetFriendListText(int userId)
        {
            var result = _friendDao.GetFriends(userId);
            var friends = new List<string>();
            if (result != null)
            {
                foreach (DataRow row in result.Rows)
                {
                    int friendId = Convert.ToInt32(row["UserID"]);
                    string friendName = row["Username"].ToString();
                    friends.Add($"{friendId}:{friendName}");
                }
            }
            return string.Join(",", friends);
        }

        public string SendRequestStatus(int fromUserId, int toUserId)
        {
            if (_friendDao.AreFriends(fromUserId, toUserId))
                return "already_friends";

            if (_requestDao.HasPending(fromUserId, toUserId))
                return "already_sent";

            _requestDao.Insert(fromUserId, toUserId);
            return "success";
        }

        public string GetPendingRequestsText(int toUserId)
        {
            var result = _requestDao.GetPending(toUserId);
            var requests = new List<string>();
            if (result != null)
            {
                foreach (DataRow row in result.Rows)
                {
                    int requestId = Convert.ToInt32(row["RequestID"]);
                    int fromUserId = Convert.ToInt32(row["FromUserID"]);
                    string fromUsername = row["Username"].ToString();
                    requests.Add($"{requestId}:{fromUserId}:{fromUsername}");
                }
            }
            return string.Join(",", requests);
        }

        public (bool ok, string message, int fromUserId) ProcessRequest(int requestId, int currentUserId, string action)
        {
            var requestResult = _requestDao.GetById(requestId);
            if (requestResult == null || requestResult.Rows.Count == 0)
                return (false, "fail|请求不存在或已处理", 0);

            int fromUserId = Convert.ToInt32(requestResult.Rows[0]["FromUserID"]);
            int toUserId = Convert.ToInt32(requestResult.Rows[0]["ToUserID"]);

            if (toUserId != currentUserId)
                return (false, "fail|无权操作", 0);

            if (action == "accept")
            {
                _friendDao.Add(fromUserId, toUserId);
                _requestDao.UpdateStatus(requestId, 1);
            }
            else if (action == "reject")
            {
                _requestDao.UpdateStatus(requestId, 2);
            }

            return (true, "success", fromUserId);
        }
    }
}
