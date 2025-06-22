using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class DuAn
{
    public string MaDuAn { get; set; } = null!;

    public string TenDuAn { get; set; } = null!;

    public string? MoTa { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public string MaTrangThai { get; set; } = null!;

    public string MaNguoiPhuTrach { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual ICollection<GiaiDoanDuAn> GiaiDoanDuAns { get; set; } = new List<GiaiDoanDuAn>();

    public virtual NguoiDung MaNguoiPhuTrachNavigation { get; set; } = null!;

    public virtual TrangThaiDuAn MaTrangThaiNavigation { get; set; } = null!;

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();
}
