using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Views.Home
{
    public partial class LoginBox : UserControl
    {
        public LoginBox()
        {
            InitializeComponent();
            this.button1.Click += (s, e) => {
                Engine.Execute("home/login", textBox1.Text, textBox2.Text);
            };
        }
    }

    class Login : BaseView<LoginBox>
    {

    }
}
