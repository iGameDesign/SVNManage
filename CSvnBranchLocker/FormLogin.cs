using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSvnBranchLocker
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            //this.tbPassword.Text = "57jieif*94Jkd1";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
        }
    }
}
