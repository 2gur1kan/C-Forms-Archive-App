using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Archive
{
    /// <summary>
    /// tag yönetim penceresi: ekle, yeniden adlandır, sil
    /// yeniden adlandırma ve silme o tagi taşıyan kayıtlara otomatik yansır
    /// designer'ı yoktur, kontroller kodla kurulur
    /// </summary>
    public class TagForm : Form
    {
        private int formW = 460;
        private int formH = 440;
        private int margin = 12;
        private int rowH = 36;

        ListBox tagList;
        TextBox nameBox;

        public TagForm()
        {
            BuildUI();
            LoadTags();
        }

        private void BuildUI()
        {
            Text = "Tag Yönetimi";
            ClientSize = new Size(formW, formH);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            tagList = new ListBox();
            tagList.Location = new Point(margin, margin);
            tagList.Size = new Size(formW - 2 * margin, 250);
                        tagList.SelectedIndexChanged += TagList_SelectedIndexChanged;
            Controls.Add(tagList);

            nameBox = new TextBox();
            nameBox.Location = new Point(margin, tagList.Bottom + margin);
            nameBox.Width = formW - 2 * margin;
                        Controls.Add(nameBox);

            int bw = (formW - 4 * margin) / 3;
            int by = nameBox.Bottom + margin;
            Controls.Add(ArchiveMain.MakeButton("Ekle", margin, by, bw, rowH, Add_Click));
            Controls.Add(ArchiveMain.MakeButton("Yeniden Adlandır", 2 * margin + bw, by, bw, rowH, Rename_Click));
            Button del = ArchiveMain.MakeButton("Sil", 3 * margin + 2 * bw, by, bw, rowH, Delete_Click);
            Controls.Add(del);

            ArchiveMain.ApplyTheme(this); // renkler ve yazı tipleri
            ArchiveMain.Danger(del);
            tagList.Font = ArchiveMain.F(13f);
            nameBox.Font = ArchiveMain.F(13f);
        }

        private void LoadTags()
        {
            tagList.Items.Clear();
            tagList.Items.AddRange(ArchiveDB.Instance.GetTags().ToArray());
        }

        private void TagList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tagList.SelectedIndex >= 0) nameBox.Text = tagList.SelectedItem.ToString(); // seçileni yazı kutusuna alır, yeniden adlandırmak için
        }

        private void Add_Click(object sender, EventArgs e)
        {
            string gg = nameBox.Text.Trim();
            if (gg.Length == 0) return;

            if (!ArchiveDB.Instance.AddTag(gg)) ArchiveMain.ShowError("Bu tag zaten var.");

            LoadTags();
        }

        private void Rename_Click(object sender, EventArgs e)
        {
            string gg = nameBox.Text.Trim();
            if (tagList.SelectedIndex < 0 || gg.Length == 0) return;

            try
            {
                ArchiveDB.Instance.RenameTag(tagList.SelectedItem.ToString(), gg);
            }
            catch (Exception)
            {
                ArchiveMain.ShowError("Bu ada sahip bir tag zaten var."); // yeni ad başka bir tagle çakıştı
            }

            LoadTags();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (tagList.SelectedIndex < 0) return;

            string gg = tagList.SelectedItem.ToString();
            int count = ArchiveDB.Instance.TagUseCount(gg);

            DialogResult ask = MessageBox.Show("'" + gg + "' tagi " + count + " kayıttan kalkacak. Silinsin mi?", "Tag sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ask != DialogResult.Yes) return;

            ArchiveDB.Instance.DeleteTag(gg);
            nameBox.Text = "";
            LoadTags();
        }
    }
}
