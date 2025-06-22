using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class TrangThaiDuAn
{
    public string MaTrangThai { get; set; } = null!;

    public string TenTrangThai { get; set; } = null!;

    public virtual ICollection<DuAn> DuAns { get; set; } = new List<DuAn>();
}
