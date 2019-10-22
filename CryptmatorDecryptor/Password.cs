using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptomatorDecryptor
{
    public partial class Password : Form
    {
        private Decryptor parent;

        public Password(Form parent)
        {
            InitializeComponent();
            this.parent = (Decryptor)parent;
            this.StartPosition = FormStartPosition.CenterParent;

        }

        private void Password_Load(object sender, EventArgs e)
        {
            lblVault.Text = parent.Vault;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            parent.Password = txtPassword.Text;
        }
    }
}
