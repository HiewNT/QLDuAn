using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int MaVaiTro { get; set; }

    public int? MaTo { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual ICollection<DuAn> DuAns { get; set; } = new List<DuAn>();

    public virtual ToChuyenMon? MaToNavigation { get; set; }

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
