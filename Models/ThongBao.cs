using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class ThongBao
{
    public int MaThongBao { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public int MaNguoiDung { get; set; }

    public int? MaDuAn { get; set; }

    public int? MaCongViec { get; set; }

    public bool? DaDoc { get; set; }

    public virtual CongViec? MaCongViecNavigation { get; set; }

    public virtual DuAn? MaDuAnNavigation { get; set; }

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
