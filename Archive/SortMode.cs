using System;
using System.Collections.Generic;

namespace Archive
{
    /// <summary>
    /// liste sıralama seçenekleri, sıra ArchiveMain.sortNames ile aynı olmalı
    /// </summary>
    public enum SortMode
    {
        Newest, // yüklenme sırası, yeniden eskiye
        Oldest, // yüklenme sırası, eskiden yeniye
        Rate, // puan yüksekten düşüğe
        Title // başlık A-Z
    }
}
