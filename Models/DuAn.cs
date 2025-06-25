using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class DuAn
{
    public int MaDuAn { get; set; }

    public string TenDuAn { get; set; } = null!;

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public int NguoiPhuTrach { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual NguoiDung NguoiPhuTrachNavigation { get; set; } = null!;

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
