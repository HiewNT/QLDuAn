using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class BaoCao
{
    public string MaBaoCao { get; set; } = null!;

    public string TenBaoCao { get; set; } = null!;

    public string MaNguoiTao { get; set; } = null!;

    public string? NoiDung { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? DuongDanFile { get; set; }

    public virtual NguoiDung MaNguoiTaoNavigation { get; set; } = null!;
}
