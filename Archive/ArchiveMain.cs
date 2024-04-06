using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archive
{
    public partial class ArchiveMain : Form
    {
        public ArchiveMain()
        {
            InitializeComponent();
        }

        private void ArchiveMain_Load(object sender, EventArgs e)
        {

        }

        private void New_Click(object sender, EventArgs e)
        {
            SaveForm New = new SaveForm();
            New.Show();
        }
    }
}
