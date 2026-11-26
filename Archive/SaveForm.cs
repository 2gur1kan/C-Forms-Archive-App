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
    /// <summary>
    /// yeni kayıt ekleme ve mevcut kaydı düzenleme formu
    /// editId doluysa düzenleme modunda açılır, boşsa yeni kayıt olur
    /// resimler kaydedilene kadar bellekte tutulur, vazgeçilirse veri tabanına hiçbir şey yazılmaz
    /// </summary>
    public partial class SaveForm : Form
    {
        public SaveForm()
        {
            InitializeComponent();
        }

        private const string linkHint = "Link"; // link kutusunun designer'daki varsayılan yazısı

        // yerleşim ayarları
        private int leftPercent = 55; // sol sütunun (link, puan, açıklama) genişlik yüzdesi
        private int pictureEdgePercent = 40; // resim kutusunun sağ sütundaki yükseklik yüzdesi
        private int saveH = 52;

        // resim butonlarının ölçüleri (resim kutusunun altına iki satır)
        private int rowGap = 4;
        private int rowH = 26;
        private int navW = 34;
        private int moveW = 74;
        private int addW = 80;
        private int delW = 60;
        private int coverW = 110;
        private float rowFont = 9f;

        public string editId; // doluysa bu kayıt düzenlenir
        public List<string> tags = new List<string>(); // seçilebilecek taglar

        List<string> selectedTags = new List<string>();
        List<byte[]> imageList = new List<byte[]>(); // sıra 0 = kapak resmi
        int imgIndex = 0;

        Label countLabel;
        Panel imageBar;

        private void SaveForm_Load(object sender, EventArgs e)
        {
            ArchiveMain.Sizer(this);
            ArchiveMain.FillRate(Rate);

            ArchiveMain.ApplyTheme(this);
            StyleSave();
            SizeChanged += (s, ev) => LayoutSave(); // pencere boyutu değişince yerleşim yenilenir

            Link.Enter += (s, ev) => { if (Link.Text == linkHint) Link.Text = ""; }; // yer tutucu yazı tıklayınca silinir
            Link.Leave += (s, ev) => { if (Link.Text.Trim() == "") Link.Text = linkHint; };

            if (editId != null) LoadForEdit();

            UpdateListBoxes(); // tagleri listeye koyar
            LayoutSave();
        }

        /// <summary>
        /// tema dışı özel görünümler: büyük başlık kutusu, kaydet butonu, etiket yazıları
        /// </summary>
        private void StyleSave()
        {
            Title.Font = ArchiveMain.F(16f, FontStyle.Bold);
            Link.BackColor = ArchiveMain.surface2Color;
            Link.ForeColor = ArchiveMain.accentHoverColor;
            Link.Font = ArchiveMain.F(11.5f);

            rateLabel.Text = "Puan";
            TagsLabel.Text = "Taglar  (tıkla: ekle / çıkar)";
            rateLabel.AutoSize = true;
            TagsLabel.AutoSize = true;

            Save.Font = ArchiveMain.F(12f, FontStyle.Bold);
            Save.Dock = DockStyle.Bottom;
            Save.Height = saveH;

            Picture.BackColor = ArchiveMain.surfaceColor;
            Picture.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// kontrolleri pencere boyuna göre dizer: üstte başlık, solda link/puan/açıklama, sağda resim ve tag listeleri
        /// </summary>
        private void LayoutSave()
        {
            int w = ClientSize.Width;
            int pad = ArchiveMain.pad;
            int gap = ArchiveMain.gap;

            Title.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Title.Dock = DockStyle.None;
            Title.SetBounds(pad, pad, w - 2 * pad, Title.Height);

            // sol sütun
            int lw = (w - 3 * pad) * leftPercent / 100;
            int bottom = Save.Top - pad;

            Link.SetBounds(pad, Title.Bottom + gap, lw, Link.Height);

            rateLabel.Location = new Point(pad, Link.Bottom + gap + 4);
            Rate.SetBounds(rateLabel.Right + gap, Link.Bottom + gap, 70, Rate.Height);

            Info.SetBounds(pad, Rate.Bottom + gap, lw, Math.Max(40, bottom - (Rate.Bottom + gap)));

            // sağ sütun: resim, resim butonları, tag etiketi, iki tag listesi
            int x0 = pad * 2 + lw;
            int rw = Math.Max(120, w - x0 - pad);
            int top = Link.Top;
            int avail = bottom - top;
            int barH = 2 * rowH + rowGap;

            Picture.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Picture.SetBounds(x0, top, rw, Math.Max(60, avail * pictureEdgePercent / 100));

            TagsLabel.Location = new Point(x0, Picture.Bottom + rowGap + barH + gap);

            int listTop = TagsLabel.Bottom + rowGap;
            int half = (rw - gap) / 2;
            Tags.SetBounds(x0, listTop, half, Math.Max(40, bottom - listTop));
            SelectedTags.SetBounds(x0 + half + gap, listTop, half, Tags.Height);
        }

        /// <summary>
        /// resim butonları pencere tamamen yerleştikten sonra kurulur, sağa dayalı resim kutusunun gerçek konumuna göre dizilsinler
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            BuildImageUI();
            ShowCurrentImage();
        }

        /// <summary>
        /// düzenleme modunda mevcut kaydın bilgilerini forma doldurur, kayıt yoksa formu kapatır
        /// </summary>
        private void LoadForEdit()
        {
            Record gg = ArchiveDB.Instance.GetRecord(editId);

            if (gg == null)
            {
                ArchiveMain.ShowError("Kayıt bulunamadı.");
                Close();
                return;
            }

            Text = "Kaydı Düzenle";
            Title.Text = gg.Title;
            Link.Text = gg.Link != "" ? gg.Link : linkHint;
            Info.Text = gg.Info;
            Rate.Text = gg.Rate.ToString();

            selectedTags = gg.Tags;
            tags.RemoveAll(tg => selectedTags.Contains(tg)); // seçili olanlar sol listede görünmesin

            imageList = ArchiveDB.Instance.GetImages(editId);
        }

        ////////////////////// kayıt
        /// <summary>
        /// formdaki verileri kaydeder (yeni ya da güncelleme), başlık kısa veya zaten kayıtlıysa hata verip çıkar
        /// başarılı olursa form kapanır
        /// </summary>
        private void Save_Click(object sender, EventArgs e)
        {
            string gg = Title.Text.Trim();

            if (gg.Length < ArchiveMain.minTitleLength)
            {
                ArchiveMain.ShowError("Lütfen en az " + ArchiveMain.minTitleLength + " karakter içeren bir metin girin.");
                return;
            }

            try
            {
                if (ArchiveDB.Instance.TitleExists(gg, editId))
                {
                    ArchiveMain.ShowError("Bulunan başlık!! Lütfen yeni bir başlık giriniz");
                    return;
                }

                Record r = new Record();
                r.Id = editId;
                r.Title = gg;
                r.Link = Link.Text == linkHint ? "" : Link.Text.Trim(); // link yazılmamışsa boş kalır
                r.Info = Info.Text.TrimEnd();
                r.Tags = new List<string>(selectedTags);
                r.Images = imageList;

                int rate;
                int.TryParse(Rate.Text, out rate);
                r.Rate = Math.Max(0, Math.Min(ArchiveMain.maxRate, rate));

                if (editId == null) ArchiveDB.Instance.AddRecord(r);
                else ArchiveDB.Instance.UpdateRecord(r);

                MessageBox.Show("Veri başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                ArchiveMain.ShowError("Hata: " + ex.Message);
            }
        }

        private void Picture_Click(object sender, EventArgs e) => AddImages();

        private void Title_TextChanged(object sender, EventArgs e)
        {

        }

        private void Info_TextChanged(object sender, EventArgs e)
        {

        }

        ////////////////////// resimler
        /// <summary>
        /// resim kutusunun altına gezinme, taşıma, ekleme, silme ve kapak butonlarını kurar
        /// butonlar bir panelin içindedir, panel resim kutusunu takip eder (ekran ölçeklemesinde kaymasın)
        /// </summary>
        private void BuildImageUI()
        {
            int barH = 2 * rowH + rowGap;

            imageBar = new Panel();
            imageBar.Height = barH;
            imageBar.BackColor = ArchiveMain.bgColor;
            Controls.Add(imageBar);
            imageBar.BringToFront(); // ölçekleme sonrası açıklama kutusuyla çakışsa bile butonlar önde kalsın

            Picture.LocationChanged += (s, ev) => FollowPicture();
            Picture.SizeChanged += (s, ev) => FollowPicture();
            FollowPicture();

            AddRowButton("<", 0, 0, navW, (s, ev) => Browse(-1));
            AddRowButton(">", navW, 0, navW, (s, ev) => Browse(1));
            AddRowButton("Taşı <", 2 * navW + rowGap, 0, moveW, (s, ev) => MoveImage(-1));
            AddRowButton("Taşı >", 2 * navW + moveW + rowGap, 0, moveW, (s, ev) => MoveImage(1));

            int y2 = rowH + rowGap;
            AddRowButton("Ekle", 0, y2, addW, (s, ev) => AddImages());
            AddRowButton("Sil", addW, y2, delW, (s, ev) => RemoveImage());
            AddRowButton("Kapak yap", addW + delW, y2, coverW, (s, ev) => MakeCover());

            countLabel = ArchiveMain.MakeLabel("", 2 * navW + 2 * moveW + 2 * rowGap + 4, 4);
            countLabel.Font = ArchiveMain.F(rowFont);
            imageBar.Controls.Add(countLabel);
        }

        private void FollowPicture()
        {
            imageBar.Location = new Point(Picture.Left, Picture.Bottom + rowGap);
            imageBar.Width = Picture.Width;
        }

        private void AddRowButton(string text, int x, int y, int w, EventHandler click)
        {
            Button gg = ArchiveMain.MakeButton(text, x, y, w, rowH, click);
            gg.Font = ArchiveMain.F(rowFont, FontStyle.Bold);
            imageBar.Controls.Add(gg);
        }

        /// <summary>
        /// seçili resmi gösterir, önceki resmi bellekten atar, resim yoksa alan boş kalır
        /// </summary>
        private void ShowCurrentImage()
        {
            Image gg = Picture.Image;

            imgIndex = Math.Max(0, Math.Min(imageList.Count - 1, imgIndex));
            Picture.Image = imageList.Count > 0 ? ArchiveMain.ImageFromBytes(imageList[imgIndex]) : null;

            gg?.Dispose();

            countLabel.Text = imageList.Count == 0 ? "Resim yok (kutuya tıkla)" : (imgIndex + 1) + " / " + imageList.Count + (imgIndex == 0 ? "  kapak" : "");
        }

        private void Browse(int delta)
        {
            int to = imgIndex + delta;
            if (to < 0 || to >= imageList.Count) return;

            imgIndex = to;
            ShowCurrentImage();
        }

        /// <summary>
        /// resim dosyalarını seçtirir, büyükler küçültülüp belleğe alınır, geçersiz dosya atlanır
        /// </summary>
        private void AddImages()
        {
            using (OpenFileDialog gg = new OpenFileDialog())
            {
                gg.Title = "Resim Seç";
                gg.Filter = "Resim Dosyaları (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
                gg.Multiselect = true;

                if (gg.ShowDialog() != DialogResult.OK) return;

                int first = imageList.Count;

                foreach (string file in gg.FileNames)
                {
                    try
                    {
                        imageList.Add(ArchiveDB.PrepareImage(file));
                    }
                    catch (Exception ex)
                    {
                        ArchiveMain.ShowError(Path.GetFileName(file) + " eklenemedi: " + ex.Message);
                    }
                }

                if (imageList.Count > first) imgIndex = first; // eklenen ilk resmi gösterir
                ShowCurrentImage();
            }
        }

        private void RemoveImage()
        {
            if (imageList.Count == 0) return;

            imageList.RemoveAt(imgIndex);
            ShowCurrentImage();
        }

        /// <summary>
        /// seçili resmi bir sıra öne (-1) ya da geriye (+1) taşır
        /// </summary>
        private void MoveImage(int delta)
        {
            int to = imgIndex + delta;
            if (to < 0 || to >= imageList.Count) return;

            byte[] gg = imageList[imgIndex];
            imageList[imgIndex] = imageList[to];
            imageList[to] = gg;

            imgIndex = to;
            ShowCurrentImage();
        }

        /// <summary>
        /// seçili resmi başa alır, 0. sıradaki resim kapak olur (listelerde bu görünür)
        /// </summary>
        private void MakeCover()
        {
            if (imgIndex <= 0) return;

            byte[] gg = imageList[imgIndex];
            imageList.RemoveAt(imgIndex);
            imageList.Insert(0, gg);

            imgIndex = 0;
            ShowCurrentImage();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Picture.Image?.Dispose();
            base.OnFormClosed(e);
        }

        ////////////////////// taglar
        private void Tags_SelectedIndexChanged(object sender, EventArgs e) => MoveTag(tags, selectedTags, Tags.SelectedIndex);

        private void SelectedTags_SelectedIndexChanged(object sender, EventArgs e) => MoveTag(selectedTags, tags, SelectedTags.SelectedIndex);

        /// <summary>
        /// seçilen tagi bir listeden diğerine taşır, seçim yoksa (index -1) bir şey yapmaz
        /// </summary>
        private void MoveTag(List<string> from, List<string> to, int index)
        {
            if (index < 0) return;

            to.Add(from[index]);
            from.RemoveAt(index);

            UpdateListBoxes();
        }

        private void UpdateListBoxes()
        {
            Tags.Items.Clear();
            SelectedTags.Items.Clear();

            foreach (string tag in tags) Tags.Items.Add(tag);
            foreach (string tag in selectedTags) SelectedTags.Items.Add(tag);
        }

        private void TagsLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
