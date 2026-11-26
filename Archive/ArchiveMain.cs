using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
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

        ////////////////////// dosya yolları (veri artık Archive.db içinde, bunlar eski klasör yapısını aktarmak ve DB yolu için kalır)
        public static string archivePath = "Archive\\";
        public static string dataPath = "Data\\";
        public static string titleDataBaseName = "Titles.txt";
        public static string tagsDataBaseName = "Tags.txt";

        ////////////////////// ayarlar (sayılar koda gömülmesin diye burada durur)
        public static float sizeRatio = .5f; // pencere ekranın kaçta kaçı kadar olsun
        public static int windowOffset = 10; // pencerenin ekran köşesine uzaklığı
        public static int maxRate = 5;
        public static int minTitleLength = 3;
        public static int cardHeight = 96; // arama sonuçlarındaki kart yüksekliği
        public static float smallFont = 10f; // kodla eklenen kontrollerin yazı boyu
        public static string[] sortNames = { "Yeniden eskiye (yüklenme sırası)", "Eskiden yeniye (yüklenme sırası)", "Puana göre (yüksekten düşüğe)", "Başlığa göre (A-Z)" }; // sıra SortMode ile aynı

        ////////////////////// tema (tüm formlar buradan beslenir)
        public static Color bgColor = Color.FromArgb(15, 17, 21);
        public static Color surfaceColor = Color.FromArgb(24, 27, 34); // kartlar
        public static Color surface2Color = Color.FromArgb(34, 39, 49); // giriş kutuları ve üzerine gelinen kart
        public static Color borderColor = Color.FromArgb(52, 58, 72);
        public static Color textColor = Color.FromArgb(232, 234, 240);
        public static Color mutedColor = Color.FromArgb(139, 147, 167);
        public static Color accentColor = Color.FromArgb(91, 140, 255);
        public static Color accentHoverColor = Color.FromArgb(122, 162, 255);
        public static Color starColor = Color.FromArgb(255, 194, 75);
        public static Color dangerColor = Color.FromArgb(229, 72, 77);
        public static Color dangerHoverColor = Color.FromArgb(240, 100, 104);
        public static Color chipColor = Color.FromArgb(32, 46, 84);
        public static string fontName = "Segoe UI";
        public static int pad = 16; // pencere kenar boşluğu
        public static int gap = 10; // kontroller arası boşluk

        // ana ekran yerleşimi
        private int toolH = 34;
        private int rowGap = 8;
        private int shelfGap = 12;
        private int minShelfH = 64;
        private int maxShelfH = 120;
        private int thumbPad = 12; // raf kartında resim ile kart kenarı arası toplam boşluk
        public static int thumbGap = 14; // küçük resim ile başlık yazısı arası boşluk

        public static string DataFolder { get => Path.Combine(Environment.CurrentDirectory, archivePath + dataPath); }
        public static string TitleFile { get => Path.Combine(Environment.CurrentDirectory, archivePath + titleDataBaseName); }

        public List<string> Tags = new List<string>();
        public List<Button> Shelfs = new List<Button>();

        ComboBox sortBox;
        CheckBox deepBox;
        Label sortLabel;
        Button tagsButton;
        Button backupButton;
        Button restoreButton;

        ////////////////////// ana fonksiyonlar
        private void ArchiveMain_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            Sizer(this);

            if (!OpenDataBase())
            {
                Close();
                return;
            }

            Shelfs.AddRange(new Button[] { Shelf1, Shelf2, Shelf3, Shelf4 }); // butonlara hızlı erişim için
            for (int i = 1; i < Shelfs.Count; i++) Shelfs[i].Click += Shelf_Click; // Shelf1 designer'dan bağlı, kalanları kod bağlar

            BuildExtraUI();
            ApplyTheme(this);
            StyleMain();
            LayoutMain();
            SizeChanged += (s, ev) => LayoutMain(); // pencere boyutu değişince yerleşim yenilenir

            ArchiveDB.Instance.Changed += RefreshAll; // veri nerede değişirse ana ekran kendini yeniler
            FormClosed += (s, ev) => ArchiveDB.Instance.Changed -= RefreshAll;

            RefreshAll();
        }

        /// <summary>
        /// veri tabanını açar, ilk açılışta eski klasör yapısı varsa içindekileri aktarır ve haber verir
        /// açılamazsa false döner
        /// </summary>
        private bool OpenDataBase()
        {
            try
            {
                int imported = ArchiveDB.Instance.Open();
                if (imported == 0) return true;

                string skipped = ArchiveDB.Instance.ImportSkipped > 0 ? " (" + ArchiveDB.Instance.ImportSkipped + " kayıt alınamadı)" : "";
                MessageBox.Show(imported + " kayıt eski klasör yapısından veri tabanına aktarıldı" + skipped + ". Eski Data klasörüne dokunulmadı, kontrol ettikten sonra silebilirsin.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Veri tabanı açılamadı: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// designer'da olmayan kontrolleri ekler: sıralama, açıklamada arama, tag yönetimi, yedek
        /// konumları LayoutMain verir
        /// </summary>
        private void BuildExtraUI()
        {
            sortLabel = MakeLabel("Sırala", 0, 0);
            Controls.Add(sortLabel);

            sortBox = new ComboBox();
            sortBox.Width = 300;
            FillSort(sortBox);
            sortBox.SelectedIndexChanged += (s, ev) => LoadShelfs(); // raflar seçilen sıraya göre dolar
            Controls.Add(sortBox);

            deepBox = new CheckBox();
            deepBox.Text = "Açıklama ve linkte de ara";
            deepBox.Width = 260;
            Controls.Add(deepBox);

            tagsButton = MakeButton("Tag'ler", 0, 0, 90, toolH, Tags_Click);
            backupButton = MakeButton("Yedekle", 0, 0, 90, toolH, Backup_Click);
            restoreButton = MakeButton("Yedekten Geri Yükle", 0, 0, 170, toolH, Restore_Click);
            Controls.AddRange(new Control[] { tagsButton, backupButton, restoreButton });
        }

        /// <summary>
        /// designer'daki kontrollerin tema dışı özel görünümleri (başlık, birincil butonlar, arama kutusu)
        /// </summary>
        private void StyleMain()
        {
            label1.Text = "Archive"; // eski "Search Bar" etiketi uygulama başlığı oldu
            label1.AutoSize = true;
            label1.Font = F(22f, FontStyle.Bold);
            label1.ForeColor = textColor;

            SearchBar.Font = F(15f);
            SearchBar.BorderStyle = BorderStyle.FixedSingle;
        }

        /// <summary>
        /// tüm kontrolleri pencere boyuna göre dizer: üstte başlık ve araçlar, altında arama satırı, seçenekler ve raf kartları
        /// </summary>
        private void LayoutMain()
        {
            if (sortBox == null || Shelfs.Count == 0) return; // yükleme bitmeden çalışmasın

            int w = ClientSize.Width;

            // başlık satırı: solda isim, sağda araç butonları
            label1.Location = new Point(pad, pad);
            int x = w - pad;
            foreach (Button gg in new Button[] { restoreButton, backupButton, tagsButton })
            {
                x -= gg.Width;
                gg.Location = new Point(x, label1.Top + (label1.Height - toolH) / 2);
                x -= gap;
            }

            // arama satırı: kutu esner, butonlar sağa dizilir
            int y = label1.Bottom + gap;
            int rowH = SearchBar.Height;

            x = w - pad;
            foreach (Button gg in new Button[] { New, DetailedSearch, Search })
            {
                gg.Size = new Size(gg == DetailedSearch ? 150 : 100, rowH);
                x -= gg.Width;
                gg.Location = new Point(x, y);
                x -= gap;
            }

            SearchBar.Location = new Point(pad, y);
            SearchBar.Width = Math.Max(120, x - pad);

            // seçenek satırı
            int y2 = y + rowH + gap;
            sortLabel.Location = new Point(pad, y2 + 4);
            sortBox.Location = new Point(sortLabel.Right + rowGap, y2);
            deepBox.Location = new Point(sortBox.Right + 2 * gap, y2 + 3);

            // raf kartları kalan yüksekliği paylaşır
            int top = sortBox.Bottom + pad;
            int avail = ClientSize.Height - top - pad;
            int cardH = Math.Max(minShelfH, Math.Min(maxShelfH, (avail - (Shelfs.Count - 1) * shelfGap) / Shelfs.Count));

            for (int i = 0; i < Shelfs.Count; i++)
            {
                Shelfs[i].Size = new Size(w - 2 * pad, cardH);
                Shelfs[i].Location = new Point(pad, top + i * (cardH + shelfGap));
            }
        }

        /// <summary>
        /// tag listesini veri tabanından tazeler ve rafları yeniden doldurur
        /// </summary>
        private void RefreshAll()
        {
            try
            {
                Tags = ArchiveDB.Instance.GetTags();
                LoadShelfs();
            }
            catch (Exception ex)
            {
                ShowError("Hata: " + ex.Message);
            }
        }

        /// <summary>
        /// raflara seçili sıralamanın ilk 4 kaydını koyar (kapak resmi ve yıldız puanıyla), kaydı olmayan raf tıklanamaz olur
        /// </summary>
        private void LoadShelfs()
        {
            List<Record> top = ArchiveDB.Instance.Search("", false, 0, null, ToSort(sortBox));

            for (int i = 0; i < Shelfs.Count; i++)
            {
                bool hasFlag = i < top.Count; // boş rafa basınca olmayan kayıt açılmasın
                int thumbH = Math.Max(1, Shelfs[i].Height - thumbPad);

                Shelfs[i].Enabled = hasFlag;
                Shelfs[i].Tag = hasFlag ? top[i].Id : null;

                if (hasFlag) FillCard(Shelfs[i], top[i], ArchiveDB.Instance.GetImage(top[i].Id, 0), thumbH * 3 / 2, thumbH);
                else FillCard(Shelfs[i], null, null, 0, 0);
            }
        }

        private void New_Click(object sender, EventArgs e)
        {
            SaveForm gg = new SaveForm();
            gg.tags = new List<string>(Tags); // kopya verilir, yoksa kayıt formu ana tag listesinden eleman siler

            gg.StartPosition = FormStartPosition.Manual;
            gg.Location = Location;

            gg.Show();
        }

        private void Search_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBar.Text))
            {
                MessageBox.Show("Arama metni boş olamaz.");
                return;
            }

            List<Record> found = ArchiveDB.Instance.Search(SearchBar.Text, deepBox.Checked, 0, null, ToSort(sortBox));

            if (found.Count == 0)
            {
                MessageBox.Show("Aramanızla ilgili herhangi bir eşleşme bulunamadı.");
                return;
            }

            SearchResultForm gg = new SearchResultForm();
            gg.Results = found;

            gg.StartPosition = FormStartPosition.Manual;
            gg.Location = Location;

            gg.Show();
        }

        private void Shelf1_Click(object sender, EventArgs e) => Shelf_Click(sender, e);

        private void Shelf_Click(object sender, EventArgs e)
        {
            string id = ((Button)sender).Tag as string;
            if (id == null) return;

            OpenData(id, this);
        }

        private void DetailedSearch_Click(object sender, EventArgs e)
        {
            DetailedSearchForm gg = new DetailedSearchForm();
            gg.tags = new List<string>(Tags); // kopya verilir, yoksa seçilen tag ana listeden düşer
            gg.startSort = ToSort(sortBox); // ana ekrandaki sıralama varsayılan gelir
            gg.startDeepFlag = deepBox.Checked;

            gg.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        ////////////////////// tag yönetimi ve yedek
        private void Tags_Click(object sender, EventArgs e)
        {
            using (TagForm gg = new TagForm()) gg.ShowDialog(this);
        }

        /// <summary>
        /// tüm arşivi (kayıtlar, taglar, resimler) tek bir .db dosyasına yedekler
        /// </summary>
        private void Backup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog gg = new SaveFileDialog())
            {
                gg.Title = "Yedek dosyası";
                gg.Filter = "Arşiv yedeği (*.db)|*.db";
                gg.FileName = "Archive_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".db";

                if (gg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    ArchiveDB.Instance.Backup(gg.FileName);
                    MessageBox.Show("Yedek alındı:\n" + gg.FileName, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowError("Yedek alınamadı: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// yedekten geri yükler, mevcut arşiv Archive.db.bak olarak saklanır, açık diğer pencereler kapatılır
        /// </summary>
        private void Restore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog gg = new OpenFileDialog())
            {
                gg.Title = "Yedek dosyası seç";
                gg.Filter = "Arşiv yedeği (*.db)|*.db";

                if (gg.ShowDialog() != DialogResult.OK) return;

                DialogResult ask = MessageBox.Show("Mevcut arşivin yerine bu yedek yüklenecek (mevcut arşiv Archive.db.bak olarak saklanır). Devam edilsin mi?", "Geri yükle", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (ask != DialogResult.Yes) return;

                try
                {
                    foreach (Form other in Application.OpenForms.Cast<Form>().Where(f => f != this).ToList()) other.Close(); // eski veriyi gösteren pencereler kalmasın

                    ArchiveDB.Instance.Restore(gg.FileName);
                    MessageBox.Show("Yedek geri yüklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowError("Geri yüklenemedi: " + ex.Message);
                }
            }
        }

        #region Static Helpers (tüm formlar buradan kullanır)

        /// <summary>
        /// pencereyi ekranın bir kısmı kadar boyutlandırıp köşeye koyar
        /// </summary>
        public static void Sizer(Form gg)
        {
            System.Drawing.Rectangle rec = Screen.PrimaryScreen.WorkingArea;
            gg.Size = new System.Drawing.Size(Convert.ToInt32(sizeRatio * rec.Width), Convert.ToInt32(sizeRatio * rec.Height));
            gg.Location = new System.Drawing.Point(windowOffset, windowOffset);
        }

        public static void ShowError(string message) => MessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// rate kutusunu 0 ile maxRate arası doldurur, varsayılan 0
        /// </summary>
        public static void FillRate(ComboBox gg)
        {
            for (int i = 0; i <= maxRate; i++) gg.Items.Add(i.ToString());

            gg.DropDownStyle = ComboBoxStyle.DropDownList;
            gg.Text = "0";
        }

        /// <summary>
        /// sıralama kutusunu doldurur, varsayılan en yeni eklenen
        /// </summary>
        public static void FillSort(ComboBox gg)
        {
            gg.Items.AddRange(sortNames);
            gg.DropDownStyle = ComboBoxStyle.DropDownList;
            gg.SelectedIndex = 0;
        }

        public static SortMode ToSort(ComboBox gg) => (SortMode)Math.Max(0, gg.SelectedIndex);

        /// <summary>
        /// puanı dolu ve boş yıldızlarla gösterir (örn. 3 puan = ★★★☆☆)
        /// </summary>
        public static string Stars(int rate)
        {
            int gg = Math.Max(0, Math.Min(maxRate, rate));
            return new string('★', gg) + new string('☆', maxRate - gg);
        }

        #region Tema

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string subAppName, string subIdList);

        public static Font F(float size, FontStyle style = FontStyle.Regular) => new Font(fontName, size, style);

        /// <summary>
        /// formu ve içindeki tüm kontrolleri (iç içe olanlar dahil) koyu temaya çevirir
        /// konumlara dokunmaz, sadece renk, yazı tipi ve düğme biçimi verir
        /// </summary>
        public static void ApplyTheme(Control root)
        {
            Form form = root as Form;

            if (form != null)
            {
                form.BackColor = bgColor;
                form.ForeColor = textColor;
                form.Font = F(10f);
                DarkTitleBar(form);
            }

            foreach (Control gg in root.Controls) StyleControl(gg);
        }

        private static void StyleControl(Control c)
        {
            if (c is Button) StyleByName((Button)c);
            else if (c is LinkLabel) StyleLink((LinkLabel)c);
            else if (c is Label) StyleLabel((Label)c);
            else if (c is TextBox) StyleInput((TextBox)c);
            else if (c is ListBox) StyleList((ListBox)c);
            else if (c is ComboBox) StyleCombo((ComboBox)c);
            else if (c is CheckBox) StyleCheck((CheckBox)c);
            else if (c is PictureBox) c.BackColor = surfaceColor;
            else if (c is Panel) { c.BackColor = bgColor; DarkScroll(c); }

            foreach (Control gg in c.Controls) StyleControl(gg); // paneller ve gruplar için
        }

        /// <summary>
        /// designer'daki düğmelerin rolü adından anlaşılır: birincil, kart, gezinme oku veya normal
        /// </summary>
        private static void StyleByName(Button b)
        {
            switch (b.Name)
            {
                case "Search":
                case "New":
                case "Save":
                case "SearchButton":
                    Primary(b);
                    break;
                case "BackImage":
                case "NextImage":
                    Ghost(b);
                    b.Font = F(18f, FontStyle.Bold);
                    break;
                default:
                    if (b.Name.StartsWith("Shelf")) StyleCard(b);
                    else Normal(b);
                    break;
            }
        }

        public static void StyleButton(Button b, Color back, Color hover, Color fore)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.UseVisualStyleBackColor = false;
            b.AutoSize = false; // designer'daki otomatik boyut yerleşimi bozmasın
            b.BackColor = back;
            b.ForeColor = fore;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = hover;
            b.FlatAppearance.MouseDownBackColor = hover;
            b.Cursor = Cursors.Hand;
            b.Font = F(10f, FontStyle.Bold);
        }

        public static void Primary(Button b) => StyleButton(b, accentColor, accentHoverColor, Color.White);

        public static void Normal(Button b) => StyleButton(b, surface2Color, borderColor, textColor);

        public static void Ghost(Button b) => StyleButton(b, surfaceColor, surface2Color, mutedColor);

        public static void Danger(Button b) => StyleButton(b, dangerColor, dangerHoverColor, Color.White);

        /// <summary>
        /// kart düğmesi: soldan hizalı yazı, solda kapak resmi, kenarlık zemin renginde olduğu için kartlar arasında boşluk gibi görünür
        /// </summary>
        public static void StyleCard(Button b)
        {
            StyleButton(b, surfaceColor, surface2Color, textColor);
            b.Font = F(13f, FontStyle.Bold);
            b.FlatAppearance.BorderSize = 4;
            b.FlatAppearance.BorderColor = bgColor;
            b.TextAlign = ContentAlignment.MiddleLeft;
            b.ImageAlign = ContentAlignment.MiddleLeft;
            b.TextImageRelation = TextImageRelation.ImageBeforeText;
            b.Padding = new Padding(8, 0, 8, 0);
        }

        private static void StyleLabel(Label l)
        {
            l.BackColor = Color.Transparent;
            l.ForeColor = mutedColor;
            l.Font = F(10f);
        }

        private static void StyleLink(LinkLabel l)
        {
            l.BackColor = Color.Transparent;
            l.LinkColor = accentHoverColor;
            l.ActiveLinkColor = accentColor;
            l.VisitedLinkColor = accentHoverColor;
            l.Font = F(11f);
        }

        private static void StyleInput(TextBox t)
        {
            t.BackColor = surface2Color;
            t.ForeColor = textColor;
            t.BorderStyle = BorderStyle.FixedSingle;
            t.Font = F(11.5f);
            DarkScroll(t);
        }

        private static void StyleList(ListBox l)
        {
            l.BackColor = surface2Color;
            l.ForeColor = textColor;
            l.BorderStyle = BorderStyle.None;
            l.Font = F(11f);
            DarkScroll(l);
        }

        private static void StyleCombo(ComboBox c)
        {
            c.FlatStyle = FlatStyle.Flat;
            c.BackColor = surface2Color;
            c.ForeColor = textColor;
            c.Font = F(10.5f);
        }

        private static void StyleCheck(CheckBox c)
        {
            c.BackColor = Color.Transparent;
            c.ForeColor = textColor;
            c.Font = F(10f);
            c.AutoSize = true;
        }

        /// <summary>
        /// başlık çubuğunu koyu yapar (Windows 10 sonrası), desteklemeyen sistemde sessizce atlanır
        /// </summary>
        private static void DarkTitleBar(Form f)
        {
            try
            {
                int on = 1;
                DwmSetWindowAttribute(f.Handle, 20, ref on, sizeof(int)); // Windows 11 ve 10 20H1+
                DwmSetWindowAttribute(f.Handle, 19, ref on, sizeof(int)); // eski Windows 10 sürümleri
            }
            catch (Exception) { }
        }

        /// <summary>
        /// kaydırma çubuklarını koyu yapar, handle oluşmadıysa oluşunca uygular
        /// </summary>
        private static void DarkScroll(Control c)
        {
            if (c.IsHandleCreated) SetDarkTheme(c);
            else c.HandleCreated += (s, e) => SetDarkTheme(c);
        }

        private static void SetDarkTheme(Control c)
        {
            try
            {
                SetWindowTheme(c.Handle, "DarkMode_Explorer", null);
            }
            catch (Exception) { }
        }

        #endregion

        public static Label MakeLabel(string text, int x, int y)
        {
            Label gg = new Label();
            gg.Text = text;
            gg.Location = new Point(x, y);
            gg.AutoSize = true;
            StyleLabel(gg);
            return gg;
        }

        public static Button MakeButton(string text, int x, int y, int w, int h, EventHandler click)
        {
            Button gg = new Button();
            gg.Text = text;
            gg.Location = new Point(x, y);
            gg.Size = new Size(w, h);
            Normal(gg);
            gg.Font = F(smallFont, FontStyle.Bold);
            gg.Click += click;
            return gg;
        }

        /// <summary>
        /// tag için küçük etiket (chip) üretir
        /// </summary>
        public static Label MakeChip(string text)
        {
            Label gg = new Label();
            gg.Text = text;
            gg.AutoSize = true;
            gg.Padding = new Padding(10, 4, 10, 4);
            gg.Margin = new Padding(0, 0, 6, 6);
            gg.BackColor = chipColor;
            gg.ForeColor = accentHoverColor;
            gg.Font = F(10f, FontStyle.Bold);
            return gg;
        }

        /// <summary>
        /// veri tabanından gelen resim baytlarını dosya kilitlemeden Image yapar
        /// Image.FromStream akışın açık kalmasını istediği için kopyası alınır
        /// </summary>
        public static Image ImageFromBytes(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (Image gg = Image.FromStream(ms)) return new Bitmap(gg);
        }

        /// <summary>
        /// resmi verilen kutuyu tamamen dolduracak şekilde büyütüp ortadan kırpar (oran bozulmaz)
        /// sağına thumbGap kadar saydam boşluk eklenir, böylece yazı resme yapışmaz
        /// </summary>
        public static Image MakeThumb(Image src, int w, int h)
        {
            Bitmap gg = new Bitmap(w + thumbGap, h);
            float ratio = Math.Max((float)w / src.Width, (float)h / src.Height); // taşan kısım kırpılır
            int sw = Math.Max(1, (int)(src.Width * ratio));
            int sh = Math.Max(1, (int)(src.Height * ratio));

            using (Graphics g = Graphics.FromImage(gg))
            {
                g.SetClip(new Rectangle(0, 0, w, h)); // kırpma sadece resim alanında, boşluk saydam kalır
                g.Clear(surface2Color);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, (w - sw) / 2, (h - sh) / 2, sw, sh);
            }

            return gg;
        }

        /// <summary>
        /// kart düğmesini kayıtla doldurur: başlık, altında yıldızlar, solda kapak resmi
        /// r null ise boş raf yazısı konur, önceki küçük resim bellekten atılır
        /// </summary>
        public static void FillCard(Button gg, Record r, byte[] cover, int thumbW, int thumbH)
        {
            gg.Image?.Dispose();
            gg.Image = null;

            if (r == null)
            {
                gg.Text = "Empty";
                return;
            }

            gg.Text = r.Title + Environment.NewLine + Stars(r.Rate);
            gg.Tag = r.Id;

            if (cover == null) return;

            try
            {
                using (Image im = ImageFromBytes(cover)) gg.Image = MakeThumb(im, thumbW, thumbH);
            }
            catch (Exception) { } // bozuk resim kartın açılmasını engellemesin
        }

        /// <summary>
        /// kayıt için liste kartı üretir (Dock.Top), butonun Tag'ine kayıt kimliği konur, başlığa göre değil kimliğe göre açılır
        /// </summary>
        public static Button CreateTitleButton(Record r, EventHandler click)
        {
            Button gg = new Button();
            StyleCard(gg);
            gg.Dock = DockStyle.Top;
            gg.Height = cardHeight;

            int thumbH = cardHeight - 16;
            FillCard(gg, r, ArchiveDB.Instance.GetImage(r.Id, 0), thumbH * 3 / 2, thumbH);

            gg.Click += click;
            return gg;
        }

        /// <summary>
        /// paneldeki butonları ve küçük resimlerini temizler, aramalar üst üste binmesin
        /// </summary>
        public static void ClearButtons(Control gg)
        {
            while (gg.Controls.Count > 0)
            {
                (gg.Controls[0] as Button)?.Image?.Dispose();
                gg.Controls[0].Dispose();
            }
        }

        /// <summary>
        /// kaydın görüntüleme formunu açar
        /// </summary>
        public static void OpenData(string id, Form from)
        {
            Data dataForm = new Data();
            dataForm.recordId = id;

            dataForm.StartPosition = FormStartPosition.Manual;
            dataForm.Location = from.Location;

            dataForm.Show();
        }

        #endregion
    }
}
