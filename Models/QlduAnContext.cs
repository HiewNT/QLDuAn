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

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<CongViec> CongViecs { get; set; }

    public virtual DbSet<DuAn> DuAns { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; }

    public virtual DbSet<TaiLieu> TaiLieus { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<ToChuyenMon> ToChuyenMons { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HIEW;Database=QLDuAnDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BINH_LUA__87CB66A06B570442");

            entity.ToTable("BINH_LUAN");

            entity.Property(e => e.NgayTao).HasColumnType("datetime");

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_BINH_LUAN_CONG_VIEC");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BINH_LUAN_NGUOI_DUNG");
        });

        modelBuilder.Entity<CongViec>(entity =>
        {
            entity.HasKey(e => e.MaCongViec).HasName("PK__CONG_VIE__41B7DD189E68B836");

            entity.ToTable("CONG_VIEC");

            entity.Property(e => e.GiaiDoan).HasMaxLength(50);
            entity.Property(e => e.TenCongViec).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_CONG_VIEC_DU_AN");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_CONG_VIEC_NGUOI_DUNG");

            entity.HasOne(d => d.MaToNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaTo)
                .HasConstraintName("FK_CONG_VIEC_TO_CHUYEN_MON");
        });

        modelBuilder.Entity<DuAn>(entity =>
        {
            entity.HasKey(e => e.MaDuAn).HasName("PK__DU_AN__EFD751E4F384CB82");

            entity.ToTable("DU_AN");

            entity.Property(e => e.TenDuAn).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.NguoiPhuTrachNavigation).WithMany(p => p.DuAns)
                .HasForeignKey(d => d.NguoiPhuTrach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DU_AN_NGUOI_DUNG");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NGUOI_DU__C539D762444FB9A9");

            entity.ToTable("NGUOI_DUNG");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);

            entity.HasOne(d => d.MaToNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaTo)
                .HasConstraintName("FK_NGUOI_DUNG_TO_CHUYEN_MON");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGUOI_DUNG_VAI_TRO");
        });

        modelBuilder.Entity<PhanQuyenTaiLieu>(entity =>
        {
            entity.HasKey(e => e.MaPhanQuyen).HasName("PK__PHAN_QUY__529AB12B8B34DB7E");

            entity.ToTable("PHAN_QUYEN_TAI_LIEU");

            entity.Property(e => e.QuyenTruyCap).HasMaxLength(50);

            entity.HasOne(d => d.MaTaiLieuNavigation).WithMany(p => p.PhanQuyenTaiLieus)
                .HasForeignKey(d => d.MaTaiLieu)
                .HasConstraintName("FK_PHAN_QUYEN_TAI_LIEU_TAI_LIEU");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.PhanQuyenTaiLieus)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHAN_QUYEN_TAI_LIEU_VAI_TRO");
        });

        modelBuilder.Entity<TaiLieu>(entity =>
        {
            entity.HasKey(e => e.MaTaiLieu).HasName("PK__TAI_LIEU__FD18A657D2C7481D");

            entity.ToTable("TAI_LIEU");

            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.NgayUpload).HasColumnType("datetime");
            entity.Property(e => e.TenTaiLieu).HasMaxLength(200);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_TAI_LIEU_CONG_VIEC");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_TAI_LIEU_DU_AN");

            entity.HasOne(d => d.NguoiUploadNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.NguoiUpload)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TAI_LIEU_NGUOI_DUNG");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__THONG_BA__04DEB54E21F2E2C6");

            entity.ToTable("THONG_BAO");

            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_THONG_BAO_CONG_VIEC");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaDuAn)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_THONG_BAO_DU_AN");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THONG_BAO_NGUOI_DUNG");
        });

        modelBuilder.Entity<ToChuyenMon>(entity =>
        {
            entity.HasKey(e => e.MaTo).HasName("PK__TO_CHUYE__2725005CBA31EEDD");

            entity.ToTable("TO_CHUYEN_MON");

            entity.Property(e => e.TenTo).HasMaxLength(100);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VAI_TRO__C24C41CF5ECBCF1E");

            entity.ToTable("VAI_TRO");

            entity.Property(e => e.MaVaiTro).ValueGeneratedNever();
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
