using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class PhanQuyenTaiLieu
{
    public int MaPhanQuyen { get; set; }

    public int MaTaiLieu { get; set; }

    public int MaVaiTro { get; set; }

    public string QuyenTruyCap { get; set; } = null!;

    public virtual TaiLieu MaTaiLieuNavigation { get; set; } = null!;

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
