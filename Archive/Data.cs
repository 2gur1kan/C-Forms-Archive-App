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
        public string recordId; // OpenData doldurur, kayıt buna göre veri tabanından okunur

        // yerleşim ayarları
        private int headerH = 60;
        private int actionH = 36;
        private int editW = 84;
        private int deleteW = 96;
        private int linkH = 28;
        private int navW = 40;
        private int bottomBandH = 96; // puan ve tag satırlarının yüksekliği
        private int infoPercent = 40; // sol açıklama alanının genişlik yüzdesi

        Record record;
        int imageCount = 0;
        int imageNum = 0;

        Label ratingLabel;
        Label imageInfoLabel;
        FlowLayoutPanel tagsPanel;
        Button deleteButton;

        private void Data_Load(object sender, EventArgs e)
        {
            ArchiveMain.Sizer(this);

            BuildExtraUI();
            ArchiveMain.ApplyTheme(this);
            StyleData();
            LayoutData();
            SizeChanged += (s, ev) => LayoutData(); // pencere boyutu değişince yerleşim yenilenir

            LoadRecord();
        }

        /// <summary>
        /// designer'da olmayan kontrolleri ekler: Delete butonu, yıldız puanı, resim sayacı ve tag etiketleri
        /// </summary>
        private void BuildExtraUI()
        {
            deleteButton = ArchiveMain.MakeButton("Delete", 0, 0, deleteW, actionH, Delete_Click);
            Controls.Add(deleteButton);

            ratingLabel = ArchiveMain.MakeLabel("", 0, 0);
            imageInfoLabel = ArchiveMain.MakeLabel("", 0, 0);
            Controls.Add(ratingLabel);
            Controls.Add(imageInfoLabel);

            tagsPanel = new FlowLayoutPanel();
            tagsPanel.WrapContents = true;
            tagsPanel.AutoScroll = true;
            tagsPanel.BackColor = ArchiveMain.bgColor;
            Controls.Add(tagsPanel);
        }

        /// <summary>
        /// tema dışı özel görünümler: başlık çubuğu, yıldızlar, düğmeler
        /// </summary>
        private void StyleData()
        {
            Title.AutoSize = false;
            Title.BackColor = ArchiveMain.surfaceColor;
            Title.ForeColor = ArchiveMain.textColor;
            Title.Font = ArchiveMain.F(18f, FontStyle.Bold);
            Title.TextAlign = ContentAlignment.MiddleLeft;
            Title.Padding = new Padding(ArchiveMain.pad, 0, 0, 0);

            ArchiveMain.Danger(deleteButton);
            deleteButton.Font = ArchiveMain.F(10f, FontStyle.Bold);

            Info.BorderStyle = BorderStyle.None;
            Info.BackColor = ArchiveMain.surfaceColor;
            Info.Font = ArchiveMain.F(11.5f);

            Picture.BackColor = ArchiveMain.surfaceColor;

            ratingLabel.Font = ArchiveMain.F(20f);
            ratingLabel.ForeColor = ArchiveMain.starColor;
        }

        /// <summary>
        /// kontrolleri pencere boyuna göre dizer: üstte başlık ve işlem düğmeleri, solda açıklama, sağda link, resim ve altında puan/tag
        /// </summary>
        private void LayoutData()
        {
            if (tagsPanel == null) return; // yükleme bitmeden çalışmasın

            int w = ClientSize.Width;
            int h = ClientSize.Height;
            int pad = ArchiveMain.pad;
            int gap = ArchiveMain.gap;

            // başlık çubuğu, işlem düğmeleri içinde sağda
            Title.Dock = DockStyle.None;
            Title.SetBounds(0, 0, w, headerH);

            int btnY = (headerH - actionH) / 2;
            Edit.SetBounds(w - pad - editW, btnY, editW, actionH);
            deleteButton.SetBounds(Edit.Left - gap - deleteW, btnY, deleteW, actionH);
            Edit.BringToFront();
            deleteButton.BringToFront();

            // sol: açıklama
            int y0 = headerH + pad;
            int leftW = (w - 3 * pad) * infoPercent / 100;

            Info.Dock = DockStyle.None;
            Info.SetBounds(pad, y0, leftW, Math.Max(40, h - y0 - pad));

            // sağ: link, resim alanı, puan ve taglar
            int x0 = Info.Right + pad;
            int rw = Math.Max(120, w - x0 - pad);

            Link.Dock = DockStyle.None;
            Link.SetBounds(x0, y0, rw, linkH);

            int py = Link.Bottom + gap;
            int ph = Math.Max(60, h - pad - bottomBandH - py);

            BackImage.SetBounds(x0, py, navW, ph);
            Picture.SetBounds(x0 + navW + gap, py, Math.Max(40, rw - 2 * (navW + gap)), ph);
            NextImage.SetBounds(Picture.Right + gap, py, navW, ph);

            ratingLabel.Location = new Point(x0, Picture.Bottom + gap);
            imageInfoLabel.Location = new Point(ratingLabel.Right + 2 * gap, ratingLabel.Top + (ratingLabel.Height - imageInfoLabel.Height) / 2);
            tagsPanel.SetBounds(x0, ratingLabel.Bottom + gap / 2, rw, Math.Max(30, h - pad - (ratingLabel.Bottom + gap / 2)));
        }

        /// <summary>
        /// kaydı veri tabanından okuyup ekrana yazar, kayıt silinmişse formu kapatır
        /// düzenleme penceresinden dönünce de çağrılır
        /// </summary>
        private void LoadRecord()
        {
            record = ArchiveDB.Instance.GetRecord(recordId);

            if (record == null)
            {
                ArchiveMain.ShowError("Kayıt bulunamadı.");
                Close();
                return;
            }

            titleText = record.Title;
            Title.Text = record.Title;
            Link.Text = record.Link;
            Info.Text = record.Info;
            Info.Select(0, 0); // salt okunur kutu tüm metin seçili açılmasın

            ratingLabel.Text = ArchiveMain.Stars(record.Rate);
            FillTags();

            imageCount = ArchiveDB.Instance.ImageCount(recordId);
            ShowImage(0);
        }

        /// <summary>
        /// kaydın tag'lerini küçük etiketler (chip) olarak dizer, tag yoksa soluk bir not yazar
        /// </summary>
        private void FillTags()
        {
            tagsPanel.Controls.Clear();

            foreach (string tag in record.Tags) tagsPanel.Controls.Add(ArchiveMain.MakeChip(tag));

            if (record.Tags.Count == 0) tagsPanel.Controls.Add(ArchiveMain.MakeLabel("Tag yok", 0, 0));
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            using (SaveForm gg = new SaveForm())
            {
                gg.editId = recordId;
                gg.tags = ArchiveDB.Instance.GetTags();
                gg.ShowDialog(this);
            }

            LoadRecord();
        }

        /// <summary>
        /// kaydı onay alarak siler ve formu kapatır
        /// </summary>
        private void Delete_Click(object sender, EventArgs e)
        {
            DialogResult ask = MessageBox.Show("'" + record.Title + "' kalıcı olarak silinecek. Emin misin?", "Kaydı sil", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ask != DialogResult.Yes) return;

            ArchiveDB.Instance.DeleteRecord(recordId);
            Close();
        }

        /// <summary>
        /// linki tarayıcıda açar, yalnızca http ve https linkleri açılır (kayıttaki rastgele metin program başlatmasın)
        /// </summary>
        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Uri gg;
            if (!Uri.TryCreate(Link.Text, UriKind.Absolute, out gg)) return;
            if (gg.Scheme != Uri.UriSchemeHttp && gg.Scheme != Uri.UriSchemeHttps) return;

            try
            {
                Process.Start(gg.AbsoluteUri);
            }
            catch (Exception ex)
            {
                ArchiveMain.ShowError("Hata: " + ex.Message);
            }
        }

        ////////////////////// resimler
        /// <summary>
        /// seçilen resmi açar, önceki resmi bellekten atar, ileri geri butonlarını ve resim sayacını günceller
        /// resim yoksa alan boş kalır
        /// </summary>
        private void ShowImage(int num)
        {
            Image gg = Picture.Image;

            imageNum = num;
            Picture.SizeMode = PictureBoxSizeMode.Zoom;

            byte[] data = imageCount > 0 ? ArchiveDB.Instance.GetImage(recordId, num) : null;
            Picture.Image = data != null ? ArchiveMain.ImageFromBytes(data) : null;

            gg?.Dispose(); // önceki resim bellekte kalmasın

            BackImage.Enabled = num > 0;
            NextImage.Enabled = num + 1 < imageCount;
            imageInfoLabel.Text = imageCount > 0 ? "Resim " + (num + 1) + " / " + imageCount : "Resim yok";

            LayoutData(); // sayaç yazısı değişti, konumu tazelenir
        }

        private void BackImage_Click(object sender, EventArgs e)
        {
            if (imageNum > 0) ShowImage(imageNum - 1);
        }

        private void NextImage_Click(object sender, EventArgs e)
        {
            if (imageNum + 1 < imageCount) ShowImage(imageNum + 1);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Picture.Image?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
