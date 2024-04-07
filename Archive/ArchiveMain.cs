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
        public static string tagsDataBaseName = "Tags.txt";

        public string titlePath;
        public string tagsPath;
        public List<string> Tags = new List<string>();
        public List<Button> Shelfs = new List<Button>();

        private void ArchiveMain_Load(object sender, EventArgs e)
        {
            AreRegistrationPlacesActive();

            //butonlara hızlı erişim için
            {
                Shelfs.Add(Shelf1);
                Shelfs.Add(Shelf2);
                Shelfs.Add(Shelf3);
                Shelfs.Add(Shelf4);
            }

            //title dosya adına sahip bir dosya vermı ona bakar
            {
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
            }

            //tagleri listeye çeker
            {
                tagsPath = Path.Combine(Environment.CurrentDirectory, archivePath + tagsDataBaseName);

                try
                {
                    // Dosya yoksa oluşturur
                    if (!File.Exists(tagsPath))
                    {
                        using (FileStream fs = File.Create(tagsPath)){ }

                        using (StreamWriter sw = new StreamWriter(tagsPath, true))
                        {
                            // Başlığı dosyaya yaz
                            sw.WriteLine("2016");
                            sw.WriteLine("2017");
                            sw.WriteLine("2018");
                            sw.WriteLine("2019");
                            sw.WriteLine("2020");
                        }
                    }

                    // Dosyayı satır satır oku ve her satırı List<string> koleksiyonuna ekle
                    using (StreamReader sr = new StreamReader(tagsPath))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            Tags.Add(line);
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Hata durumunda hata mesajını göster
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

            //önerilenlere 4 tane arşivden bilgi veriir
            {
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
            

        }

        private void AreRegistrationPlacesActive()
        {
            string SavePath = Path.Combine(Environment.CurrentDirectory, archivePath);

            try
            {
                // Klasör zaten varsa oluşturma
                if (!Directory.Exists(SavePath))
                {
                    // Klasörü oluştur
                    Directory.CreateDirectory(SavePath);

                    // Resimler klasörü oluştur
                    Directory.CreateDirectory(SavePath + dataPath);

                    MessageBox.Show("Veri kayıt klasörü başarıyla oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

        private void New_Click(object sender, EventArgs e)
        {
            SaveForm New = new SaveForm();
            New.tags = this.Tags;
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
            Dataform.linkText = datas[1];

            Dataform.InfoText = infoFinder(Shelf1.Text);

            Dataform.Show();
        }

        private string[] dataFinder(string title)
        {
            string DataPath = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath + title + "\\Data.txt");

            string[] Datas = File.ReadAllLines(DataPath);

            return Datas;
        }

        private string infoFinder(string title)
        {
            string DataPath = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath + title + "\\Info.txt");

            string Datas = File.ReadAllText(DataPath);

            return Datas;
        }
    }
}
