using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Views.Account
{
    class Default : BaseView<string>
    {
        protected override void RenderBody()
        {
            Caption("Accounts");
            Menu(new Dictionary<string, string> {
                { "all", "List All" },
                { "create", "Create New" },
                { "edit", "Select For Update" },
                { "delete", "Select For Remove" },
                { "clear", "Refesh" },
            });
        }
    }
    class All : BaseView<List<Models.Account>>
    {
        protected override void RenderBody()
        {
            foreach (var e in Model)
            {
                Info(string.Format("{0, -20}{1}", e.Id, e.Role));
            }
            Controller.GoFirst();
        }
    }
    class Create : BaseView<Models.Account>
    {
        protected override void RenderBody()
        {
            var un = Input("User Name");
            var rl = Input("Role");
            Controller.Execute("update", "insert", un, rl);
        }
    }
    class Delete : BaseView<Models.Account>
    {
        protected override void RenderBody()
        {
            var un = Input("User Name");
            Controller.Execute("update", "delete", un, null);
        }
    }
    class Edit : BaseView<Models.Account>
    {
        protected override void RenderBody()
        {
            var un = Input("User Name");
            var rl = Input("Role");
            Controller.Execute("update", "update", un, rl);
        }
    }
}
