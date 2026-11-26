using System;
using System.Collections.Generic;

namespace Archive
{
    /// <summary>
    /// arşivdeki tek bir kaydın verisi, veri tabanı ile formlar arasında taşınır
    /// Images sadece kayıt/düzenleme sırasında dolar, listelerde boş gelir (bellek şişmesin)
    /// </summary>
    public class Record
    {
        public string Id; // hash, başlık değişse de değişmez
        public string Title = "";
        public int Rate;
        public string Link = "";
        public string Info = "";
        public long Created; // yüklenme sırası için UTC ticks
        public List<string> Tags = new List<string>();
        public List<byte[]> Images = new List<byte[]>(); // sıra 0 = kapak resmi
    }
}
