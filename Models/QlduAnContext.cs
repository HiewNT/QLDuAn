using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLDuAn.Models;

public partial class QlduAnContext : DbContext
{
    public QlduAnContext()
    {
    }

    public QlduAnContext(DbContextOptions<QlduAnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaoCao> BaoCaos { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<CongViec> CongViecs { get; set; }

    public virtual DbSet<DuAn> DuAns { get; set; }

    public virtual DbSet<GiaiDoanDuAn> GiaiDoanDuAns { get; set; }

    public virtual DbSet<LichLamViec> LichLamViecs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; }

    public virtual DbSet<TagNguoiDung> TagNguoiDungs { get; set; }

    public virtual DbSet<TaiLieu> TaiLieus { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<ToChuyenMon> ToChuyenMons { get; set; }

    public virtual DbSet<TrangThaiCongViec> TrangThaiCongViecs { get; set; }

    public virtual DbSet<TrangThaiDuAn> TrangThaiDuAns { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HIEW;Database=QLDuAn;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaoCao>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BAO_CAO__25A9188CA3621FCE");

            entity.ToTable("BAO_CAO");

            entity.Property(e => e.MaBaoCao)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DuongDanFile).HasMaxLength(500);
            entity.Property(e => e.MaNguoiTao)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenBaoCao).HasMaxLength(200);

            entity.HasOne(d => d.MaNguoiTaoNavigation).WithMany(p => p.BaoCaos)
                .HasForeignKey(d => d.MaNguoiTao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BAO_CAO__MaNguoi__76969D2E");
        });

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BINH_LUA__87CB66A0F8120DC5");

            entity.ToTable("BINH_LUAN");

