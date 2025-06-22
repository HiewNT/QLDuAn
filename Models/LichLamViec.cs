using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class LichLamViec
{
    public string MaLich { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string? MoTa { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public string? MaCongViec { get; set; }

    public string? DiaDiem { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual CongViec? MaCongViecNavigation { get; set; }
}
