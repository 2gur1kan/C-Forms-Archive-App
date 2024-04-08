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
using System.IO;

namespace Archive
{
    public partial class Data : Form
    {
        public Data()
        {
            InitializeComponent();
        }

        public string titleText;
        public string path;

        string[] dataTexts;
        int imageNum = 0;

        private void Data_Load(object sender, EventArgs e)
        {
            Sizer();

            dataTexts = dataFinder();

            //başlığı yazar
            Title.Text = dataTexts[0];
            //linki yazar
            Link.Text = dataTexts[1];

            // infodaki verileir yazar
            Info.Text = infoFinder();

            // varsa ilk resmi açar
            if (File.Exists(path + "\\Images\\" + "0.jpg"))
            {
                ShowImage(0);
                if(!File.Exists(path + "\\Images\\" + "1.jpg")) NextImage.Enabled = false;
                BackImage.Enabled = false;
            }
            else
            {
                BackImage.Enabled = false;
                NextImage.Enabled = false;
            }
        }

        private void Sizer()
        {
            System.Drawing.Rectangle rec = Screen.PrimaryScreen.WorkingArea;
            this.Size = new System.Drawing.Size(Convert.ToInt32(.5f * rec.Width), Convert.ToInt32(.5f * rec.Height));
            this.Location = new System.Drawing.Point(10, 10);
        }

        private void Edit_Click(object sender, EventArgs e)
        {

        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(dataTexts[1])) Process.Start(dataTexts[1]);
        }

        private string[] dataFinder()
        {
            string DataPath = path + "\\Data.txt";

            string[] Datas = File.ReadAllLines(DataPath);

            return Datas;
        }

        private string infoFinder()
        {
            string DataPath = path + "\\Info.txt";

            string Datas = File.ReadAllText(DataPath);

            return Datas;
        }

        private void ShowImage(int gg)
        {
            Picture.SizeMode = PictureBoxSizeMode.Zoom;
            // seçilen resmi açar
            Picture.Image = Image.FromFile(path + "\\Images\\" + gg.ToString() + ".jpg");
        }

        private void BackImage_Click(object sender, EventArgs e)
        {
            if (File.Exists(path + "\\Images\\" + (imageNum - 1).ToString() + ".jpg"))
            {
                imageNum--;
                ShowImage(imageNum);
                NextImage.Enabled = true;

                if (!File.Exists(path + "\\Images\\" + (imageNum - 1).ToString() + ".jpg")) BackImage.Enabled = false;
            }
        }

        private void NextImage_Click(object sender, EventArgs e)
        {
            if(File.Exists(path + "\\Images\\" + (imageNum + 1).ToString() + ".jpg"))
            {
                imageNum++;
                ShowImage(imageNum);
                BackImage.Enabled = true;

                if (!File.Exists(path + "\\Images\\" + (imageNum + 1).ToString() + ".jpg")) NextImage.Enabled = false;
            }
        }
    }
}
