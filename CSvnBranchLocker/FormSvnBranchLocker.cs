using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;

namespace CSvnBranchLocker
{
    public partial class FormSvnBranchLocker : Form
    {
        string szFileIni = Directory.GetCurrentDirectory() + "/CSvnBranchLocker.ini";
        private int nBranches;// trunkroots.conf 文件里面分支数量
        private Dictionary<string, int> dictBranches = new Dictionary<string, int>();
        private string szPassword = "";

        #region "读取配置文件"

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        public string IniReadValue(string section, string key) 
        { 
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(section, key, "", temp, 255, this.szFileIni); 
            return temp.ToString();
        }

        #endregion

        public FormSvnBranchLocker()
        {
            InitializeComponent();
            //this.tbSvnServerIP.Text = "192.168.1.242";
            //this.tbSvnServerIP.Text = "svn.funova.com";
            //this.tbSvnServerIP.Text = "192.168.2.135";
            //this.tbSvnServerPort.Text = "18008";
            this.tbSvnServerIP.Text = IniReadValue("Svn", "SvnServerIP");
            this.tbSvnServerPort.Text = IniReadValue("Svn", "SvnServerPort");

            string[] temp = IniReadValue("Svn", "ProjectName").Split('|');
            this.cbProjectName.Items.AddRange(temp);
            if(temp.Length>0)
            {
                this.cbProjectName.SelectedIndex = 0;
            }

            szPassword = IniReadValue("Svn", "Password");
        }

        private void FormSvnBranchLocker_Load(object sender, EventArgs e)
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 1)
            {
                this.tbSvnServerIP.Text = commandLineArgs[1];
            }

