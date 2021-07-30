using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Views.Account
{
    class Default : BaseView<IEnumerable<Models.Account>, DanhSach>
    {
        protected override void RenderBody()
        {
            MainContent.DataSource = Model;
        }
    }
}
