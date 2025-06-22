using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class ThongBao
{
    public string MaThongBao { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string? MaNguoiGui { get; set; }

    public string MaNguoiNhan { get; set; } = null!;

    public DateTime? NgayGui { get; set; }

    public bool? DaDoc { get; set; }

    public virtual NguoiDung? MaNguoiGuiNavigation { get; set; }

    public virtual NguoiDung MaNguoiNhanNavigation { get; set; } = null!;
}
