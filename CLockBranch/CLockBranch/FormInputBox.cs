namespace CLockBranch
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class FormInputBox : Form
    {
        private Button btnLogin;
        private IContainer components = null;
        private Label label1;
        public TextBox txtboxPasword;

        public FormInputBox()
        {
            this.InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnLogin = new Button();
            this.txtboxPasword = new TextBox();
            this.label1 = new Label();
            base.SuspendLayout();
            this.btnLogin.Location = new Point(0x89, 0x60);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(0x69, 0x24);
            this.btnLogin.TabIndex = 1;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            this.txtboxPasword.Location = new Point(0x79, 0x30);
            this.txtboxPasword.Name = "txtboxPasword";
            this.txtboxPasword.PasswordChar = '*';
            this.txtboxPasword.Size = new Size(0x9a, 0x15);
            this.txtboxPasword.TabIndex = 0;
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x56, 0x33);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x1d, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "密码";
            base.AcceptButton = this.btnLogin;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x17a, 160);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.txtboxPasword);
            base.Controls.Add(this.btnLogin);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FormInputBox";
            base.ShowInTaskbar = false;
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "请输入登录密码";
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}

