using System;

namespace MinQQServer.Models
{
    public class Friend
    {
        public int FriendID { get; set; }
        public int UserID { get; set; }
        public int FriendUserID { get; set; }
        public DateTime AddTime { get; set; }
    }
}
