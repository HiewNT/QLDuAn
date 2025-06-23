using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class VaiTro
{
    public int MaVaiTro { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

    public virtual ICollection<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; } = new List<PhanQuyenTaiLieu>();
}
