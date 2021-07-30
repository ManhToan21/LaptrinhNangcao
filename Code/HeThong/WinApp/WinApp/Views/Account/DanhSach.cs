using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Views.Account
{
    public partial class DanhSach : UserControl
    {
        public DanhSach()
        {
            InitializeComponent();
            
            this.Dock = DockStyle.Fill;
            this.listView1.MultiSelect = false;
            this.listView1.View = View.Details;

            this.listView1.Columns.Add("User Name").Width = 200;
            this.listView1.Columns.Add("Role").Width = 100;
            this.listView1.Columns.Add("Password").Width = 250;
        }

        object _dataSource;
        public object DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                this.listView1.Items.Clear();

                foreach (var e in (IEnumerable<Models.Account>)value)
                {
                    var item = this.listView1.Items.Add(e.Id);
                    item.SubItems.Add(e.Role);
                    item.SubItems.Add(e.Password);
                    item.Tag = e;
                }
            }
        }
        public Models.Account SelectedItem
        {
            get
            {
                if (this.listView1.SelectedItems.Count == 0)
                    return null;
                return (Models.Account)this.listView1.SelectedItems[0].Tag;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var frm = new FormEdit() { 
                Tag = new Models.Account()
            };
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var frm = new FormEdit()
            {
                Tag = SelectedItem
            };
            frm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var item = SelectedItem;
            var accept = MessageBox.Show("Delete " + item.Id + "?", "", MessageBoxButtons.YesNo);
            if (accept == DialogResult.Yes)
            {
                Engine.Execute("account/update", "delete", item.Id, item.Role);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
