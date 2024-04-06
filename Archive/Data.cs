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
    public partial class Data : Form
    {
        public Data()
        {
            InitializeComponent();
        }

        public string titleText;
        public string InfoText;

        private void Data_Load(object sender, EventArgs e)
        {
            Title.Text = titleText;
            Info.Text = InfoText;
        }
    }
}
