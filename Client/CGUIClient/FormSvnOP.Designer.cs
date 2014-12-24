namespace CGUIClient
{
	partial class FormSvnOP
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtBoxIP = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.lblBranch = new System.Windows.Forms.Label();
            this.txtBoxBranch = new System.Windows.Forms.TextBox();
            this.lblLocal = new System.Windows.Forms.Label();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.lblLocalIP = new System.Windows.Forms.Label();
            this.txtLocalIP = new System.Windows.Forms.TextBox();
            this.btnCommit = new System.Windows.Forms.Button();
            this.lblSelTip = new System.Windows.Forms.Label();
            this.cbSelRoot = new System.Windows.Forms.ComboBox();
            this.txtBoxInfo = new System.Windows.Forms.RichTextBox();
            this.btnRefreshRoots = new System.Windows.Forms.Button();
            this.chkRoot = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbProjectName = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(6, 20);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(65, 12);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "SVN服务器:";
            // 
            // txtBoxIP
            // 
            this.txtBoxIP.Location = new System.Drawing.Point(74, 16);
            this.txtBoxIP.Name = "txtBoxIP";
            this.txtBoxIP.Size = new System.Drawing.Size(132, 21);
            this.txtBoxIP.TabIndex = 1;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(225, 19);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(47, 12);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "端口号:";
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(275, 16);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(76, 21);
            this.txtBoxPort.TabIndex = 3;
            // 
            // lblBranch
            // 
            this.lblBranch.AutoSize = true;
            this.lblBranch.Location = new System.Drawing.Point(24, 74);
            this.lblBranch.Name = "lblBranch";
            this.lblBranch.Size = new System.Drawing.Size(47, 12);
            this.lblBranch.TabIndex = 4;
            this.lblBranch.Text = "分支名:";
            // 
            // txtBoxBranch
            // 
            this.txtBoxBranch.Location = new System.Drawing.Point(74, 71);
            this.txtBoxBranch.Name = "txtBoxBranch";
            this.txtBoxBranch.Size = new System.Drawing.Size(277, 21);
            this.txtBoxBranch.TabIndex = 1;
            // 
            // lblLocal
            // 
            this.lblLocal.AutoSize = true;
            this.lblLocal.Location = new System.Drawing.Point(362, 20);
            this.lblLocal.Name = "lblLocal";
            this.lblLocal.Size = new System.Drawing.Size(47, 12);
            this.lblLocal.TabIndex = 5;
            this.lblLocal.Text = "用户名:";
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(411, 16);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.ReadOnly = true;
            this.txtBoxName.Size = new System.Drawing.Size(132, 21);
            this.txtBoxName.TabIndex = 1;
            // 
            // lblLocalIP
            // 
            this.lblLocalIP.AutoSize = true;
            this.lblLocalIP.Location = new System.Drawing.Point(555, 20);
            this.lblLocalIP.Name = "lblLocalIP";
            this.lblLocalIP.Size = new System.Drawing.Size(23, 12);
            this.lblLocalIP.TabIndex = 7;
            this.lblLocalIP.Text = "IP:";
            // 
            // txtLocalIP
            // 
            this.txtLocalIP.Location = new System.Drawing.Point(580, 16);
            this.txtLocalIP.Name = "txtLocalIP";
            this.txtLocalIP.ReadOnly = true;
            this.txtLocalIP.Size = new System.Drawing.Size(103, 21);
            this.txtLocalIP.TabIndex = 6;
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(365, 69);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(121, 23);
            this.btnCommit.TabIndex = 8;
            this.btnCommit.Text = "提交创建申请";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // lblSelTip
            // 
            this.lblSelTip.AutoSize = true;
            this.lblSelTip.Location = new System.Drawing.Point(221, 48);
            this.lblSelTip.Name = "lblSelTip";
            this.lblSelTip.Size = new System.Drawing.Size(71, 12);
            this.lblSelTip.TabIndex = 4;
            this.lblSelTip.Text = "选择主分支:";
            // 
            // cbSelRoot
            // 
            this.cbSelRoot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelRoot.FormattingEnabled = true;
            this.cbSelRoot.Location = new System.Drawing.Point(294, 44);
            this.cbSelRoot.Name = "cbSelRoot";
            this.cbSelRoot.Size = new System.Drawing.Size(132, 20);
            this.cbSelRoot.TabIndex = 9;
            // 
            // txtBoxInfo
            // 
            this.txtBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxInfo.Location = new System.Drawing.Point(14, 98);
            this.txtBoxInfo.Name = "txtBoxInfo";
            this.txtBoxInfo.ReadOnly = true;
            this.txtBoxInfo.Size = new System.Drawing.Size(669, 448);
            this.txtBoxInfo.TabIndex = 11;
            this.txtBoxInfo.Text = "";
            // 
            // btnRefreshRoots
            // 
            this.btnRefreshRoots.Location = new System.Drawing.Point(431, 42);
            this.btnRefreshRoots.Name = "btnRefreshRoots";
            this.btnRefreshRoots.Size = new System.Drawing.Size(113, 23);
            this.btnRefreshRoots.TabIndex = 12;
            this.btnRefreshRoots.Text = "<<更新主分支列表";
            this.btnRefreshRoots.UseVisualStyleBackColor = true;
            this.btnRefreshRoots.Click += new System.EventHandler(this.btnRefreshRoots_Click);
            // 
            // chkRoot
            // 
            this.chkRoot.AutoSize = true;
            this.chkRoot.Location = new System.Drawing.Point(492, 74);
            this.chkRoot.Name = "chkRoot";
            this.chkRoot.Size = new System.Drawing.Size(96, 16);
            this.chkRoot.TabIndex = 13;
            this.chkRoot.Text = "创建为主分支";
            this.chkRoot.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "项目:";
            // 
            // cbProjectName
            // 
            this.cbProjectName.FormattingEnabled = true;
            this.cbProjectName.Location = new System.Drawing.Point(74, 45);
            this.cbProjectName.Name = "cbProjectName";
            this.cbProjectName.Size = new System.Drawing.Size(121, 20);
            this.cbProjectName.TabIndex = 15;
            // 
            // FormSvnOP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 558);
            this.Controls.Add(this.cbProjectName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkRoot);
            this.Controls.Add(this.btnRefreshRoots);
            this.Controls.Add(this.txtBoxInfo);
            this.Controls.Add(this.cbSelRoot);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.lblLocalIP);
            this.Controls.Add(this.txtLocalIP);
            this.Controls.Add(this.lblLocal);
            this.Controls.Add(this.lblSelTip);
            this.Controls.Add(this.lblBranch);
            this.Controls.Add(this.txtBoxPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtBoxName);
            this.Controls.Add(this.txtBoxBranch);
            this.Controls.Add(this.txtBoxIP);
            this.Controls.Add(this.lblAddress);
            this.MinimumSize = new System.Drawing.Size(711, 596);
            this.Name = "FormSvnOP";
            this.Text = "SVN操作(2.0)";
            this.Load += new System.EventHandler(this.FormSvnOP_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblAddress;
		private System.Windows.Forms.TextBox txtBoxIP;
		private System.Windows.Forms.Label lblPort;
		private System.Windows.Forms.TextBox txtBoxPort;
		private System.Windows.Forms.Label lblBranch;
		private System.Windows.Forms.TextBox txtBoxBranch;
		private System.Windows.Forms.Label lblLocal;
		private System.Windows.Forms.TextBox txtBoxName;
		private System.Windows.Forms.Label lblLocalIP;
		private System.Windows.Forms.TextBox txtLocalIP;
		private System.Windows.Forms.Button btnCommit;
		private System.Windows.Forms.Label lblSelTip;
		private System.Windows.Forms.ComboBox cbSelRoot;
		private System.Windows.Forms.RichTextBox txtBoxInfo;
		private System.Windows.Forms.Button btnRefreshRoots;
		private System.Windows.Forms.CheckBox chkRoot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbProjectName;
	}
}