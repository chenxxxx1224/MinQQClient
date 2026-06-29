using System;
using System.Data;

namespace MinQQServer.DataAccess
{
    public class MessageDao
    {
        public int Insert(int senderId, int receiverId, string content)
        {
            string sql = $"INSERT INTO [Message] (SenderID, ReceiverID, Content, SendTime, IsRead) VALUES ({senderId}, {receiverId}, '{content}', GETDATE(), 0)";
            return Dao.CUD(sql);
        }

        public DataTable GetOffline(int userId)
        {
            string sql = $@"
                SELECT m.MessageID, m.SenderID, u.Username AS SenderName, 
                       m.Content, CONVERT(VARCHAR, m.SendTime, 120) AS SendTime
                FROM [Message] m
                INNER JOIN [User] u ON m.SenderID = u.UserID
                WHERE m.ReceiverID = {userId} AND m.IsRead = 0
                ORDER BY m.SendTime ASC";
            return Dao.getData(sql);
        }

        public int MarkRead(int messageId)
        {
            string sql = $"UPDATE [Message] SET IsRead = 1 WHERE MessageID = {messageId}";
            return Dao.CUD(sql);
        }
    }
}
