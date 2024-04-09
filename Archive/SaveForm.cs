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
    public partial class SaveForm : Form
    {
        public SaveForm()
        {
            InitializeComponent();
        }

        // Dosya adını belirle ve dosya yolunu oluştur
        string archivePath = ArchiveMain.archivePath;
        string dataPath = ArchiveMain.dataPath;
        string titleDataBaseName = ArchiveMain.titleDataBaseName;
        public List<string> tags = new List<string>();

        string titlePath;
        string[] selectedImages;
        List<string> selectedTags = new List<string>();

        private void SaveForm_Load(object sender, EventArgs e)
        {
            Sizer();

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

            foreach (string tag in tags)
            {
                Tags.Items.Add(tag);
            }

            //set rate
            string[] ranges = new string[] { "0", "1", "2", "3", "4", "5" };
            Rate.Items.AddRange(ranges);
            Rate.DropDownStyle = ComboBoxStyle.DropDownList;
            Rate.Text = "0";


        } 

        private void Sizer()
        {
            System.Drawing.Rectangle rec = Screen.PrimaryScreen.WorkingArea;
            this.Size = new System.Drawing.Size(Convert.ToInt32(.5f * rec.Width), Convert.ToInt32(.5f * rec.Height));
            this.Location = new System.Drawing.Point(10, 10);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try // başlığı sisteme kaydeder (sonradan arama yparken kolaylık olsun diye)
            {
                if (!string.IsNullOrWhiteSpace(Title.Text) && Title.Text.Length >= 3) // veri 3 karkaterden fazla karaktere sahip olmalı
                {
                    if (!IsThereATitle(Title.Text)) //veri dosyada varmı kontrol et
                    {
                        // StreamWriter nesnesi oluştur ve dosyayı aç
                        using (StreamWriter sw = new StreamWriter(titlePath, true))
                        {
                            // Veriyi dosyaya yaz
                            sw.WriteLine(Title.Text);
                        }

                        try // kalan veriyi kaydeden kısım 
                        {
                            string DataPath = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath + Title.Text);

                            try
                            {
                                // Klasör zaten varsa oluşturma
                                if (!Directory.Exists(DataPath))
                                {
                                    // Klasörü oluştur
                                    Directory.CreateDirectory(DataPath);

                                    // verilerin kayıtı
                                    {
                                        File.WriteAllText(DataPath + "\\Data.txt", string.Empty);//dosya yazılı ise önceden sıfırlar

                                        // StreamWriter nesnesi oluştur ve dosyayı aç
                                        using (StreamWriter sw = new StreamWriter(DataPath + "\\Data.txt", true))
                                        {
                                            // rate i kaydet
                                            sw.WriteLine(Rate.Text);

                                            // Başlığı dosyaya yaz
                                            sw.WriteLine(Title.Text);

                                            if(Link.Text != "Link")
                                            {
                                                // linki kaydet
                                                sw.WriteLine(Link.Text);
                                            }
                                            else
                                            {
                                                // link yazılmamışsa boşluk yaz
                                                sw.WriteLine("                                                                    ");
                                            }
                                        }
                                    }

                                    // Info txt kayıt
                                    {
                                        File.WriteAllText(DataPath + "\\Info.txt", string.Empty);//dosya yazılı ise önceden sıfırlar

                                        // StreamWriter nesnesi oluştur ve dosyayı aç
                                        using (StreamWriter sw = new StreamWriter(DataPath + "\\Info.txt", true))
                                        {
                                            // açıklamayı dosyaya yaz
                                            sw.WriteLine(Info.Text);
                                        }
                                    }

                                    // tagleri kaydeder
                                    {
                                        File.WriteAllText(DataPath + "\\Tags.txt", string.Empty);//dosya yazılı ise önceden sıfırlar

                                        // StreamWriter nesnesi oluştur ve dosyayı aç
                                        using (StreamWriter sw = new StreamWriter(DataPath + "\\Tags.txt", true))
                                        {
                                            while(selectedTags.Count > 0)
                                            {
                                                // açıklamayı dosyaya yaz
                                                sw.WriteLine(selectedTags[0]);
                                                selectedTags.RemoveAt(0);
                                            }
                                        }
                                    }

                                    // Resimler klasörü oluştur
                                    Directory.CreateDirectory(DataPath + "\\Images");
                                    ProcessImages(DataPath + "\\Images");

                                    MessageBox.Show("Veri başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    Console.WriteLine("Klasör zaten var: " + DataPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Hata: " + ex.Message);
                            }

                        }
                        catch (Exception ex)
                        {
                            // Hata durumunda hata mesajını göster
                            MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        MessageBox.Show("Başlık başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Bulunan başlık!! Lütfen yeni bir başlık giriniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen en az 3 karakter içeren bir metin girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını göster
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


           
        }

        /// <summary>
        /// başlık varmı yokmu onu kontrol eder
        /// </summary>
        /// <param name="arananMetin"></param>
        /// <returns></returns>
        private bool IsThereATitle(string Title)
        {
            // Dosyanın içeriğini satır satır oku
            using (StreamReader sr = new StreamReader(titlePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // Aranan metni içeren bir satır bulunursa true döndür
                    if (line.Contains(Title))
                    {
                        return true;
                    }
                }
            }
            // Aranan metin dosyada bulunamazsa false döndür
            return false;
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            SelectImages();
        }

        private void Title_TextChanged(object sender, EventArgs e)
        {

        }

        private void Info_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// resimleri seçmemizi sağlar // resimlerin ebadlanması gerek
        /// </summary>
        private void SelectImages()
        {
            // OpenFileDialog oluştur
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Dosya seçim iletişim kutusu başlığı
                openFileDialog.Title = "Resim Seç";

                // Yalnızca resim dosyalarını filtrele
                openFileDialog.Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";

                // Birden fazla dosya seçimini etkinleştir
                openFileDialog.Multiselect = true;

                // Kullanıcı dosyaları seçtikten sonra
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Seçilen dosyaların yollarını geri döndür
                    selectedImages = openFileDialog.FileNames;

                    // seçilen ilk resmi açar
                    Picture.SizeMode = PictureBoxSizeMode.Zoom;
                    Picture.Image = Image.FromFile(selectedImages[0]);
                }
            }
        }

        /// <summary>
        /// seçilen resimleri klasöre kopyalar
        /// </summary>
        /// <param name="path"></param>
        private void ProcessImages(string path)
        {
            int i = 0;

            // Seçilen her dosya için işlem yap
            foreach (string file in selectedImages)
            {
                try
                {
                    // resmin adını belirle
                    string fileName = "\\" + i + ".jpg";

                    // Dosyayı hedef klasöre kopyala
                    File.Copy(file, path + fileName);

                    i++;
                }
                catch (Exception ex)
                {
                    // Hata durumunda hata mesajını göster
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Tags_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçilen öğeyi işle
            int index = Tags.SelectedIndex;

            if(index > -1)
            {
                selectedTags.Add(tags[index]);
                tags.RemoveAt(index);
            }

            UpdateListBoxes();
        }

        private void SelectedTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçilen öğeyi işle
            int index = SelectedTags.SelectedIndex;

            if (index > -1)
            {
                tags.Add(selectedTags[index]);
                selectedTags.RemoveAt(index);
            }

            UpdateListBoxes();
        }

        private void UpdateListBoxes()
        {
            // Listbox'ları temizle
            Tags.Items.Clear();
            SelectedTags.Items.Clear();

            // tags listesindeki öğeleri Tags listbox'ına ekleyin
            foreach (string tag in tags)
            {
                Tags.Items.Add(tag);
            }

            // selectedTags listesindeki öğeleri SelectedTags listbox'ına ekleyin
            foreach (string tag in selectedTags)
            {
                SelectedTags.Items.Add(tag);
            }
        }

        private void TagsLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
