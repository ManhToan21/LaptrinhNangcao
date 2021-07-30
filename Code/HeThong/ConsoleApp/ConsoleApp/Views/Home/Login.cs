using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Views.Home
{
    class Login : BaseView<object>
    {
        protected override void RenderBody()
        {
            Caption("LOGIN");
            string un = Input("User name");
            string pw = Input("Password");

            Controller.Execute("login", un, pw);
        }
    }
}
