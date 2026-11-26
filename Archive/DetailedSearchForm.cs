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
    public partial class DetailedSearchForm : Form
    {
        public DetailedSearchForm()
        {
            InitializeComponent();
        }

        public List<string> tags = new List<string>();
        public SortMode startSort = SortMode.Newest; // ana ekranın seçimi varsayılan gelir
        public bool startDeepFlag = false;

        // yerleşim ayarları
        private int filterW = 330; // sol filtre sütununun genişliği
        private int searchH = 46;

        List<string> selectedTags = new List<string>();

        ComboBox sortBox;
        CheckBox deepBox;
        Label sortLabel;

        private void DetailedSearchForm_Load(object sender, EventArgs e)
        {
            ArchiveMain.FillRate(Rate);
            BuildExtraUI();

            ArchiveMain.ApplyTheme(this);
            StyleDetailed();
            LayoutDetailed();
            SizeChanged += (s, ev) => LayoutDetailed(); // pencere boyutu değişince yerleşim yenilenir

            UpdateListBoxes(); // tagleri ekler
        }

        /// <summary>
        /// designer'da olmayan sıralama ve açıklamada arama seçeneklerini ekler, konumları LayoutDetailed verir
        /// </summary>
        private void BuildExtraUI()
        {
            sortLabel = ArchiveMain.MakeLabel("Sırala", 0, 0);
            Controls.Add(sortLabel);

            sortBox = new ComboBox();
            ArchiveMain.FillSort(sortBox);
            sortBox.SelectedIndex = (int)startSort;
            Controls.Add(sortBox);

            deepBox = new CheckBox();
            deepBox.Text = "Açıklama ve linkte de ara";
            deepBox.Checked = startDeepFlag;
            Controls.Add(deepBox);
        }

        private void StyleDetailed()
        {
            SearchBar.Font = ArchiveMain.F(14f);
            rateLabel.Text = "En düşük puan";
            TagsLabel.Text = "Taglar  (tıkla: ekle / çıkar)";
            rateLabel.AutoSize = true;
            TagsLabel.AutoSize = true;

            SearchButton.Font = ArchiveMain.F(12f, FontStyle.Bold);
            Panel.AutoScroll = true; // sonuç sığmazsa kaydırılabilsin
            Panel.Padding = new Padding(0, 0, ArchiveMain.gap, 0);
        }

        /// <summary>
        /// solda filtre sütunu (arama, puan, sıralama, taglar, arama butonu), sağda sonuç kartları
        /// </summary>
        private void LayoutDetailed()
        {
            if (sortBox == null) return; // yükleme bitmeden çalışmasın

            int w = ClientSize.Width;
            int h = ClientSize.Height;
            int pad = ArchiveMain.pad;
            int gap = ArchiveMain.gap;
            int cw = filterW - pad; // sütun içi genişlik

            SearchBar.SetBounds(pad, pad, cw, SearchBar.Height);

            rateLabel.Location = new Point(pad, SearchBar.Bottom + gap + 4);
            Rate.SetBounds(rateLabel.Right + gap, SearchBar.Bottom + gap, 70, Rate.Height);

            sortLabel.Location = new Point(pad, Rate.Bottom + gap + 4);
            sortBox.SetBounds(pad, sortLabel.Bottom + 4, cw, sortBox.Height);

            deepBox.Location = new Point(pad, sortBox.Bottom + gap);

            TagsLabel.Location = new Point(pad, deepBox.Bottom + gap);

            SearchButton.SetBounds(pad, h - pad - searchH, cw, searchH);

            int listTop = TagsLabel.Bottom + 4;
            int half = (cw - gap) / 2;
            Tags.SetBounds(pad, listTop, half, Math.Max(40, SearchButton.Top - gap - listTop));
            SelectedTags.SetBounds(pad + half + gap, listTop, half, Tags.Height);

            Panel.Dock = DockStyle.None;
            Panel.SetBounds(filterW + pad, pad, Math.Max(100, w - filterW - 2 * pad), Math.Max(100, h - 2 * pad));
        }

        ////////////////////// arama
        /// <summary>
        /// başlık metnine, en düşük puana ve seçilen taglerin hepsine göre süzüp seçili sırayla buton olarak listeler
        /// arama kutusu boşsa tüm kayıtlardan başlar
        /// </summary>
        private void SearchButton_Click(object sender, EventArgs e)
        {
            int minRate;
            int.TryParse(Rate.Text, out minRate);

            List<Record> found;

            try
            {
                found = ArchiveDB.Instance.Search(SearchBar.Text, deepBox.Checked, minRate, selectedTags, ArchiveMain.ToSort(sortBox));
            }
            catch (Exception ex)
            {
                ArchiveMain.ShowError("Hata: " + ex.Message);
                return;
            }

            ArchiveMain.ClearButtons(Panel); // önceki aramanın butonları kalmasın

            if (found.Count == 0)
            {
                ArchiveMain.ShowError("Istenilen kriterlede eleman bulunamadı ");
                return;
            }

            // Dock.Top kartlar sondan başa dizilir, o yüzden ters sırayla eklenir ki liste seçilen sıralamayla aynı görünsün
            foreach (Record r in Enumerable.Reverse(found)) Panel.Controls.Add(ArchiveMain.CreateTitleButton(r, Button_Click));
        }

        private void Button_Click(object sender, EventArgs e) => ArchiveMain.OpenData((string)((Button)sender).Tag, this);

        ////////////////////// taglar
        private void Tags_SelectedIndexChanged(object sender, EventArgs e) => MoveTag(tags, selectedTags, Tags.SelectedIndex);

        private void SelectedTags_SelectedIndexChanged_1(object sender, EventArgs e) => MoveTag(selectedTags, tags, SelectedTags.SelectedIndex);

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
    }
}
