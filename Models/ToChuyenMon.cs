using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class ToChuyenMon
{
    public string MaTo { get; set; } = null!;

    public string TenTo { get; set; } = null!;

    public string? MoTa { get; set; }

    public int? SoLuongNhanVien { get; set; }

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
