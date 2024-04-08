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
    public partial class SearchResultForm : Form
    {
        public SearchResultForm()
        {
            InitializeComponent();
        }

        public List<string> PartialTitle = new List<string>();
        public string path;

        private void SearchResultForm_Load(object sender, EventArgs e)
        {
            // Panelin formun tüm alanını kapladığını belirtin
            Panel.Dock = DockStyle.Fill;

            // Liste elemanları üzerinde döngü yapın
            foreach (string item in PartialTitle)
            {
                // Her bir liste elemanı için bir buton oluşturun
                Button button = new Button();
                button.Text = item; // Butonun metnini liste elemanına ayarlayın

                button.Dock = DockStyle.Top;
                button.AutoSize = true; // Otomatik boyutlandırmayı etkinleştirin
                button.TextAlign = ContentAlignment.MiddleLeft; // Metni sağa hizalayın

                // Butonun stilini ve çerçeve rengini ayarlayın
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.White; // Çerçeve rengini beyaz olarak ayarlayın
                button.BackColor = Color.White;

                // Butonun tıklama olayına dinamik olarak bir işlev ekleyin
                button.Click += Button_Click;

                // Butonu panele ekleyin
                Panel.Controls.Add(button);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender; // Buton nesnesine dönüştürün
            string buttonText = clickedButton.Text; // Butonun metnini alın

            Data Dataform = new Data();

            Dataform.titleText = buttonText;
            Dataform.path = path + buttonText;

            Dataform.Show();

        }


        private void Panel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
