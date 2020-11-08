using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Chinese_Chess
{
    public partial class Connect : Form
    {
        private bool isClicked = false;
        public Connect()
        {
            InitializeComponent();
        }

        private void Connect_Load(object sender, EventArgs e)
        {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            IPAddress[] ip_list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            IPAddress myIp = ip_list[ip_list.Length - 1];//获取IP地址
            AddressIP = myIp.ToString();
            Local_IP_input.Text = AddressIP;
            Local_IP_input.Enabled = false;
        }

        private void Listen_btn_Click(object sender, EventArgs e)
        {
            Listen_btn.Enabled = false;
            Thread ListenThread = new Thread(new ThreadStart(LAN_connection.Lan_connection.Listen_Operation));
            ListenThread.Start();
            MessageBox.Show("端口已打开！");
            MainWnd.CurrentPlayer = Player.红;
            MainWnd.player = Player.红;
            isClicked = true;
            Close();
        }

        private void Connect_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Remote_IP_input.Text))
            {
                MessageBox.Show("请输入目的IP地址！");
                return;
            }
            if (LAN_connection.Lan_connection.Connect_Operation(Remote_IP_input.Text) == true)
            {
                MessageBox.Show("连接成功！");
                MainWnd.CurrentPlayer = Player.红;
                MainWnd.player = Player.黑;
                isClicked = true;
                Close();
            }
        }

        private void Connect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClicked)
                Application.Exit();
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)e;
            if (mouse.X >= 761 && mouse.Y <= 38)
                Close();
        }

        private void Connect_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X >= 761 && e.Y <= 38)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
        }

        private void Listen_btn_MouseMove(object sender, MouseEventArgs e)
        {
            Connect_btn.BackColor = Color.Transparent;
            Listen_btn.FlatAppearance.BorderSize = 0;
            Listen_btn.FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;
        }

        private void Connect_btn_MouseMove(object sender, MouseEventArgs e)
        {
            Connect_btn.BackColor = Color.Transparent;
            Connect_btn.FlatStyle = FlatStyle.Flat;
            Connect_btn.FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
        }
    }
}
