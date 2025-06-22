using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class VaiTro
{
    public string MaVaiTro { get; set; } = null!;

    public string TenVaiTro { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

    public virtual ICollection<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; } = new List<PhanQuyenTaiLieu>();
}
