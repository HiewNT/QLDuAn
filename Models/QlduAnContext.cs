using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLDuAn.Models;

public partial class QlduAnContext : DbContext
{
    private const string ConnectionString = "Server=HIEW\\HIEW;Database=QLDuAnDB;Trusted_Connection=True;TrustServerCertificate=True;";

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

    public virtual DbSet<LichLamViec> LichLamViecs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhanQuyenTaiLieu> PhanQuyenTaiLieus { get; set; }

    public virtual DbSet<TaiLieu> TaiLieus { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<ToChuyenMon> ToChuyenMons { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BINH_LUA__87CB66A071AA4463");

            entity.ToTable("BINH_LUAN");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaCongViec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BINH_LUAN__MaCon__5070F446");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BINH_LUAN__MaNgu__4F7CD00D");
        });

        modelBuilder.Entity<CongViec>(entity =>
        {
            entity.HasKey(e => e.MaCongViec).HasName("PK__CONG_VIE__41B7DD1855852D5A");

            entity.ToTable("CONG_VIEC");

            entity.Property(e => e.GiaiDoan).HasMaxLength(50);
            entity.Property(e => e.TenCongViec).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaDuAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CONG_VIEC__MaDuA__440B1D61");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK__CONG_VIEC__MaNgu__44FF419A");

            entity.HasOne(d => d.MaToNavigation).WithMany(p => p.CongViecs)
                .HasForeignKey(d => d.MaTo)
                .HasConstraintName("FK__CONG_VIEC__MaTo__45F365D3");
        });

        modelBuilder.Entity<DuAn>(entity =>
        {
            entity.HasKey(e => e.MaDuAn).HasName("PK__DU_AN__EFD751E4CDB8BCA8");

            entity.ToTable("DU_AN");

            entity.Property(e => e.TenDuAn).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.NguoiPhuTrachNavigation).WithMany(p => p.DuAns)
                .HasForeignKey(d => d.NguoiPhuTrach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DU_AN__NguoiPhuT__412EB0B6");
        });

        modelBuilder.Entity<LichLamViec>(entity =>
        {
            entity.HasKey(e => e.MaLich).HasName("PK__LICH_LAM__728A9AE98EC9E650");

            entity.ToTable("LICH_LAM_VIEC");

            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.LichLamViecs)
                .HasForeignKey(d => d.MaDuAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LICH_LAM___MaDuA__59FA5E80");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NGUOI_DU__C539D762289AA561");

            entity.ToTable("NGUOI_DUNG");

            entity.HasIndex(e => e.Email, "UQ__NGUOI_DU__A9D1053453D38994").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaToNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaTo)
                .HasConstraintName("FK__NGUOI_DUNG__MaTo__3E52440B");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NGUOI_DUN__MaVai__3D5E1FD2");
        });

        modelBuilder.Entity<PhanQuyenTaiLieu>(entity =>
        {
            entity.HasKey(e => e.MaPhanQuyen).HasName("PK__PHAN_QUY__529AB12B4B4C79EC");

            entity.ToTable("PHAN_QUYEN_TAI_LIEU");

            entity.Property(e => e.QuyenTruyCap).HasMaxLength(50);

            entity.HasOne(d => d.MaTaiLieuNavigation).WithMany(p => p.PhanQuyenTaiLieus)
                .HasForeignKey(d => d.MaTaiLieu)
                .HasConstraintName("FK__PHAN_QUYE__MaTai__5CD6CB2B");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.PhanQuyenTaiLieus)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PHAN_QUYE__MaVai__5DCAEF64");
        });

        modelBuilder.Entity<TaiLieu>(entity =>
        {
            entity.HasKey(e => e.MaTaiLieu).HasName("PK__TAI_LIEU__FD18A657D52C1D12");

            entity.ToTable("TAI_LIEU");

            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.NgayUpload)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenTaiLieu).HasMaxLength(200);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK__TAI_LIEU__MaCong__4AB81AF0");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.MaDuAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_LIEU__MaDuAn__49C3F6B7");

            entity.HasOne(d => d.NguoiUploadNavigation).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.NguoiUpload)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TAI_LIEU__NguoiU__4BAC3F29");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__THONG_BA__04DEB54E566B0F86");

            entity.ToTable("THONG_BAO");

            entity.Property(e => e.DaDoc).HasDefaultValue(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TieuDe).HasMaxLength(200);

            entity.HasOne(d => d.MaCongViecNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK__THONG_BAO__MaCon__571DF1D5");

            entity.HasOne(d => d.MaDuAnNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK__THONG_BAO__MaDuA__5629CD9C");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__THONG_BAO__MaNgu__5535A963");
        });

        modelBuilder.Entity<ToChuyenMon>(entity =>
        {
            entity.HasKey(e => e.MaTo).HasName("PK__TO_CHUYE__2725005CB00EDA4A");

            entity.ToTable("TO_CHUYEN_MON");

            entity.Property(e => e.TenTo).HasMaxLength(100);
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VAI_TRO__C24C41CF1A24F70C");

            entity.ToTable("VAI_TRO");

            entity.Property(e => e.MaVaiTro).ValueGeneratedNever();
            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}