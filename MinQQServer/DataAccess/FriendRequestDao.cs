using System;
using System.Data;

namespace MinQQServer.DataAccess
{
    public class FriendRequestDao
    {
        public bool HasPending(int fromUserId, int toUserId)
        {
            string sql = $@"
                SELECT COUNT(*) FROM [FriendRequest] 
                WHERE FromUserID = {fromUserId} AND ToUserID = {toUserId} AND Status = 0";
            var result = Dao.getScalar(sql);
            return result != null && Convert.ToInt32(result) > 0;
        }

        public int Insert(int fromUserId, int toUserId)
        {
            string sql = $@"
                INSERT INTO [FriendRequest] (FromUserID, ToUserID, Status, RequestTime) 
                VALUES ({fromUserId}, {toUserId}, 0, GETDATE())";
            return Dao.CUD(sql);
        }

        public DataTable GetPending(int toUserId)
        {
            string sql = $@"
                SELECT fr.RequestID, fr.FromUserID, u.Username 
                FROM [FriendRequest] fr
                INNER JOIN [User] u ON fr.FromUserID = u.UserID
                WHERE fr.ToUserID = {toUserId} AND fr.Status = 0
                ORDER BY fr.RequestTime DESC";
            return Dao.getData(sql);
        }

        public DataTable GetById(int requestId)
        {
            string sql = $"SELECT FromUserID, ToUserID FROM [FriendRequest] WHERE RequestID = {requestId} AND Status = 0";
            return Dao.getData(sql);
        }

        public int UpdateStatus(int requestId, int status)
        {
            string sql = $"UPDATE [FriendRequest] SET Status = {status} WHERE RequestID = {requestId}";
            return Dao.CUD(sql);
        }
    }
}
