using System;
using System.Windows.Forms;
using System.Text.Json;

namespace MinQQClient
{
    public partial class AddFriend : Form
    {
        private Client client;
        private int? searchResultUserId;  // 搜索结果的用户ID
        private string searchResultUsername;  // 搜索结果的用户名

        public AddFriend(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        // 搜索用户
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string userIdStr = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(userIdStr))
            {
                MessageBox.Show("请输入用户ID！", "提示");
                return;
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                MessageBox.Show("用户ID必须是数字！", "提示");
                return;
            }

            // 不能添加自己
            if (userId == client.UserId)
            {
                MessageBox.Show("不能添加自己为好友！", "提示");
                return;
            }

            try
            {
                // 发送搜索请求（MsgType = 4, 格式：search|userId）
                await client.SendMessageAsync(4, $"search|{userId}");

                // 等待搜索结果
                var response = await client.ReceiveMessageAsync();

                if (response != null && response.MsgType == 4)
                {
                    // 格式：success|userId|username 或 fail|reason
                    string[] parts = response.Content.Split(new[] { '|' }, 3);
                    if (parts[0] == "success")
                    {
                        searchResultUserId = int.Parse(parts[1]);
                        searchResultUsername = parts[2];
                        txtResult.Text = $"用户ID：{searchResultUserId}\r\n用户名：{searchResultUsername}\r\n\r\n点击【添加】发送好友请求";
                    }
                    else
                    {
                        searchResultUserId = null;
                        searchResultUsername = null;
                        txtResult.Text = "未找到该用户";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("搜索失败：" + ex.Message, "错误");
            }
        }

        // 添加好友
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!searchResultUserId.HasValue)
            {
                MessageBox.Show("请先搜索用户！", "提示");
                return;
            }

            try
            {
                // 发送添加好友请求（MsgType = 5, 格式：toUserId）
                await client.SendMessageAsync(5, searchResultUserId.Value.ToString());

                // 等待响应
                var response = await client.ReceiveMessageAsync();

                if (response != null && response.MsgType == 5)
                {
                    if (response.Content == "success")
                    {
                        MessageBox.Show("好友请求已发送！", "提示");
                        this.Close();
                    }
                    else if (response.Content == "already_friends")
                    {
                        MessageBox.Show("该用户已经是您的好友！", "提示");
                    }
                    else if (response.Content == "already_sent")
                    {
                        MessageBox.Show("您已发送过好友请求，请等待对方处理！", "提示");
                    }
                    else
                    {
                        MessageBox.Show("发送失败：" + response.Content, "提示");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加失败：" + ex.Message, "错误");
            }
        }

        // 关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
