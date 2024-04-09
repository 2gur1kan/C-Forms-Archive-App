﻿using System;
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
    public partial class DetailedSearchForm : Form
    {
        public DetailedSearchForm()
        {
            InitializeComponent();
        }

        string archivePath = ArchiveMain.archivePath;
        string dataPath = ArchiveMain.dataPath;
        string titleDataBaseName = ArchiveMain.titleDataBaseName;
        string tagDataBaseName = ArchiveMain.tagsDataBaseName;

        string titlePath;
        string tagPath;
        string DataPath;

        public List<string> tags = new List<string>();
        List<string> selectedTags = new List<string>();

        List<string> searchTitleList = new List<string>();

        private void DetailedSearchForm_Load(object sender, EventArgs e)
        {
            DataPath = Path.Combine(Environment.CurrentDirectory, archivePath + dataPath);
            titlePath = Path.Combine(Environment.CurrentDirectory, archivePath + titleDataBaseName);
            tagPath = Path.Combine(Environment.CurrentDirectory, archivePath + tagDataBaseName);

            //set rate
            string[] ranges = new string[] { "0", "1", "2", "3", "4", "5" };
            Rate.Items.AddRange(ranges);
            Rate.DropDownStyle = ComboBoxStyle.DropDownList;
            Rate.Text = "0";

            // tagleri ekler
            foreach (string tag in tags)
            {
                Tags.Items.Add(tag);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            searchTitleList = new List<string>();

            string SearchTitle = SearchBar.Text;

            if (File.Exists(titlePath))
            {
                //aranacaklar listesini oluşturur
                if (!string.IsNullOrEmpty(SearchTitle))
                {
                    searchTitleList = new List<string>();

                    // Dosya mevcut ise, dosyadaki tüm satırları okuyun
                    string[] lines = File.ReadAllLines(titlePath);

                    // Aranan başlığın tam eşleşmesini arayın
                    bool isThere = lines.Any(line => line.Equals(SearchTitle));

                    if (isThere)
                    {
                        searchTitleList.Add(SearchTitle);
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
                                if (part != SearchTitle) searchTitleList.Add(part);
                            }
                        }
                    }
                }
                else
                {
                    // titlePath'deki dosyayı satır satır okuyun
                    string[] lines = File.ReadAllLines(titlePath);

                    // searchTitleList listesine her satırı ekle
                    foreach (string line in lines)
                    {
                        // Satırın boş olmadığından ve zaten listeye eklenmediğinden emin olun
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            searchTitleList.Add(line);
                        }
                    }
                }

                // rate verilmiş ise onları listede eler
                if(Rate.Text != "0")
                {
                    int rate = int.Parse(Rate.Text);

                    foreach (string title in searchTitleList)
                    {
                        string path = Path.Combine(DataPath, title, "Data.txt");

                        if (File.Exists(path))
                        { 
                            // Dosyayı açın ve ilk satırı okuyun
                            // ilk satır rate i veriyor
                            using (StreamReader sr = new StreamReader(path))
                            {
                                string firstLine = sr.ReadLine();

                                if (!string.IsNullOrEmpty(firstLine))
                                {
                                    if (rate < int.Parse(firstLine))
                                    {
                                        searchTitleList.Remove(title);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Dosya boş.");
                                }
                            }
                        }
                    }
                }

                // taglere göre ele
                if(selectedTags.Count > 0)
                {
                    foreach (string title in searchTitleList)
                    {
                        string path = Path.Combine(DataPath, title, "Tags.txt");
                        List<string> tags = new List<string>();

                        if (File.Exists(path))
                        {
                            // Dosyayı açın ve ilk satırı okuyun
                            // ilk satır rate i veriyor
                            using (StreamReader sr = new StreamReader(path))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    // Her bir satırı "tags" listesine ekleyin
                                    tags.Add(line);
                                }
                            }
                        }

                        if(tags.Count > 0)
                        {
                            // burada tags ler eğer selectedTags değişkenindeki elemanları içermiyorsa o elemanı searchTitleList içerisinden silsin
                            foreach (string selectedTag in selectedTags)
                            {
                                bool haveTag = false;

                                foreach (string tag in tags)
                                {
                                    if (tag == selectedTag)
                                    {
                                        haveTag = true;
                                        break; // Eşleşme bulunduğunda iç içe döngüyü sonlandır
                                    }
                                }

                                if (!haveTag)
                                {
                                    searchTitleList.Remove(title);
                                    break;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                // Dosya mevcut değil ise
                MessageBox.Show("Belirtilen dosya bulunamadı: " + titlePath);
            }       
        }

        private void Tags_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçilen öğeyi işle
            int index = Tags.SelectedIndex;

            if (index > -1)
            {
                selectedTags.Add(tags[index]);
                tags.RemoveAt(index);
            }

            UpdateListBoxes();
        }

        private void SelectedTags_SelectedIndexChanged_1(object sender, EventArgs e)
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


    }
}