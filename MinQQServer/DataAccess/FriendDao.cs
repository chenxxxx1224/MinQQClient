using System;
using System.Data;

namespace MinQQServer.DataAccess
{
    public class FriendDao
    {
        public int Add(int fromUserId, int toUserId)
        {
            string sql = $@"
                INSERT INTO [Friend] (UserID, FriendUserID, AddTime) 
                VALUES ({fromUserId}, {toUserId}, GETDATE())";
            return Dao.CUD(sql);
        }

        public bool AreFriends(int userA, int userB)
        {
            string sql = $@"
                SELECT COUNT(*) FROM [Friend] 
                WHERE (UserID = {userA} AND FriendUserID = {userB})
                   OR (UserID = {userB} AND FriendUserID = {userA})";
            var result = Dao.getScalar(sql);
            return result != null && Convert.ToInt32(result) > 0;
        }

        public DataTable GetFriends(int userId)
        {
            string sql = $@"
                SELECT u.UserID, u.Username 
                FROM [Friend] f
                INNER JOIN [User] u ON f.FriendUserID = u.UserID
                WHERE f.UserID = {userId}
                UNION
                SELECT u.UserID, u.Username 
                FROM [Friend] f
                INNER JOIN [User] u ON f.UserID = u.UserID
                WHERE f.FriendUserID = {userId}";
            return Dao.getData(sql);
        }
    }
}
