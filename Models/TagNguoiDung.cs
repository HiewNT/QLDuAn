using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class TagNguoiDung
{
    public string MaTag { get; set; } = null!;

    public string MaBinhLuan { get; set; } = null!;

    public string MaNguoiDuocTag { get; set; } = null!;

    public DateTime? NgayTag { get; set; }

    public virtual BinhLuan MaBinhLuanNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDuocTagNavigation { get; set; } = null!;
}
