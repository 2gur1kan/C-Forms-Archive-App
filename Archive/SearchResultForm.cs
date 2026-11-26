using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archive
{
    public partial class SearchResultForm : Form
    {
        public SearchResultForm()
        {
            InitializeComponent();
        }

        public List<Record> Results = new List<Record>(); // ArchiveMain doldurur, sıralama arama sırasında yapılmıştır

        private void SearchResultForm_Load(object sender, EventArgs e)
        {
            ArchiveMain.ApplyTheme(this);

            Text = "Sonuçlar (" + Results.Count + ")";
            Panel.Dock = DockStyle.Fill; // panel formun tüm alanını kaplar
            Panel.Padding = new Padding(ArchiveMain.pad);
            Panel.AutoScroll = true; // sonuç sığmazsa kaydırılabilsin

            // Dock.Top kartlar sondan başa dizilir, o yüzden ters sırayla eklenir ki liste sıralamayla aynı görünsün
            foreach (Record r in Enumerable.Reverse(Results)) Panel.Controls.Add(ArchiveMain.CreateTitleButton(r, Button_Click));
        }

        private void Button_Click(object sender, EventArgs e) => ArchiveMain.OpenData((string)((Button)sender).Tag, this);

        private void Panel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
