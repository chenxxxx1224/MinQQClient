using System;

namespace MinQQServer.Models
{
    public class FriendRequest
    {
        public int RequestID { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public int Status { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
