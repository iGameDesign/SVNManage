namespace CGUIClient
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;

    public class FormClient : Form
    {
        private Button btnCreate;
        private ComboBox verComboBox;
        private Button btnLock;
        private Button btnQuery;
        private IContainer components = null;
        private ImageList imageList1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private OpenFileDialog openFileDialog1;
        private PictureBox picLocked;
        private PictureBox picUnlock;
        private RichTextBox rtxt;
        private TextBox txtBranchName;
        private TextBox txtIP;
        private TextBox txtLog;
        private TextBox txtPort;
        private TextBox txtSvrIP;
        private TextBox txtUserName;

        public FormClient()
        {
            this.InitializeComponent();
            this.txtBranchName.Focus();
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 1)
            {
                this.txtIP.Text = commandLineArgs[1];
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient client = new TcpClient();
                this.log(string.Format("连接[{0}:{1}]...", this.txtSvrIP.Text, this.txtPort.Text), Color.Blue);
                client.Connect(this.txtSvrIP.Text, int.Parse(this.txtPort.Text));
                this.log("已连接!", Color.DarkSlateBlue);
                string s = string.Format("{0}\t{1}\t{2}", "lockmasterbranch", this.picLocked.Visible ? "0" : "1", "2");
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                this.log("命令发送中....", Color.Green);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buffer = new byte[0x400];
                int num = stream.Read(buffer, 0, 0x400);
                this.log("接收结果:\r\n" + unicode.GetString(buffer) + "\r\n", Color.DarkRed);
                client.Close();
                this.UpdateLockStatu();
                MessageBox.Show(unicode.GetString(buffer), "注意", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception exception)
            {
                this.log(string.Format("Error:{0}", exception.Message), Color.Red);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.Enabled = false;
            this.Query();
            base.Enabled = true;
        }

        private void Commit(string strIP, string strUser, string strBranchName, string strMainVer)
        {
            try
            {
                TcpClient client = new TcpClient();
                this.log(string.Format("连接[{0}:{1}]...", this.txtSvrIP.Text, this.txtPort.Text), Color.Blue);
                client.Connect(this.txtSvrIP.Text, int.Parse(this.txtPort.Text));
                this.log("已连接!", Color.DarkSlateBlue);
                string s = string.Format("createbranch\t{0}\t{1}\t{2}\t{3}", strIP, strUser, strBranchName, strMainVer);
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                this.log("申请发送中....", Color.Green);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buffer = new byte[0x400];
                int num = stream.Read(buffer, 0, 0x400);
                this.log("接收申请结果:" + unicode.GetString(buffer), Color.DarkRed);
                client.Close();
                if (unicode.GetString(buffer).IndexOf("失败") >= 0)
                {
                    this.txtBranchName.Select();
                    this.txtBranchName.SelectAll();
                }
            }
            catch (Exception exception)
            {
                this.log(string.Format("Error:{0}", exception.Message), Color.Red);
            }
        }

        private void Create_Click(object sender, EventArgs e)
        {
            if (checkVerName(this.verComboBox.Text) == false)
            {
                MessageBox.Show("请选择一个语言的主版本！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (this.txtBranchName.Text.Length <= 0)
            {
                MessageBox.Show("请输入正确的分支名称！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                this.txtBranchName.Focus();
            }
            else if ((this.txtSvrIP.Text.Length <= 0) || (this.txtPort.Text.Length <= 0))
            {
                MessageBox.Show("请输入正确的SVN服务器地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                base.Enabled = false;
                this.Commit(this.txtIP.Text, this.txtUserName.Text, this.txtBranchName.Text, this.verComboBox.Text);
                base.Enabled = true;
            }
        }

        private Boolean checkVerName(String strVer)
        {
            if (strVer != "")
            {
                foreach(String s in this.verComboBox.Items)
                {
                    if (s == strVer)
                        return true;
                }
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.txtUserName.Text = Environment.MachineName;
            this.txtIP.Text = Dns.Resolve(this.txtUserName.Text).AddressList[0].ToString();
        }

        private void FormClient_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Q) && e.Control)
            {
                this.btnCreate.Visible = !this.btnCreate.Visible;
                this.btnCreate.Enabled = !this.btnCreate.Enabled;
                this.btnQuery.Visible = !this.btnCreate.Visible;
                this.txtBranchName.Text = "";
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(FormClient));
            this.imageList1 = new ImageList(this.components);
            this.btnCreate = new Button();
            this.verComboBox = new System.Windows.Forms.ComboBox();
            this.openFileDialog1 = new OpenFileDialog();
            this.txtUserName = new TextBox();
            this.label1 = new Label();
            this.txtLog = new TextBox();
            this.label2 = new Label();
            this.txtIP = new TextBox();
            this.txtSvrIP = new TextBox();
            this.label3 = new Label();
            this.label4 = new Label();
            this.txtBranchName = new TextBox();
            this.label5 = new Label();
            this.txtPort = new TextBox();
            this.label6 = new Label();
            this.rtxt = new RichTextBox();
            this.btnQuery = new Button();
            this.btnLock = new Button();
            this.picUnlock = new PictureBox();
            this.picLocked = new PictureBox();
            ((ISupportInitialize) this.picUnlock).BeginInit();
            ((ISupportInitialize) this.picLocked).BeginInit();
            base.SuspendLayout();
            this.imageList1.ImageStream = (ImageListStreamer) manager.GetObject("imageList1.ImageStream");
            this.imageList1.TransparentColor = Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "20120826095130249_easyicon_cn_48.ico");
            this.imageList1.Images.SetKeyName(1, "20120826095555921_easyicon_cn_48.png");
            this.imageList1.Images.SetKeyName(2, "20120829031430308_easyicon_cn_24.ico");
            this.imageList1.Images.SetKeyName(3, "20120829031710387_easyicon_cn_32.ico");
            this.imageList1.Images.SetKeyName(4, "20120829031500314_easyicon_cn_48.ico");
            this.imageList1.Images.SetKeyName(5, "2014022803405638_easyicon_net_80.png");
            this.imageList1.Images.SetKeyName(6, "20140228034109988_easyicon_net_80.png");
            this.btnCreate.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnCreate.DialogResult = DialogResult.OK;
            this.btnCreate.ImageAlign = ContentAlignment.MiddleLeft;
            this.btnCreate.ImageList = this.imageList1;
            this.btnCreate.Location = new Point(0x189, 0x6f);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new Size(0xb2, 0x36 / 2);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "提交创建申请";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new EventHandler(this.Create_Click);
            this.verComboBox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.verComboBox.FormattingEnabled = true;
            this.verComboBox.Items.AddRange(new object[] { "CN_MAIN", "EN_MAIN" });
            this.verComboBox.Location = new System.Drawing.Point(0x189, 0x6f + 0x23);
            this.verComboBox.Name = "verComboBox";
            this.verComboBox.Size = new System.Drawing.Size(121, 20);
            this.verComboBox.TabIndex = 0;
            this.verComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.openFileDialog1.FileName = "authz";
            this.txtUserName.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.txtUserName.Location = new Point(0x3d, 0x6f);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new Size(0x131, 0x15);
            this.txtUserName.TabIndex = 2;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(14, 0x72);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "用户名";
            this.txtLog.Location = new Point(12, 0xb6);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new Size(560, 330);
            this.txtLog.TabIndex = 2;
            this.txtLog.Visible = false;
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x26, 0x93);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x11, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "IP";
            this.txtIP.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.txtIP.Location = new Point(0x3d, 0x90);
            this.txtIP.Name = "txtIP";
            this.txtIP.ReadOnly = true;
            this.txtIP.Size = new Size(0x131, 0x15);
            this.txtIP.TabIndex = 2;
            this.txtSvrIP.Location = new Point(0x65, 0x17);
            this.txtSvrIP.Name = "txtSvrIP";
            this.txtSvrIP.Size = new Size(0xbb, 0x15);
            this.txtSvrIP.TabIndex = 1;
            this.txtSvrIP.Text = "192.168.1.242";
            this.label3.AutoSize = true;
            this.label3.Location = new Point(12, 0x1a);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x53, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "SVN服务器地址";
            this.label4.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.label4.BorderStyle = BorderStyle.Fixed3D;
            this.label4.Location = new Point(0x10, 0x35);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x22c, 2);
            this.label4.TabIndex = 4;
            this.txtBranchName.AccessibleRole = AccessibleRole.Equation;
            this.txtBranchName.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.txtBranchName.Location = new Point(0x3d, 0x45);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new Size(0x1ff, 0x15);
            this.txtBranchName.TabIndex = 0;
            this.label5.AutoSize = true;
            this.label5.Location = new Point(14, 0x48);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x29, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "分支名";
            this.txtPort.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.txtPort.Location = new Point(0x15f, 0x17);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new Size(0xdd, 0x15);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "18008";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(0x130, 0x1a);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x29, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "端口号";
            this.rtxt.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.rtxt.Location = new Point(12, 0xb6);
            this.rtxt.Name = "rtxt";
            this.rtxt.ReadOnly = true;
            this.rtxt.Size = new Size(0x22f, 330);
            this.rtxt.TabIndex = 5;
            this.rtxt.Text = "";
            this.btnQuery.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnQuery.DialogResult = DialogResult.OK;
            this.btnQuery.ImageAlign = ContentAlignment.MiddleLeft;
            this.btnQuery.Location = new Point(0x189, 0x6f);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new Size(0xb2, 0x36);
            this.btnQuery.TabIndex = 3;
            this.btnQuery.Text = "查询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Visible = false;
            this.btnQuery.Click += new EventHandler(this.button1_Click);
            this.btnLock.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnLock.DialogResult = DialogResult.OK;
            this.btnLock.ImageAlign = ContentAlignment.MiddleLeft;
            this.btnLock.ImageList = this.imageList1;
            this.btnLock.Location = new Point(0x189, 0x6f);
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new Size(0xb2, 0x36);
            this.btnLock.TabIndex = 6;
            this.btnLock.Text = "锁定主分支";
            this.btnLock.UseVisualStyleBackColor = true;
            this.btnLock.Visible = false;
            this.btnLock.Click += new EventHandler(this.btnLock_Click);
            this.picUnlock.Image = (Image) manager.GetObject("picUnlock.Image");
            this.picUnlock.Location = new Point(0x189, 0x3a);
            this.picUnlock.Name = "picUnlock";
            this.picUnlock.Size = new Size(0x2c, 0x31);
            this.picUnlock.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picUnlock.TabIndex = 7;
            this.picUnlock.TabStop = false;
            this.picUnlock.Visible = false;
            this.picLocked.Image = (Image) manager.GetObject("picLocked.Image");
            this.picLocked.Location = new Point(0x189, 0x3a);
            this.picLocked.Name = "picLocked";
            this.picLocked.Size = new Size(0x2c, 0x31);
            this.picLocked.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picLocked.TabIndex = 7;
            this.picLocked.TabStop = false;
            this.picLocked.Visible = false;
            base.AcceptButton = this.btnCreate;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x247, 0x20e);
            base.Controls.Add(this.picLocked);
            base.Controls.Add(this.picUnlock);
            base.Controls.Add(this.btnLock);
            base.Controls.Add(this.rtxt);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label6);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.txtLog);
            base.Controls.Add(this.txtIP);
            base.Controls.Add(this.txtPort);
            base.Controls.Add(this.txtSvrIP);
            base.Controls.Add(this.txtBranchName);
            base.Controls.Add(this.txtUserName);
            base.Controls.Add(this.btnCreate);
            base.Controls.Add(this.verComboBox);
            base.Controls.Add(this.btnQuery);
            this.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            base.KeyPreview = true;
            this.MinimumSize = new Size(0x251, 0x22e);
            base.Name = "FormClient";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "SVN操作(1.2)";
            base.Load += new EventHandler(this.Form1_Load);
            base.KeyDown += new KeyEventHandler(this.FormClient_KeyDown);
            ((ISupportInitialize) this.picUnlock).EndInit();
            ((ISupportInitialize) this.picLocked).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void log(string strlog, Color clr)
        {
            this.rtxt.SelectionLength = 0;
            this.rtxt.SelectionColor = clr;
			string logstr = string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), strlog, "\r\n");
			this.rtxt.AppendText(logstr);
            this.rtxt.Refresh();
            this.rtxt.ScrollToCaret();
			this.txtLog.Text = this.txtLog.Text + logstr;
            this.txtLog.Refresh();
        }

        private void Query()
        {
            try
            {
                TcpClient client = new TcpClient();
                this.log(string.Format("连接[{0}:{1}]...", this.txtSvrIP.Text, this.txtPort.Text), Color.Blue);
                client.Connect(this.txtSvrIP.Text, int.Parse(this.txtPort.Text));
                this.log("已连接!", Color.DarkSlateBlue);
                string s = string.Format("{0}\t{1}\t{2}", "getRightsBranches", "1", "2");
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                this.log("命令发送中....", Color.Green);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buffer = new byte[0x400];
                int num = stream.Read(buffer, 0, 0x400);
                this.log("接收申请结果:\r\n" + unicode.GetString(buffer), Color.DarkRed);
                client.Close();
                if (unicode.GetString(buffer).IndexOf("失败") >= 0)
                {
                    this.txtBranchName.Select();
                    this.txtBranchName.SelectAll();
                }
            }
            catch (Exception exception)
            {
                this.log(string.Format("Error:{0}", exception.Message), Color.Red);
            }
        }

        private int QueryLockStatus()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(this.txtSvrIP.Text, int.Parse(this.txtPort.Text));
                string s = string.Format("{0}\t{1}\t{2}", "getlockstatu", "1", "2");
                Stream stream = client.GetStream();
                byte[] bytes = new ASCIIEncoding().GetBytes(s);
                Encoding unicode = Encoding.Unicode;
                bytes = unicode.GetBytes(s);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buffer = new byte[0x400];
                int num = stream.Read(buffer, 0, 0x400);
                client.Close();
                return int.Parse(unicode.GetString(buffer));
            }
            catch (Exception exception)
            {
                this.log(string.Format("Error:{0}", exception.Message), Color.Red);
                return 0;
            }
        }

        private void UpdateLockStatu()
        {
            if (this.QueryLockStatus() == 1)
            {
                this.picLocked.Visible = true;
                this.picUnlock.Visible = !this.picLocked.Visible;
                this.btnLock.Text = "解锁主分支";
            }
            else
            {
                this.picLocked.Visible = false;
                this.picUnlock.Visible = !this.picLocked.Visible;
                this.btnLock.Text = "锁定主分支";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}

