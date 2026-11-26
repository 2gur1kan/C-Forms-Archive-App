using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Archive
{
    /// <summary>
    /// arşivin tüm veri işlemleri burada toplanır, veri tek bir SQLite dosyasındadır (Archive\Archive.db)
    /// - kayıt kimliği hash'tir, başlık değişse de kimlik ve bağlantılar bozulmaz
    /// - resimler içerik hash'i ile bir kez saklanır, aynı resim iki kayda eklenirse tek kopya tutulur
    /// - her işlem kendi bağlantısını açıp kapatır, dosya kilitli kalmaz (yedekten dönüş bu yüzden kolay)
    /// - veri değişince Changed olayı tetiklenir, açık formlar kendini yeniler
    /// </summary>
    public class ArchiveDB
    {
        public static ArchiveDB Instance { get => instance ?? (instance = new ArchiveDB()); }
        private static ArchiveDB instance;

        public event Action Changed;

        ////////////////////// ayarlar
        public static string dbFileName = "Archive.db";
        public static int maxImageSize = 1600; // bundan büyük resimler bu ölçüye küçültülür
        public static long jpegQuality = 85L;
        public static string[] defaultTags = { "2016", "2017", "2018", "2019", "2020" }; // ilk açılışta oluşan tagler

        // eski dosya yapısı, sadece ilk açılışta veri aktarmak için okunur
        private static string oldDataName = "Data.txt";
        private static string oldInfoName = "Info.txt";
        private static string oldTagsName = "Tags.txt";
        private static string oldImagesName = "Images";

        public int ImportSkipped; // aktarımda alınamayan kayıt sayısı

        public string DbFile { get => Path.Combine(Environment.CurrentDirectory, ArchiveMain.archivePath + dbFileName); }
        private string ConnString { get => "Data Source=" + DbFile + ";Version=3;Pooling=False;"; } // Pooling kapalı, dosya kapanınca serbest kalır

        ////////////////////// bağlantı yardımcıları
        private SQLiteConnection Connect()
        {
            SQLiteConnection gg = new SQLiteConnection(ConnString);
            gg.Open();
            Run(gg, "PRAGMA foreign_keys = ON"); // silme ve yeniden adlandırma bağlı tablolara yayılsın
            return gg;
        }

        /// <summary>
        /// parametreler sırayla @p0, @p1 ... olarak sorguya bağlanır
        /// </summary>
        private SQLiteCommand Cmd(SQLiteConnection conn, string sql, params object[] args)
        {
            SQLiteCommand gg = new SQLiteCommand(sql, conn);
            for (int i = 0; i < args.Length; i++) gg.Parameters.AddWithValue("@p" + i, args[i]);
            return gg;
        }

        private int Run(SQLiteConnection conn, string sql, params object[] args)
        {
            using (SQLiteCommand gg = Cmd(conn, sql, args)) return gg.ExecuteNonQuery();
        }

        private object Scalar(SQLiteConnection conn, string sql, params object[] args)
        {
            using (SQLiteCommand gg = Cmd(conn, sql, args)) return gg.ExecuteScalar();
        }

        private List<T> Query<T>(Func<SQLiteDataReader, T> map, string sql, params object[] args)
        {
            List<T> gg = new List<T>();

            using (SQLiteConnection conn = Connect())
            using (SQLiteCommand cmd = Cmd(conn, sql, args))
            using (SQLiteDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read()) gg.Add(map(rd));
            }

            return gg;
        }

        private static Record ReadRecord(SQLiteDataReader rd)
        {
            Record gg = new Record();
            gg.Id = rd.GetString(0);
            gg.Title = rd.GetString(1);
            gg.Rate = rd.GetInt32(2);
            gg.Link = rd.GetString(3);
            gg.Info = rd.GetString(4);
            gg.Created = rd.GetInt64(5);
            return gg;
        }

        private void RaiseChanged() => Changed?.Invoke();

        ////////////////////// açılış
        /// <summary>
        /// veri tabanını açar, yoksa oluşturur
        /// ilk oluşturulurken eski klasör yapısı (Titles.txt + Data klasörleri) varsa içindekileri aktarır
        /// dönen sayı aktarılan kayıt sayısıdır
        /// </summary>
        public int Open()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DbFile));

            bool freshFlag = !File.Exists(DbFile);
            EnsureSchema();

            if (!freshFlag) return 0;
            return ImportOldArchive();
        }

        private void EnsureSchema()
        {
            using (SQLiteConnection conn = Connect())
            {
                Run(conn, @"
                    CREATE TABLE IF NOT EXISTS records(
                        id TEXT PRIMARY KEY,
                        title TEXT NOT NULL UNIQUE COLLATE NOCASE,
                        rate INTEGER NOT NULL DEFAULT 0,
                        link TEXT NOT NULL DEFAULT '',
                        info TEXT NOT NULL DEFAULT '',
                        created INTEGER NOT NULL);
                    CREATE TABLE IF NOT EXISTS tags(
                        name TEXT PRIMARY KEY COLLATE NOCASE);
                    CREATE TABLE IF NOT EXISTS record_tags(
                        record_id TEXT NOT NULL REFERENCES records(id) ON DELETE CASCADE,
                        tag TEXT NOT NULL COLLATE NOCASE REFERENCES tags(name) ON DELETE CASCADE ON UPDATE CASCADE,
                        PRIMARY KEY(record_id, tag));
                    CREATE TABLE IF NOT EXISTS images(
                        hash TEXT PRIMARY KEY,
                        data BLOB NOT NULL);
                    CREATE TABLE IF NOT EXISTS record_images(
                        record_id TEXT NOT NULL REFERENCES records(id) ON DELETE CASCADE,
                        pos INTEGER NOT NULL,
                        image_hash TEXT NOT NULL REFERENCES images(hash),
                        PRIMARY KEY(record_id, pos));
                    CREATE INDEX IF NOT EXISTS idx_records_created ON records(created);");
            }
        }

        ////////////////////// kayıtlar
        /// <summary>
        /// yeni kayıt ekler ve kimliğini (hash) döner, başlık aynı ada sahipse hata fırlatır
        /// Created boşsa şimdiki zaman yazılır
        /// </summary>
        public string AddRecord(Record r)
        {
            r.Id = Hash(Encoding.UTF8.GetBytes(r.Title + "|" + DateTime.UtcNow.Ticks + "|" + Guid.NewGuid()));
            if (r.Created <= 0) r.Created = DateTime.UtcNow.Ticks;

            using (SQLiteConnection conn = Connect())
            using (SQLiteTransaction tx = conn.BeginTransaction())
            {
                Run(conn, "INSERT INTO records(id, title, rate, link, info, created) VALUES(@p0, @p1, @p2, @p3, @p4, @p5)", r.Id, r.Title, r.Rate, r.Link, r.Info, r.Created);
                WriteLinks(conn, r);
                tx.Commit();
            }

            RaiseChanged();
            return r.Id;
        }

        /// <summary>
        /// kaydı günceller, tag ve resim listesi r içindekiyle birebir değiştirilir, yüklenme zamanı korunur
        /// </summary>
        public void UpdateRecord(Record r)
        {
            using (SQLiteConnection conn = Connect())
            using (SQLiteTransaction tx = conn.BeginTransaction())
            {
                Run(conn, "UPDATE records SET title = @p1, rate = @p2, link = @p3, info = @p4 WHERE id = @p0", r.Id, r.Title, r.Rate, r.Link, r.Info);
                WriteLinks(conn, r);
                tx.Commit();
            }

            RaiseChanged();
        }

        public void DeleteRecord(string id)
        {
            using (SQLiteConnection conn = Connect())
            using (SQLiteTransaction tx = conn.BeginTransaction())
            {
                Run(conn, "DELETE FROM records WHERE id = @p0", id); // tag ve resim bağlantıları birlikte silinir
                CleanImages(conn);
                tx.Commit();
            }

            RaiseChanged();
        }

        /// <summary>
        /// kaydın tag ve resim bağlantılarını r'ye göre baştan yazar
        /// </summary>
        private void WriteLinks(SQLiteConnection conn, Record r)
        {
            Run(conn, "DELETE FROM record_tags WHERE record_id = @p0", r.Id);
            Run(conn, "DELETE FROM record_images WHERE record_id = @p0", r.Id);

            foreach (string tag in r.Tags)
            {
                Run(conn, "INSERT OR IGNORE INTO tags(name) VALUES(@p0)", tag);
                Run(conn, "INSERT OR IGNORE INTO record_tags(record_id, tag) VALUES(@p0, @p1)", r.Id, tag);
            }

            for (int i = 0; i < r.Images.Count; i++)
            {
                string hash = Hash(r.Images[i]);
                Run(conn, "INSERT OR IGNORE INTO images(hash, data) VALUES(@p0, @p1)", hash, r.Images[i]);
                Run(conn, "INSERT INTO record_images(record_id, pos, image_hash) VALUES(@p0, @p1, @p2)", r.Id, i, hash);
            }

            CleanImages(conn);
        }

        /// <summary>
        /// hiçbir kayda bağlı olmayan resimleri siler, dosya şişmesin
        /// </summary>
        private void CleanImages(SQLiteConnection conn) => Run(conn, "DELETE FROM images WHERE hash NOT IN (SELECT image_hash FROM record_images)");

        /// <summary>
        /// kaydı tag'leriyle birlikte getirir, resimler getirilmez (GetImage / GetImages kullanılır), yoksa null
        /// </summary>
        public Record GetRecord(string id)
        {
            List<Record> gg = Query<Record>(ReadRecord, "SELECT id, title, rate, link, info, created FROM records WHERE id = @p0", id);
            if (gg.Count == 0) return null;

            gg[0].Tags = Query<string>(rd => rd.GetString(0), "SELECT tag FROM record_tags WHERE record_id = @p0 ORDER BY tag", id);
            return gg[0];
        }

        /// <summary>
        /// başlık başka bir kayıtta var mı (düzenlerken kendi kaydı ignoreId ile sayılmaz)
        /// </summary>
        public bool TitleExists(string title, string ignoreId = null)
        {
            using (SQLiteConnection conn = Connect()) return Convert.ToInt32(Scalar(conn, "SELECT COUNT(*) FROM records WHERE title = @p0 AND id <> @p1", title, ignoreId ?? "")) > 0;
        }

        ////////////////////// arama
        /// <summary>
        /// kayıtları süzer ve sıralar
        /// text boşsa tümü gelir, deepFlag açıksa açıklama ve linkte de aranır
        /// minRate bu puan ve üstünü getirir, tags verilenlerin hepsine sahip kayıtları getirir
        /// metin eşleşmesi büyük küçük harfe duyarsızdır
        /// </summary>
        public List<Record> Search(string text, bool deepFlag, int minRate, List<string> tags, SortMode sort)
        {
            StringBuilder sql = new StringBuilder("SELECT r.id, r.title, r.rate, r.link, r.info, r.created FROM records r WHERE r.rate >= @p0");
            List<object> args = new List<object> { minRate };

            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    args.Add(tag);
                    sql.Append(" AND EXISTS(SELECT 1 FROM record_tags t WHERE t.record_id = r.id AND t.tag = @p" + (args.Count - 1) + ")");
                }
            }

            sql.Append(" ORDER BY " + OrderBy(sort));
            List<Record> all = Query<Record>(ReadRecord, sql.ToString(), args.ToArray());

            if (string.IsNullOrWhiteSpace(text)) return all;

            string gg = text.Trim();
            return all.Where(rc => Has(rc.Title, gg) || (deepFlag && (Has(rc.Info, gg) || Has(rc.Link, gg)))).ToList();
        }

        private static bool Has(string source, string part) => source.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;

        private static string OrderBy(SortMode sort)
        {
            switch (sort)
            {
                case SortMode.Oldest: return "r.created ASC, r.rowid ASC";
                case SortMode.Rate: return "r.rate DESC, r.created DESC";
                case SortMode.Title: return "r.title COLLATE NOCASE ASC";
                default: return "r.created DESC, r.rowid DESC";
            }
        }

        ////////////////////// resimler
        public int ImageCount(string id)
        {
            using (SQLiteConnection conn = Connect()) return Convert.ToInt32(Scalar(conn, "SELECT COUNT(*) FROM record_images WHERE record_id = @p0", id));
        }

        /// <summary>
        /// kaydın pos. sıradaki resmini döner, yoksa null (0. sıra kapaktır)
        /// </summary>
        public byte[] GetImage(string id, int pos)
        {
            using (SQLiteConnection conn = Connect()) return Scalar(conn, "SELECT i.data FROM record_images ri JOIN images i ON i.hash = ri.image_hash WHERE ri.record_id = @p0 AND ri.pos = @p1", id, pos) as byte[];
        }

        public List<byte[]> GetImages(string id) => Query<byte[]>(rd => (byte[])rd[0], "SELECT i.data FROM record_images ri JOIN images i ON i.hash = ri.image_hash WHERE ri.record_id = @p0 ORDER BY ri.pos", id);

        /// <summary>
        /// resim dosyasını saklanacak hale getirir
        /// maxImageSize'dan büyükse oranı bozmadan küçültüp JPEG yapar, küçükse dosya olduğu gibi alınır
        /// resim olmayan dosyada hata fırlatır
        /// </summary>
        public static byte[] PrepareImage(string file)
        {
            byte[] raw = File.ReadAllBytes(file);

            using (MemoryStream ms = new MemoryStream(raw))
            using (Image src = Image.FromStream(ms))
            {
                if (src.Width <= maxImageSize && src.Height <= maxImageSize) return raw;

                float ratio = Math.Min((float)maxImageSize / src.Width, (float)maxImageSize / src.Height);
                int w = Math.Max(1, (int)(src.Width * ratio));
                int h = Math.Max(1, (int)(src.Height * ratio));

                using (Bitmap bmp = new Bitmap(w, h))
                {
                    using (Graphics gg = Graphics.FromImage(bmp))
                    {
                        gg.Clear(Color.White); // şeffaf resim JPEG'de siyah kalmasın
                        gg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gg.DrawImage(src, 0, 0, w, h);
                    }

                    return EncodeJpeg(bmp);
                }
            }
        }

        private static byte[] EncodeJpeg(Bitmap bmp)
        {
            ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == "image/jpeg");

            using (EncoderParameters ep = new EncoderParameters(1))
            using (MemoryStream ms = new MemoryStream())
            {
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);
                bmp.Save(ms, codec, ep);
                return ms.ToArray();
            }
        }

        public static string Hash(byte[] data)
        {
            using (SHA256 gg = SHA256.Create()) return BitConverter.ToString(gg.ComputeHash(data)).Replace("-", "").ToLowerInvariant();
        }

        ////////////////////// taglar
        public List<string> GetTags() => Query<string>(rd => rd.GetString(0), "SELECT name FROM tags ORDER BY name COLLATE NOCASE");

        /// <summary>
        /// tag ekler, zaten varsa false döner
        /// </summary>
        public bool AddTag(string name)
        {
            int count;
            using (SQLiteConnection conn = Connect()) count = Run(conn, "INSERT OR IGNORE INTO tags(name) VALUES(@p0)", name);

            if (count > 0) RaiseChanged();
            return count > 0;
        }

        /// <summary>
        /// tagi yeniden adlandırır, o tagi taşıyan tüm kayıtlar otomatik güncellenir, yeni ad varsa hata fırlatır
        /// </summary>
        public void RenameTag(string oldName, string newName)
        {
            using (SQLiteConnection conn = Connect()) Run(conn, "UPDATE tags SET name = @p1 WHERE name = @p0", oldName, newName);
            RaiseChanged();
        }

        /// <summary>
        /// tagi siler, kayıtlardan da kalkar
        /// </summary>
        public void DeleteTag(string name)
        {
            using (SQLiteConnection conn = Connect()) Run(conn, "DELETE FROM tags WHERE name = @p0", name);
            RaiseChanged();
        }

        public int TagUseCount(string name)
        {
            using (SQLiteConnection conn = Connect()) return Convert.ToInt32(Scalar(conn, "SELECT COUNT(*) FROM record_tags WHERE tag = @p0", name));
        }

        ////////////////////// yedek
        /// <summary>
        /// tüm arşivi (kayıtlar, taglar, resimler) tek bir .db dosyasına yedekler, var olan dosyanın üstüne yazar
        /// </summary>
        public void Backup(string path)
        {
            if (File.Exists(path)) File.Delete(path); // VACUUM INTO var olan dosyaya yazmaz

            using (SQLiteConnection conn = Connect()) Run(conn, "VACUUM INTO @p0", path);
        }

        /// <summary>
        /// yedekten geri yükler, önce yedeğin geçerli bir arşiv olduğunu kontrol eder
        /// mevcut arşiv Archive.db.bak olarak saklanır, bozuk dosya yüklenirse hata fırlatır ve hiçbir şey değişmez
        /// </summary>
        public void Restore(string path)
        {
            if (Path.GetFullPath(path) == Path.GetFullPath(DbFile)) throw new InvalidOperationException("Seçilen dosya zaten kullanımdaki arşiv.");

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + path + ";Version=3;Pooling=False;"))
                {
                    conn.Open();
                    Scalar(conn, "SELECT COUNT(*) FROM records");
                    Scalar(conn, "SELECT COUNT(*) FROM tags");
                    Scalar(conn, "SELECT COUNT(*) FROM record_tags");
                    Scalar(conn, "SELECT COUNT(*) FROM record_images");
                    Scalar(conn, "SELECT COUNT(*) FROM images");
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException("Seçilen dosya geçerli bir arşiv yedeği değil.");
            }

            if (File.Exists(DbFile)) File.Copy(DbFile, DbFile + ".bak", true);
            File.Copy(path, DbFile, true);

            EnsureSchema();
            RaiseChanged();
        }

        ////////////////////// eski yapıdan aktarım
        /// <summary>
        /// eski klasör yapısındaki (Titles.txt, Data\başlık\...) arşivi veri tabanına aktarır
        /// eski dosyalara dokunulmaz, sadece okunur, eski yapı yoksa sadece varsayılan tagler açılır
        /// </summary>
        private int ImportOldArchive()
        {
            string tagsFile = Path.Combine(Environment.CurrentDirectory, ArchiveMain.archivePath + ArchiveMain.tagsDataBaseName);
            string[] tagNames = File.Exists(tagsFile) ? File.ReadAllLines(tagsFile) : defaultTags;

            foreach (string tag in tagNames.Where(ln => !string.IsNullOrWhiteSpace(ln))) AddTag(tag.Trim());

            if (!File.Exists(ArchiveMain.TitleFile)) return 0;

            int count = 0;
            ImportSkipped = 0;

            foreach (string title in File.ReadAllLines(ArchiveMain.TitleFile).Where(ln => !string.IsNullOrWhiteSpace(ln)))
            {
                try
                {
                    ImportOldRecord(title.Trim());
                    count++;
                }
                catch (Exception)
                {
                    ImportSkipped++; // klasörü eksik veya başlığı tekrar eden kayıt atlanır
                }
            }

            return count;
        }

        private void ImportOldRecord(string title)
        {
            string folder = Path.Combine(ArchiveMain.DataFolder, title);
            if (!Directory.Exists(folder)) throw new DirectoryNotFoundException(folder);

            Record gg = new Record();
            gg.Title = title;
            gg.Created = Directory.GetCreationTimeUtc(folder).Ticks; // yüklenme sırası klasörün açılış zamanından gelir

            string[] lines = ReadOldLines(folder, oldDataName); // sıra: rate, başlık, link
            int rate;
            int.TryParse(lines.Length > 0 ? lines[0] : "", out rate);
            gg.Rate = Math.Max(0, Math.Min(ArchiveMain.maxRate, rate));
            gg.Link = lines.Length > 2 ? lines[2].Trim() : "";

            string infoFile = Path.Combine(folder, oldInfoName);
            gg.Info = File.Exists(infoFile) ? File.ReadAllText(infoFile).TrimEnd() : "";

            gg.Tags = ReadOldLines(folder, oldTagsName).Where(ln => !string.IsNullOrWhiteSpace(ln)).Select(ln => ln.Trim()).ToList();

            for (int i = 0; File.Exists(Path.Combine(folder, oldImagesName, i + ".jpg")); i++) gg.Images.Add(PrepareImage(Path.Combine(folder, oldImagesName, i + ".jpg")));

            AddRecord(gg);
        }

        private string[] ReadOldLines(string folder, string fileName)
        {
            string gg = Path.Combine(folder, fileName);
            return File.Exists(gg) ? File.ReadAllLines(gg) : new string[0];
        }
    }
}
