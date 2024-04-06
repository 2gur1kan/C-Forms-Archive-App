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
        string archivePath = "Archive\\";
        string dataPath = "Data\\";
        public string titleDataBaseName = "Titles.txt";
        string titlePath;

        private void SaveForm_Load(object sender, EventArgs e)
        {
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


                try // kalan veriyi kaydeden kısım 
                {
                    string DataPath = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath + Title.Text + ".txt");

                    // StreamWriter nesnesi oluştur ve dosyayı aç
                    using (StreamWriter sw = new StreamWriter(DataPath, true))
                    {
                        // Veriyi dosyaya yaz
                        sw.WriteLine(Title.Text);
                        sw.WriteLine(Info.Text);
                    }

                    MessageBox.Show("Veri başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Hata durumunda hata mesajını göster
                    MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private bool IsThereATitle(string arananMetin)
        {
            // Dosyanın içeriğini satır satır oku
            using (StreamReader sr = new StreamReader(titlePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // Aranan metni içeren bir satır bulunursa true döndür
                    if (line.Contains(arananMetin))
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

        }

        private void Title_TextChanged(object sender, EventArgs e)
        {

        }

        private void Info_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
