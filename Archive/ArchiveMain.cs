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

        List<string> PartialTitle = new List<string>();

        private void ArchiveMain_Load(object sender, EventArgs e)
        {
            // Yeni formun başlangıç konumunu manuel olarak ayarlayın
            this.StartPosition = FormStartPosition.CenterScreen;

            Sizer();
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
        private void Sizer()
        {
            System.Drawing.Rectangle rec = Screen.PrimaryScreen.WorkingArea;
            this.Size = new System.Drawing.Size(Convert.ToInt32(.5f * rec.Width), Convert.ToInt32(.5f * rec.Height));
            this.Location = new System.Drawing.Point(10, 10);
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

            // Yeni formun başlangıç konumunu manuel olarak ayarlayın
            New.StartPosition = FormStartPosition.Manual;
            New.Location = this.Location;

            New.Show();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            string SearchTitle = SearchBar.Text;

            if (!string.IsNullOrEmpty(SearchTitle))
            {
                PartialTitle = new List<string>();

                // Arama yapılacak dosyanın yolunu belirtin (titlePath değişkeni)
                // Örneğin: string titlePath = "dosya_yolu.txt";

                if (File.Exists(titlePath))
                {
                    // Dosya mevcut ise, dosyadaki tüm satırları okuyun
                    string[] lines = File.ReadAllLines(titlePath);

                    // Aranan başlığın tam eşleşmesini arayın
                    bool isThere = lines.Any(line => line.Equals(SearchTitle));

                    if (isThere)
                    {
                        PartialTitle.Add(SearchTitle);
                    }
                    

                    {
                        // Arama metninden gereksiz boşlukları kaldırın
                        string trimmedSearchTitle = SearchTitle.Trim();

                        // Gereksiz boşlukları kaldırılmış arama metnini büyük-küçük harfe duyarlı olmayan bir şekilde arayın
                        List<string> partial = lines.Where(line => line.IndexOf(trimmedSearchTitle, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                        if (partial.Any())
                        {
                            foreach (var part in partial)
                            {
                                if(part != SearchTitle) PartialTitle.Add(part);
                            }
                        }

                        if (PartialTitle.Count <= 0)
                        {
                            // Herhangi bir eşleşme bulunamadı
                            MessageBox.Show("Aramanızla ilgili herhangi bir eşleşme bulunamadı.");
                        }
                        else
                        {
                            SearchResultForm result = new SearchResultForm();
                            result.PartialTitle = this.PartialTitle;
                            result.path = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath);

                            // Yeni formun başlangıç konumunu manuel olarak ayarlayın
                            result.StartPosition = FormStartPosition.Manual;
                            result.Location = this.Location;

                            result.Show();
                        }
                    }
                }
                else
                {
                    // Dosya mevcut değil ise
                    MessageBox.Show("Belirtilen dosya bulunamadı: " + titlePath);
                }
            }
            else
            {
                // Arama metni boş ise
                MessageBox.Show("Arama metni boş olamaz.");
            }
        }

        private void Shelf1_Click(object sender, EventArgs e)
        {

            Data Dataform = new Data();

            Dataform.titleText = Shelf1.Text;
            Dataform.path = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath + Shelf1.Text);

            // Yeni formun başlangıç konumunu manuel olarak ayarlayın
            Dataform.StartPosition = FormStartPosition.Manual;
            Dataform.Location = this.Location;

            Dataform.Show();
        }

        private void DetailedSearch_Click(object sender, EventArgs e)
        {
            DetailedSearchForm DSF = new DetailedSearchForm();

            DSF.tags = this.Tags;

            DSF.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
