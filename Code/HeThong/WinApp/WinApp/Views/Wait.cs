using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WinApp.Views
{
    class WaitApi : BaseView<WebRequest, Control>
    {
        const int one = 10;
        public override Control GetMainContent()
        {
            return null;
        }
        protected override void RenderBody()
        {
            Cursor.Current = Cursors.WaitCursor;


            bool stop = false;
            int count = 0;
            int ms = 1000 / one;
            Model.Responsed += r => {
                stop = true;
                if (r.Code < 0) {
                    Error(string.Format("Error {0}: {1}", -r.Code, r.Message));
                }
            };
            while (!stop)
            {
                if (count++ == one)
                {
                    count = 0;
                    Console.Write(".");
                }
                System.Threading.Thread.Sleep(ms);
            }
        }
    }
}
