using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class BinhLuan
{
    public string MaBinhLuan { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string? MaDuAn { get; set; }

    public string? MaCongViec { get; set; }

    public string MaNguoiBinhLuan { get; set; } = null!;

    public DateTime? NgayBinhLuan { get; set; }

    public string? MaBinhLuanCha { get; set; }

    public virtual CongViec? MaCongViecNavigation { get; set; }

    public virtual DuAn? MaDuAnNavigation { get; set; }

    public virtual NguoiDung MaNguoiBinhLuanNavigation { get; set; } = null!;

    public virtual ICollection<TagNguoiDung> TagNguoiDungs { get; set; } = new List<TagNguoiDung>();
}
