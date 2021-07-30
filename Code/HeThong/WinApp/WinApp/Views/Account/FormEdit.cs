using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Views.Account
{
    public partial class FormEdit : Form
    {
        public FormEdit()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            var acc = (Models.Account)this.Tag;
            this.textBox1.Text = acc.Id;
            this.textBox2.Text = acc.Role;

            this.Name = acc.Id == null ? "insert" : "update";
            base.OnLoad(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Engine.Execute("account/update", this.Name, this.textBox1.Text, this.textBox2.Text);
        }
    }
}
