using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class ToChuyenMon
{
    public int MaTo { get; set; }

    public string TenTo { get; set; } = null!;

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
