using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class TrangThaiCongViec
{
    public string MaTrangThaiCv { get; set; } = null!;

    public string TenTrangThai { get; set; } = null!;

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();
}
