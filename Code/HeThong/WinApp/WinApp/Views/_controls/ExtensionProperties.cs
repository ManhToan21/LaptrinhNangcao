using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Forms
{
    public class ExtensionControl<T> : Control where T: Control, new()
    {
        protected Control _content;
        public T Content => (T)_content;
        public ExtensionControl()
        {
            _content = new T();
        }
        public ExtensionControl(Control control)
        {
            _content = control;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.Parent != null)
            {
                if (_content.Parent == null)
                {
                    this.Text = "Extention " + typeof(T).Name;
                    _content.Parent = this;
                    _content.Dock = DockStyle.Fill;
                }
                this.Dock = DockStyle.Fill;
            }
        }

        int _eventFlags = 0;
        void Set(int flag) { _eventFlags |= 1 << flag; }
        void Reset(int flag) { _eventFlags &= ~(1 << flag); }
        void Invert(int flag) { _eventFlags ^= 1 << flag; }
        bool Has(int flag) { return (_eventFlags & (1 << flag)) != 0; }

        new public event Action Click;
        protected virtual void CreateClickEvent()
        {
            MouseDown += (s, e) => {
                Invert(1);
            };
            MouseUp += (s, e) => { 
                if (Has(1)) { Click?.Invoke(); Reset(1); }
            };
            MouseLeave += (s, e) => { Reset(1); };
        }
        string _url;
        public string Url 
        {
            get => _url;
            set
            {
                _url = value;
                CreateClickEvent();
            }
        }

        new string Text
        {
            get => _content.Text;
            set => _content.Text = value;
        }
    }

    public class MyLabel : ExtensionControl<Label>
    {
        public MyLabel()
        {
            _content.BackColor = System.Drawing.Color.Black;
            _content.ForeColor = System.Drawing.Color.White;
            _content.Font = new Drawing.Font(this.Font.FontFamily, 13);
            Content.TextAlign = Drawing.ContentAlignment.MiddleLeft;
        }
    }
}
