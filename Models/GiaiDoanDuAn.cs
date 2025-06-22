using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class GiaiDoanDuAn
{
    public string MaGiaiDoanDuAn { get; set; } = null!;

    public string MaDuAn { get; set; } = null!;

    public string TenGiaiDoan { get; set; } = null!;

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public int? ThuTu { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual DuAn MaDuAnNavigation { get; set; } = null!;
}
