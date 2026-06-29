using System;
using MinQQServer.DataAccess;
using System.Data;
using System.Text;

namespace MinQQServer.Services
{
    public class MessageService
    {
        private readonly MessageDao _messageDao = new MessageDao();

        public int Save(int senderId, int receiverId, string content)
        {
            return _messageDao.Insert(senderId, receiverId, content);
        }

        public string GetOfflineMessagesText(int userId)
        {
            var result = _messageDao.GetOffline(userId);
            var sb = new StringBuilder();
            int count = 0;

            if (result != null && result.Rows.Count > 0)
            {
                foreach (DataRow row in result.Rows)
                {
                    int messageId = Convert.ToInt32(row["MessageID"]);
                    int senderId = Convert.ToInt32(row["SenderID"]);
                    string senderName = row["SenderName"].ToString();
                    string content = row["Content"].ToString();
                    string sendTime = row["SendTime"].ToString();

                    // 这里将消息文本和已读标记的副作用分开，但保留原有实现
                    _messageDao.MarkRead(messageId);
                    count++;
                }
            }

            return count.ToString();
        }

        public DataTable GetOfflineMessages(int userId)
        {
            return _messageDao.GetOffline(userId);
        }

        public void MarkRead(int messageId)
        {
            _messageDao.MarkRead(messageId);
        }
    }
}
