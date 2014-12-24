namespace CSvnBranchLocker
{
    partial class FormSvnBranchLocker
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbSvnServerIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSvnServerPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbLoginUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLoginIP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rtbLogInfo = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnLockBranches = new System.Windows.Forms.Button();
            this.ckListBoxBranches = new System.Windows.Forms.CheckedListBox();
            this.btnUpdateBranches = new System.Windows.Forms.Button();
            this.cbProjectName = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "SVN服务器地址：";
            // 
            // tbSvnServerIP
            // 
            this.tbSvnServerIP.Location = new System.Drawing.Point(114, 8);
            this.tbSvnServerIP.Name = "tbSvnServerIP";
            this.tbSvnServerIP.Size = new System.Drawing.Size(166, 21);
            this.tbSvnServerIP.TabIndex = 1;
            this.tbSvnServerIP.Text = "svn.funova.com";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(296, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口：";
            // 
            // tbSvnServerPort
            // 
            this.tbSvnServerPort.Location = new System.Drawing.Point(341, 9);
            this.tbSvnServerPort.Name = "tbSvnServerPort";
            this.tbSvnServerPort.Size = new System.Drawing.Size(100, 21);
            this.tbSvnServerPort.TabIndex = 3;
            this.tbSvnServerPort.Text = "18008";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(60, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "用户名：";
            // 
            // tbLoginUser
            // 
            this.tbLoginUser.Location = new System.Drawing.Point(114, 38);
            this.tbLoginUser.Name = "tbLoginUser";
            this.tbLoginUser.ReadOnly = true;
            this.tbLoginUser.Size = new System.Drawing.Size(166, 21);
            this.tbLoginUser.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(84, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "IP：";
            // 
            // tbLoginIP
            // 
            this.tbLoginIP.Location = new System.Drawing.Point(115, 67);
            this.tbLoginIP.Name = "tbLoginIP";
            this.tbLoginIP.ReadOnly = true;
            this.tbLoginIP.Size = new System.Drawing.Size(165, 21);
            this.tbLoginIP.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "选择主分支：";
            // 
            // rtbLogInfo
            // 
            this.rtbLogInfo.Location = new System.Drawing.Point(-1, 260);
            this.rtbLogInfo.Name = "rtbLogInfo";
            this.rtbLogInfo.Size = new System.Drawing.Size(501, 279);
            this.rtbLogInfo.TabIndex = 11;
            this.rtbLogInfo.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(72, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "项目：";
            // 
            // btnLockBranches
            // 
            this.btnLockBranches.Location = new System.Drawing.Point(298, 218);
            this.btnLockBranches.Name = "btnLockBranches";
            this.btnLockBranches.Size = new System.Drawing.Size(86, 23);
            this.btnLockBranches.TabIndex = 15;
            this.btnLockBranches.Text = "锁定所选分支";
            this.btnLockBranches.UseVisualStyleBackColor = true;
            this.btnLockBranches.Click += new System.EventHandler(this.btnLockBranches_Click);
            // 
            // ckListBoxBranches
            // 
            this.ckListBoxBranches.FormattingEnabled = true;
            this.ckListBoxBranches.Location = new System.Drawing.Point(117, 135);
            this.ckListBoxBranches.Name = "ckListBoxBranches";
            this.ckListBoxBranches.Size = new System.Drawing.Size(163, 116);
            this.ckListBoxBranches.TabIndex = 16;
            // 
            // btnUpdateBranches
            // 
            this.btnUpdateBranches.Location = new System.Drawing.Point(298, 145);
            this.btnUpdateBranches.Name = "btnUpdateBranches";
            this.btnUpdateBranches.Size = new System.Drawing.Size(86, 23);
            this.btnUpdateBranches.TabIndex = 17;
            this.btnUpdateBranches.Text = "更新分支信息";
            this.btnUpdateBranches.UseVisualStyleBackColor = true;
            this.btnUpdateBranches.Click += new System.EventHandler(this.btnUpdateBranches_Click);
            // 
            // cbProjectName
            // 
            this.cbProjectName.FormattingEnabled = true;
            this.cbProjectName.Location = new System.Drawing.Point(117, 100);
            this.cbProjectName.Name = "cbProjectName";
            this.cbProjectName.Size = new System.Drawing.Size(121, 20);
            this.cbProjectName.TabIndex = 18;
            // 
            // FormSvnBranchLocker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 539);
            this.Controls.Add(this.cbProjectName);
            this.Controls.Add(this.btnUpdateBranches);
            this.Controls.Add(this.ckListBoxBranches);
            this.Controls.Add(this.btnLockBranches);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rtbLogInfo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbLoginIP);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbLoginUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbSvnServerPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbSvnServerIP);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FormSvnBranchLocker";
            this.Text = "Svn Branch Locker";
            this.Load += new System.EventHandler(this.FormSvnBranchLocker_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSvnServerIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSvnServerPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbLoginUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbLoginIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox rtbLogInfo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnLockBranches;
        private System.Windows.Forms.CheckedListBox ckListBoxBranches;
        private System.Windows.Forms.Button btnUpdateBranches;
        private System.Windows.Forms.ComboBox cbProjectName;
    }
}

