using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Views
{
    class DeviceView : GroupBox
    {
        public DeviceView(Models.DeviceViewModel device)
        {
            this.Text = device.Name;
            this.BackColor = Color.White;
            int w = 0;
            int x = 0, y = 10;
            foreach (var p in device.Status)
            {
                var led = new Led { Name = p.Key, State = p.Value };
                this.Controls.Add(led);

                if (w == 0)
                {
                    w = led.Width;
                    y += w >> 1;
                    x = w;
                }
                led.Left = x + (w >> 1);
                led.Top = y;

                x += w << 1;
            }
            this.Height = (w << 1) + y - 10;
            this.Width = x + (w >> 1);

            device.Changed += (d, v) => { 
                foreach (var p in d.Status)
                {
                    var led = (Led)this.Controls[p.Key];
                    led.State = p.Value;
                }
            };
        }
    }
}
