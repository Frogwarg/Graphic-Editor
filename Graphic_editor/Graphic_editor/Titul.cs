using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphic_editor
{
    public partial class Titul : Form
    {
        public Titul()
        {
            InitializeComponent();
        }

        private void Titul_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Visible = false;
            }
        }

        private void Titul_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
