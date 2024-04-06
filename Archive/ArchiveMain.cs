using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Archive
{
    public partial class ArchiveMain : Form
    {
        public ArchiveMain()
        {
            InitializeComponent();
        }

        public static string archivePath = "Archive\\";
        public static string dataPath = "Data\\";
        public static string titleDataBaseName = "Titles.txt";

        public string titlePath;
        public List<Button> Shelfs = new List<Button>();

        private void ArchiveMain_Load(object sender, EventArgs e)
        {
            //butonlara hızlı erişim için
            Shelfs.Add(Shelf1);
            Shelfs.Add(Shelf2);
            Shelfs.Add(Shelf3);
            Shelfs.Add(Shelf4);

            //dosya adına sahip bir kolasör vermı ona bakar
            titlePath = Path.Combine(Environment.CurrentDirectory, archivePath + titleDataBaseName);

            try
            {
                // Dosya zaten varsa üzerine yazar, yoksa oluşturur
                if (!File.Exists(titlePath))
                {
                    // Dosya yoksa oluştur
                    using (FileStream fs = File.Create(titlePath)) { }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını göster
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // Dosyayı satır satır oku
                string[] titles = File.ReadAllLines(titlePath);

                int lenght = titles.Length;
                if (lenght > 4) lenght = 4;

                // Dosyadan okunan veri sayısı kadar butonların metin özelliklerini güncelle
                for (int i = 0; i < lenght && i < Controls.Count; i++)
                {
                    if (Controls[i] is Button button)
                    {
                        // Butonların metin özelliklerine dosyadan okunan veriyi ata
                        Shelfs[i].Text = titles[i];
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını göster
                MessageBox.Show("Hata: " + ex.Message, "Shelfs de sıkıntı var", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void New_Click(object sender, EventArgs e)
        {
            SaveForm New = new SaveForm();
            New.Show();
        }

        private void Search_Click(object sender, EventArgs e)
        {

        }

        private void Shelf1_Click(object sender, EventArgs e)
        {
            string[] datas = dataFinder(Shelf1.Text);

            Data Dataform = new Data();
            Dataform.titleText = datas[0];
            Dataform.InfoText = datas[1];

            Dataform.Show();
        }

        private string[] dataFinder(string title)
        {
            string DataPath = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath + title + ".txt");

            string[] Datas = File.ReadAllLines(DataPath);

            return Datas;
        }
    }
}
