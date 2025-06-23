using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class CongViec
{
    public int MaCongViec { get; set; }

    public string TenCongViec { get; set; } = null!;

    public string? GiaiDoan { get; set; }

    public string? TrangThai { get; set; }

    public DateOnly? Deadline { get; set; }

    public int MaDuAn { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaTo { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual DuAn MaDuAnNavigation { get; set; } = null!;

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }

    public virtual ToChuyenMon? MaToNavigation { get; set; }

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