            entity.Property(e => e.MaBinhLuan)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaBinhLuanCha)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaCongViec)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiBinhLuan)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBinhLuan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK__BINH_LUAN__MaCon__693CA210");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK__BINH_LUAN__MaDuA__68487DD7");

            entity.HasOne(d => d.MaNguoiBinhLuanNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNguoiBinhLuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BINH_LUAN__MaNgu__6A30C649");
        });

        modelBuilder.Entity<CongViec>(entity =>
        {
            entity.HasKey(e => e.MaCongViec).HasName("PK__CONG_VIE__41B7DD1859025699");

            entity.ToTable("CONG_VIEC");

            entity.Property(e => e.MaCongViec)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaGiaiDoanDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiThucHien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaTo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaTrangThaiCv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaTrangThaiCV");
            entity.Property(e => e.MucDoUuTien).HasDefaultValue(1);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenCongViec).HasMaxLength(200);
            entity.Property(e => e.TienDo)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaDuAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CONG_VIEC__MaDuA__5165187F");

            entity.HasOne(d => d.MaGiaiDoanDuAnNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaGiaiDoanDuAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CONG_VIEC__MaGia__52593CB8");

            entity.HasOne(d => d.MaNguoiThucHienNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaNguoiThucHien)
                .HasConstraintName("FK__CONG_VIEC__MaNgu__534D60F1");

            entity.HasOne(d => d.MaToNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaTo)
                .HasConstraintName("FK__CONG_VIEC__MaTo__5441852A");

            entity.HasOne(d => d.MaTrangThaiCvNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaTrangThaiCv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CONG_VIEC__MaTra__5535A963");
        });

        modelBuilder.Entity<DuAn>(entity =>
        {
            entity.HasKey(e => e.MaDuAn).HasName("PK__DU_AN__EFD751E48AD6EC54");

            entity.ToTable("DU_AN");

            entity.Property(e => e.MaDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiPhuTrach)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaTrangThai)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDuAn).HasMaxLength(200);

            entity.HasOne(d => d.MaNguoiPhuTrachNavigation).WithMany(p => p.DuAns)
                .HasForeignKey(d => d.MaNguoiPhuTrach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DU_AN__MaNguoiPh__48CFD27E");

            entity.HasOne(d => d.MaTrangThaiNavigation).WithMany(p => p.DuAns)
                .HasForeignKey(d => d.MaTrangThai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DU_AN__MaTrangTh__47DBAE45");
        });

        modelBuilder.Entity<GiaiDoanDuAn>(entity =>
        {
            entity.HasKey(e => e.MaGiaiDoanDuAn).HasName("PK__GIAI_DOA__20D749BFC1D21A6F");

            entity.ToTable("GIAI_DOAN_DU_AN");

            entity.Property(e => e.MaGiaiDoanDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenGiaiDoan).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.GiaiDoanDuAns)
                .HasForeignKey(d => d.MaDuAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GIAI_DOAN__MaDuA__4BAC3F29");
        });

        modelBuilder.Entity<LichLamViec>(entity =>
        {
            entity.HasKey(e => e.MaLich).HasName("PK__LICH_LAM__728A9AE9840DAFDF");

            entity.ToTable("LICH_LAM_VIEC");

            entity.Property(e => e.MaLich)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DiaDiem).HasMaxLength(255);
            entity.Property(e => e.MaCongViec)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.LichLamViecs)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK__LICH_LAM___MaCon__72C60C4A");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NGUOI_DU__C539D762BAF22462");

            entity.ToTable("NGUOI_DUNG");

            entity.HasIndex(e => e.TenDangNhap, "UQ__NGUOI_DU__55F68FC0B9D95FD6").IsUnique();

            entity.Property(e => e.MaNguoiDung)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaTo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaToNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaTo)
                .HasConstraintName("FK__NGUOI_DUNG__MaTo__403A8C7D");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NGUOI_DUN__MaVai__3F466844");
        });

        modelBuilder.Entity<PhanQuyenTaiLieu>(entity =>
        {
            entity.HasKey(e => e.MaPhanQuyenTl).HasName("PK__PHAN_QUY__8CFC73FC83D95BDE");

            entity.ToTable("PHAN_QUYEN_TAI_LIEU");

            entity.Property(e => e.MaPhanQuyenTl)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaPhanQuyenTL");
            entity.Property(e => e.MaTaiLieu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.QuyenTruyCap).HasMaxLength(50);

            entity.HasOne(d => d.MaTaiLieuNavigation).WithMany(p => p.PhanQuyenTaiLieus)
                .HasForeignKey(d => d.MaTaiLieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHAN_QUYE__MaTai__5DCAEF64");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.PhanQuyenTaiLieus)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK__PHAN_QUYE__MaVai__5EBF139D");
        });

        modelBuilder.Entity<TagNguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaTag).HasName("PK__TAG_NGUO__314EC2142BFD457F");

            entity.ToTable("TAG_NGUOI_DUNG");

            entity.Property(e => e.MaTag)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaBinhLuan)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiDuocTag)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayTag)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaBinhLuanNavigation).WithMany(p => p.TagNguoiDungs)
                .HasForeignKey(d => d.MaBinhLuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAG_NGUOI__MaBin__6E01572D");

            entity.HasOne(d => d.MaNguoiDuocTagNavigation).WithMany(p => p.TagNguoiDungs)
                .HasForeignKey(d => d.MaNguoiDuocTag)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAG_NGUOI__MaNgu__6EF57B66");
        });

        modelBuilder.Entity<TaiLieu>(entity =>
        {
            entity.HasKey(e => e.MaTaiLieu).HasName("PK__TAI_LIEU__FD18A6579D03CC18");

            entity.ToTable("TAI_LIEU");

            entity.Property(e => e.MaTaiLieu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DuongDan).HasMaxLength(500);
            entity.Property(e => e.LoaiFile).HasMaxLength(50);
            entity.Property(e => e.MaCongViec)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaDuAn)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiTai)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayTai)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenTaiLieu).HasMaxLength(255);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK__TAI_LIEU__MaCong__59FA5E80");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK__TAI_LIEU__MaDuAn__59063A47");

            entity.HasOne(d => d.MaNguoiTaiNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaNguoiTai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_LIEU__MaNguo__5AEE82B9");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__THONG_BA__04DEB54ECB88AA44");

            entity.ToTable("THONG_BAO");

            entity.Property(e => e.MaThongBao)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DaDoc).HasDefaultValue(false);
            entity.Property(e => e.MaNguoiGui)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaNguoiNhan)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayGui)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaNguoiGuiNavigation).WithMany(p => p.ThongBaoMaNguoiGuiNavigations)
                .HasForeignKey(d => d.MaNguoiGui)
                .HasConstraintName("FK__THONG_BAO__MaNgu__6383C8BA");

            entity.HasOne(d => d.MaNguoiNhanNavigation).WithMany(p => p.ThongBaoMaNguoiNhanNavigations)
                .HasForeignKey(d => d.MaNguoiNhan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__THONG_BAO__MaNgu__6477ECF3");
        });

        modelBuilder.Entity<ToChuyenMon>(entity =>
        {
            entity.HasKey(e => e.MaTo).HasName("PK__TO_CHUYE__2725005CC35B0568");

            entity.ToTable("TO_CHUYEN_MON");

            entity.Property(e => e.MaTo)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.SoLuongNhanVien).HasDefaultValue(0);
            entity.Property(e => e.TenTo).HasMaxLength(100);
        });

        modelBuilder.Entity<TrangThaiCongViec>(entity =>
        {
            entity.HasKey(e => e.MaTrangThaiCv).HasName("PK__TRANG_TH__EB26DE7196CA0C54");

            entity.ToTable("TRANG_THAI_CONG_VIEC");

            entity.Property(e => e.MaTrangThaiCv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaTrangThaiCV");
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<TrangThaiDuAn>(entity =>
        {
            entity.HasKey(e => e.MaTrangThai).HasName("PK__TRANG_TH__AADE4138E3752B1B");

            entity.ToTable("TRANG_THAI_DU_AN");

            entity.Property(e => e.MaTrangThai)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VAI_TRO__C24C41CFAFF054A9");

            entity.ToTable("VAI_TRO");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
