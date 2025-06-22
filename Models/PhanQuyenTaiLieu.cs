using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class PhanQuyenTaiLieu
{
    public string MaPhanQuyenTl { get; set; } = null!;

    public string MaTaiLieu { get; set; } = null!;

    public string? MaVaiTro { get; set; }

    public string QuyenTruyCap { get; set; } = null!;

    public virtual TaiLieu MaTaiLieuNavigation { get; set; } = null!;

    public virtual VaiTro? MaVaiTroNavigation { get; set; }
}
