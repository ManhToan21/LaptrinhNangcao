using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace WinApp.Views.Device
{
    class Default : BaseView<List<Models.DeviceViewModel>, FlowLayoutPanel>
    {
        protected override void RenderBody()
        {
            foreach (var device in Model)
            {
                var view = new DeviceView(device);
                MainContent.Controls.Add(view);
            }

            MainContent.Dock = DockStyle.Fill;
        }
    }
}
