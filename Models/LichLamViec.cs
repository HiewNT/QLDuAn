using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class LichLamViec
{
    public int MaLich { get; set; }

    public string TieuDe { get; set; } = null!;

    public DateOnly Ngay { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public int MaDuAn { get; set; }

    public virtual DuAn MaDuAnNavigation { get; set; } = null!;
}
