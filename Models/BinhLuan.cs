using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class BinhLuan
{
    public int MaBinhLuan { get; set; }

    public string NoiDung { get; set; } = null!;

    public DateTime? NgayTao { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaCongViec { get; set; }

    public virtual CongViec MaCongViecNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
