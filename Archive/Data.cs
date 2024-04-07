using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public string linkText;
        public string InfoText;

        private void Data_Load(object sender, EventArgs e)
        {
            Title.Text = titleText;
            Link.Text = linkText;
            Info.Text = InfoText;
        }

        private void Edit_Click(object sender, EventArgs e)
        {

        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(linkText)) Process.Start(linkText);
        }
    }
}
