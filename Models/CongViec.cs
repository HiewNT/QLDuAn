using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class CongViec
{
    public string MaCongViec { get; set; } = null!;

    public string TenCongViec { get; set; } = null!;

    public string? MoTa { get; set; }

    public string MaDuAn { get; set; } = null!;

    public string MaGiaiDoanDuAn { get; set; } = null!;

    public string? MaNguoiThucHien { get; set; }

    public string? MaTo { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public string MaTrangThaiCv { get; set; } = null!;

    public int? MucDoUuTien { get; set; }

    public decimal? TienDo { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();

    public virtual DuAn MaDuAnNavigation { get; set; } = null!;

    public virtual GiaiDoanDuAn MaGiaiDoanDuAnNavigation { get; set; } = null!;

    public virtual NguoiDung? MaNguoiThucHienNavigation { get; set; }

    public virtual ToChuyenMon? MaToNavigation { get; set; }

    public virtual TrangThaiCongViec MaTrangThaiCvNavigation { get; set; } = null!;

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();
}
