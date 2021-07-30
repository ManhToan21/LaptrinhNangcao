using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WinApp.Views
{
    class Led : Control
    {
        int _state;
        public int State
        {
            get => _state;
            set
            {
                _state = value;
                Invalidate();
            }
        }
        public Led()
        {
            this.Width = this.Height = 25;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var color = _state == 0 ? Color.LightGray : Color.LimeGreen;
            e.Graphics.FillEllipse(new SolidBrush(color), rect); 
            base.OnPaint(e);
        }
    }
}
