using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class TaiLieu
{
    public string MaTaiLieu { get; set; } = null!;

    public string TenTaiLieu { get; set; } = null!;

    public string DuongDan { get; set; } = null!;

    public string? LoaiFile { get; set; }

    public long? KichThuoc { get; set; }

    public string? MaDuAn { get; set; }

    public string? MaCongViec { get; set; }

    public string MaNguoiTai { get; set; } = null!;

    public DateTime? NgayTai { get; set; }

    public virtual CongViec? MaCongViecNavigation { get; set; }

    public virtual DuAn? MaDuAnNavigation { get; set; }

    public virtual NguoiDung MaNguoiTaiNavigation { get; set; } = null!;

    public virtual ICollection<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; } = new List<PhanQuyenTaiLieu>();
}
