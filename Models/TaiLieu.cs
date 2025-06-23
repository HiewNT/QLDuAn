using System;
using System.Collections.Generic;

namespace QLDuAn.Models;

public partial class TaiLieu
{
    public int MaTaiLieu { get; set; }

    public string TenTaiLieu { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int MaDuAn { get; set; }

    public int? MaCongViec { get; set; }

    public int NguoiUpload { get; set; }

    public DateTime? NgayUpload { get; set; }

    public virtual CongViec? MaCongViecNavigation { get; set; }

    public virtual DuAn MaDuAnNavigation { get; set; } = null!;

    public virtual NguoiDung NguoiUploadNavigation { get; set; } = null!;

    public virtual ICollection<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; } = new List<PhanQuyenTaiLieu>();
}
