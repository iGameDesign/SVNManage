using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CGUIClient
{
	public partial class FormSvnOP : Form
	{
        private int nBranches;// trunkroots.conf 文件里面分支数量
        private Dictionary<string, int> dictBranches = new Dictionary<string, int>();

		public FormSvnOP()
		{
			InitializeComponent();
			this.txtBoxIP.Text = "192.168.1.242";
			this.txtBoxPort.Text = "18008";
		}

        //public void InitRootCB()
        //{
        //    string strroots = DispatchCmd("getroots");
        //    string[] strArray = strroots.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
        //    this.cbSelRoot.Items.Clear();
        //    this.cbSelRoot.Items.AddRange(strArray);
        //    this.cbSelRoot.SelectedIndex = 1;
        //}

        public void GetTrunkRoots()
        {
            string strroots = DispatchCmd("getroots");
            string[] strArray = strroots.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            this.nBranches = strArray.Length;
            //this.ckListBoxBranches.Items.Clear();
            this.cbSelRoot.Items.Clear();
            dictBranches.Clear();

            //int[] status = this.QueryLockStatus();
            for (int i = 0; i < this.nBranches; i++)
            {
                //if (status[i] != 0)
                if (strArray[i] != "")
                {
                    string[] temp = strArray[i].Split('\t');
                    dictBranches.Add(temp[1], int.Parse(temp[0]));

                    //bool bFlag = status[i] != 0 ? true : false;
                    this.cbSelRoot.Items.Add(temp[1]);
                    //this.cbSelRoot.SetItemChecked(i, bFlag);
                    //this.cbSelRoot.SelectedIndex = 1;

                    //this.Log(string.Format("SetItemChecked {0} .", i), Color.Red);
                }
            }
            this.cbSelRoot.SelectedIndex = 1;
        }

		///从服务器获取主分支列表
		///创建主分支
		///

		///创建开发分支
		///
		private void Log(string strlog, Color clr)
		{
			string logstr = string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), strlog, "\r\n");
			this.txtBoxInfo.SelectionLength = 0;
			this.txtBoxInfo.SelectionColor = clr;
			this.txtBoxInfo.AppendText(logstr);
			this.txtBoxInfo.Refresh();
			this.txtBoxInfo.ScrollToCaret();
		}

		private string DispatchCmd(string strcmd)
		{
			string result = "";
			try
			{
				TcpClient client = new TcpClient();
				this.Log(string.Format("连接[{0}:{1}]...", this.txtBoxIP.Text, this.txtBoxPort.Text), Color.Blue);
				client.Connect(this.txtBoxIP.Text, int.Parse(this.txtBoxPort.Text));
				this.Log("已连接!", Color.DarkSlateBlue);
				Stream stream = client.GetStream();
				byte[] bytes = new ASCIIEncoding().GetBytes(strcmd);
				Encoding unicode = Encoding.Unicode;
				bytes = unicode.GetBytes(strcmd);
				this.Log(strcmd, Color.IndianRed);
				this.Log("命令发送中....", Color.Green);
				stream.Write(bytes, 0, bytes.Length);
				byte[] buffer = new byte[0x400];
				int num = stream.Read(buffer, 0, 0x400);
				result = unicode.GetString(buffer).Trim(new char[] { '\0' });
				this.Log("接收申请结果:" + result, Color.DarkRed);
				client.Close();
			}
			catch (Exception exception)
			{
				this.Log(string.Format("Error:{0}", exception.Message), Color.Red);
			}
			return result;
		}

		private void btnCommit_Click(object sender, EventArgs e)
		{
			///创建分支，根据选择创建主分支还是开发分支
			string cmd;
			if (this.chkRoot.Checked == true)
			{
				// 创建主分支，需弹出提示确认
				System.Windows.Forms.DialogResult ret = MessageBox.Show("确认你要添加一个主分支么！", "提醒！", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
				if (ret == System.Windows.Forms.DialogResult.OK)
				{
					Log("请求创建主分支:" + this.txtBoxBranch.Text, Color.DarkRed);
					cmd = string.Format("createroot\t{0}\t{1}\t{2}\t{3}", this.txtLocalIP.Text, this.txtBoxName.Text, this.txtBoxBranch.Text, this.cbSelRoot.Text);
				}
				else
				{
					// do nothing
					Log("取消主分支创建:" + this.txtBoxBranch.Text, Color.DarkRed);
					return;
				}
			}
			else
			{
				// 创建开发分支直接创建
				cmd = string.Format("createbranch\t{0}\t{1}\t{2}\t{3}", this.txtLocalIP.Text, this.txtBoxName.Text, this.txtBoxBranch.Text, this.cbSelRoot.Text);
			}
			DispatchCmd(cmd);
			//InitRootCB();
            this.GetTrunkRoots();
		}

		private void btnRefreshRoots_Click(object sender, EventArgs e)
		{
			//InitRootCB();
            this.GetTrunkRoots();
		}

		private void FormSvnOP_Load(object sender, EventArgs e)
		{
			this.txtBoxName.Text = Environment.MachineName;
			this.txtLocalIP.Text = Dns.Resolve(this.txtBoxName.Text).AddressList[0].ToString();
		}
	}
}
