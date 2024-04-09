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
                button.Height = 100;
                button.AutoSize = true; // Otomatik boyutlandırmayı etkinleştirin
                button.TextAlign = ContentAlignment.MiddleLeft; // Metni sağa hizalayın

                // Butonun stilini ve çerçeve rengini ayarlayın
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.White; // Çerçeve rengini beyaz olarak ayarlayın
                button.BackColor = Color.White;

                //butona varsa resim ekle
                string imagePath = Path.Combine(path, item, "Images", "0.jpg");
                if (File.Exists(imagePath))
                {
                    // Resmi yükleyin
                    Image originalImage = Image.FromFile(imagePath);
                    // Resmi buton boyutuna ölçekleyin
                    Image scaledImage = originalImage.GetThumbnailImage(button.Width * 2, button.Height, null, IntPtr.Zero);
                    // Ölçeklenmiş resmi butona atayın
                    button.Image = scaledImage;
                    // Resmin butona sağ üst köşesine hizalayın
                    button.ImageAlign = ContentAlignment.MiddleRight;
                }

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

            // Yeni formun başlangıç konumunu manuel olarak ayarlayın
            Dataform.StartPosition = FormStartPosition.Manual;
            Dataform.Location = this.Location;

            Dataform.Show();

        }


        private void Panel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
