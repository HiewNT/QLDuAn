using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class NguoiDung
{
    public string MaNguoiDung { get; set; } = null!;

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string MaVaiTro { get; set; } = null!;

    public string? MaTo { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<BaoCao> BaoCaos { get; set; } = new List<BaoCao>();

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<CongViec> CongViecs { get; set; } = new List<CongViec>();

    public virtual ICollection<DuAn> DuAns { get; set; } = new List<DuAn>();

    public virtual ToChuyenMon? MaToNavigation { get; set; }

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<TagNguoiDung> TagNguoiDungs { get; set; } = new List<TagNguoiDung>();

    public virtual ICollection<TaiLieu> TaiLieus { get; set; } = new List<TaiLieu>();

    public virtual ICollection<ThongBao> ThongBaoMaNguoiGuiNavigations { get; set; } = new List<ThongBao>();

    public virtual ICollection<ThongBao> ThongBaoMaNguoiNhanNavigations { get; set; } = new List<ThongBao>();
}