            FormLogin frmLogin = new FormLogin(szPassword);
            while (true)
            {
                if (frmLogin.ShowDialog() != DialogResult.OK)
                {
                    base.Dispose();
                    return;
                }

                string strPassword = GetMD5(frmLogin.tbPassword.Text);
                if (this.Login(strPassword))
                {
                    //IPHostEntry host = new IPHostEntry();
                    //host = Dns.GetHostEntry(Dns.GetHostName());
                    this.tbLoginUser.Text = Environment.MachineName;
                    //this.tbLoginIP.Text = host.AddressList[1].MapToIPv4().ToString();
                    this.tbLoginIP.Text = GetIPv4Address();
                    //this.UpdateLockStatus();
                    this.GetTrunkRoots();
                    return;
                }

                MessageBox.Show("登录失败！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private static string GetMD5(string myString)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Unicode.GetBytes(myString);
            byte[] buffer2 = md.ComputeHash(bytes);
            string str = null;
            for (int i = 0; i < buffer2.Length; i++)
            {
                str = str + buffer2[i].ToString("x");
            }
            return str;
        }

        private bool Login(string strPassword)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(this.tbSvnServerIP.Text, int.Parse(this.tbSvnServerPort.Text));
                string s = string.Format("{0}\t{1}\t{2}\t{3}", "login", Environment.MachineName, strPassword, this.cbProjectName.SelectedItem);
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buf = new byte[0x400];
                int num = stream.Read(buf, 0, 0x400);
                client.Close();
                if (unicode.GetString(buf).IndexOf("失败") >= 0)
                {
                    return false;
                }

                return true;
            }
            catch(Exception exception)
            {
                this.Log(string.Format("Error:{0}", exception.Message), Color.Red);
                return false;
            }
        }

        private void btCommit_Click(object sender, EventArgs e)
        {
            //
        }

        /// <summary>
        /// APIs
        /// </summary>
        /// 
        private string DispatchCmd(string strcmd)
        {
            string result = "";
            try
            {
                TcpClient client = new TcpClient();
                this.Log(string.Format("连接[{0}:{1}]...", this.tbSvnServerIP.Text, this.tbSvnServerPort.Text), Color.Blue);
                client.Connect(this.tbSvnServerIP.Text, int.Parse(this.tbSvnServerPort.Text));
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
                this.Log("接收申请结果:\r\n" + result.Replace("\t", "\r\n"), Color.DarkRed);
                client.Close();
            }
            catch (Exception exception)
            {
                this.Log(string.Format("Error:{0}", exception.Message), Color.Red);
            }
            if (result == "操作成功。")
                result = "";
            return result;
        }

        private void GetTrunkRoots()
        {
            string strroots = DispatchCmd("getroots" + "\t" + this.cbProjectName.SelectedItem);
            string[] strArray = strroots.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            this.nBranches = strArray.Length;
            this.ckListBoxBranches.Items.Clear();
            dictBranches.Clear();

            int[] status = this.QueryLockStatus();
            for (int i = 0; i < this.nBranches; i++)
            {
                //if (status[i] != 0)
                if (strArray[i] != "")
                {
                    string[] temp = strArray[i].Split('\t');
                    dictBranches.Add(temp[1], int.Parse(temp[0]));

                    bool bFlag = status[i] != 0 ? true : false;
                    this.ckListBoxBranches.Items.Add(temp[1]);
                    this.ckListBoxBranches.SetItemChecked(i, bFlag);

                    //this.Log(string.Format("SetItemChecked {0} .", i), Color.Red);
                }
            }
        }

        private void Log(string strLog, Color clr)
        {
            this.rtbLogInfo.SelectionLength = 0;
            this.rtbLogInfo.SelectionColor = clr;
            this.rtbLogInfo.AppendText(string.Format("[{0}] {1}", DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"), strLog));
            this.rtbLogInfo.AppendText("\r\n");
            this.rtbLogInfo.Refresh();
            this.rtbLogInfo.ScrollToCaret();
        }

        //private int QueryLockStatus()
        //{
        //    try
        //    {
        //        TcpClient client = new TcpClient();
        //        client.Connect(this.tbSvnServerIP.Text, int.Parse(this.tbSvnServerPort.Text));
        //        string s = string.Format("{0}\t{1}\t{2}\t{3}", "getlockstatu", this.tbLoginUser.Text, "2", this.cbProjectName.SelectedItem);
        //        Stream stream = client.GetStream();
        //        byte[] bytes = new ASCIIEncoding().GetBytes(s);
        //        Encoding unicode = Encoding.Unicode;
        //        bytes = unicode.GetBytes(s);
        //        stream.Write(bytes, 0, bytes.Length);
        //        byte[] buf = new byte[0x400];
        //        int num = stream.Read(buf, 0, 0x400);
        //        client.Close();
        //        return int.Parse(unicode.GetString(buf));
        //    }
        //    catch (Exception exception)
        //    {
        //        this.Log(string.Format("Error:{0}", exception.Message), Color.Red);
        //        return -1;
        //    }
        //}

        private int[] QueryLockStatus()
        {
            int[] res = new int[this.nBranches];
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(this.tbSvnServerIP.Text, int.Parse(this.tbSvnServerPort.Text));
                string s = string.Format("{0}\t{1}\t{2}\t{3}", "getlockstatus", this.tbLoginUser.Text, "2", this.cbProjectName.SelectedItem);
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buf = new byte[0x400];
                int num = stream.Read(buf, 0, 0x400);
                client.Close();
                string strLockStatus = unicode.GetString(buf);
                string[] status = strLockStatus.Split(new char[]{','});
                for (int i = 0; i < status.Length; i++)
                { 
                    res[i] = int.Parse(status[i]);
                }

                //return int.Parse(unicode.GetString(buf));
                return res;
            }
            catch (Exception exception)
            {
                this.Log(string.Format("Error:{0}", exception.Message), Color.Red);
                return res;
            }
        }
        
        //private void UpdateLockStatus()
        //{
        //    int num = this.QueryLockStatus();
        //    if (num == 1)
        //    {
        //        this.btCommit.Text = "解锁分支";
        //        this.Log("目前分支处于锁定状态", Color.DarkRed);
        //    }
        //    else if (num == 0)
        //    {
        //        this.btCommit.Text = "锁定分支";
        //        this.Log("目前分支处于解锁状态", Color.DarkRed);
        //    }
        //    else
        //    {
        //        this.Log("服务器未连接，无法查询锁定状态！", Color.DarkRed);
        //    }
        //}

        private void btnLockBranches_Click(object sender, EventArgs e)
        {
            //int 
            string flags = "";
            int[] flag = new int[this.nBranches];
            for (int i = 0; i < this.ckListBoxBranches.Items.Count; i++)
            {
                bool bChecked = this.ckListBoxBranches.GetItemChecked(i);
                if (bChecked)
                {
                    string branch = this.ckListBoxBranches.Items[i].ToString();
                    int id;
                    if (dictBranches.TryGetValue(branch, out id))
                    {
                        //flag[i] = i + 1;
                        flag[i] = id;
                    }
                }
            }
            flags = String.Join(",", flag);

            try
            {
                TcpClient client = new TcpClient();
                this.Log(string.Format("连接[{0}:{1}]...", this.tbSvnServerIP.Text, this.tbSvnServerPort.Text), Color.Blue);
                client.Connect(this.tbSvnServerIP.Text, int.Parse(this.tbSvnServerPort.Text));
                this.Log("已连接!", Color.DarkSlateBlue);
                string s = string.Format("{0}\t{1}\t{2}\t{3}", new object[] { "lockbranches", this.tbLoginUser.Text, flags, this.cbProjectName.SelectedItem });
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                this.Log("命令发送中...", Color.Green);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buffer = new byte[0x400];
                int num = stream.Read(buffer, 0, 0x400);
                this.Log("接收结果:" + unicode.GetString(buffer) + "\r\n", Color.DarkRed);
                client.Close();
                //this.UpdateLockStatu();
                //MessageBox.Show(unicode.GetString(buffer), "注意", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception exception)
            {
                this.Log(string.Format("Error:{0}", exception.Message), Color.Red);
            }
        }

        private void btnUpdateBranches_Click(object sender, EventArgs e)
        {
            this.GetTrunkRoots();
        }

        public static string GetIPv4Address()
        {
            string sz_ip = "";
            IPAddress[] ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
            foreach (IPAddress ip in ips)
            {
                //根据AddressFamily判断是否为ipv4,如果是InterNetWork则为ipv6
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    sz_ip = ip.ToString();
                    break;
                }
            }
            return sz_ip;
        }

        private void btAddUnblockUser_Click(object sender, EventArgs e)
        {
            if (this.tbUnblockUser.Text == "")
                return;

            bool bExist = false;
            for (int i = 0; i < this.ckListBoxUnblockUser.Items.Count; i++)
            {
                if (this.tbUnblockUser.Text == this.ckListBoxUnblockUser.Items[i].ToString())
                {
                    bExist = true;
                    break;
                }
            }

            if(!bExist)
            {
                this.ckListBoxUnblockUser.Items.Add(this.tbUnblockUser.Text);

                string strUsers = "";
                string[] users = new string[this.ckListBoxUnblockUser.Items.Count];
                for (int i = 0; i < this.ckListBoxUnblockUser.Items.Count; i++)
                {
                    users[i] = this.ckListBoxUnblockUser.Items[i].ToString();
                }
                strUsers = String.Join("\r\n", users);

                string strCMD = string.Format("{0}\t{1}\t{2}\t{3}",
                    new object[] { "setUnblockUser", this.tbLoginUser.Text, this.cbProjectName.SelectedItem, strUsers });
                string strRet = DispatchCmd(strCMD);
                this.Log("接收结果:" + strRet + "\n", Color.DarkRed);
            }
            else
            {
                this.Log("接收结果:" + "白名单重复。" + "\n", Color.DarkRed);
            }
        }

        private void btUpdateUnblockUser_Click(object sender, EventArgs e)
        {
            string strCMD = string.Format("{0}\t{1}\t{2}", "getUnblockUser", this.tbLoginUser.Text, this.cbProjectName.SelectedItem);
            string strRet = DispatchCmd(strCMD);
            string[] strArray = strRet.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            this.ckListBoxUnblockUser.Items.Clear();

            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] != "")
                {
                    this.ckListBoxUnblockUser.Items.Add(strArray[i]);
                }
            }
        }

        private void btDelUnblockUser_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.ckListBoxUnblockUser.Items.Count;)
            {
                bool bChecked = this.ckListBoxUnblockUser.GetItemChecked(i);
                if (bChecked)
                {
                    this.ckListBoxUnblockUser.Items.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            string strUsers = "";
            string[] users = new string[this.ckListBoxUnblockUser.Items.Count];
            for (int i = 0; i < this.ckListBoxUnblockUser.Items.Count; i++)
            {
                users[i] = this.ckListBoxUnblockUser.Items[i].ToString();
            }
            strUsers = String.Join("\r\n", users);

            string strCMD = string.Format("{0}\t{1}\t{2}\t{3}",
                new object[] { "setUnblockUser", this.tbLoginUser.Text, this.cbProjectName.SelectedItem, strUsers });
            string strRet = DispatchCmd(strCMD);
            this.Log("接收结果:" + strRet + "\n", Color.DarkRed);
        }
    }
}
