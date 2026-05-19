using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Domain;

namespace QuanLyNhaTro.Data;

public partial class PhongTroDaNangContext : DbContext
{
    public PhongTroDaNangContext()
    {
    }

    public PhongTroDaNangContext(DbContextOptions<PhongTroDaNangContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaiDang> BaiDangs { get; set; }

    public virtual DbSet<ChuNhaTro> ChuNhaTros { get; set; }

    public virtual DbSet<DatThue> DatThues { get; set; }

    public virtual DbSet<DiaChi> DiaChis { get; set; }

    public virtual DbSet<DonViHanhChinh> DonViHanhChinhs { get; set; }

    public virtual DbSet<DuongPho> DuongPhos { get; set; }

    public virtual DbSet<HinhAnh> HinhAnhs { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDungVaiTro> NguoiDungVaiTros { get; set; }

    public virtual DbSet<NguoiThue> NguoiThues { get; set; }

    public virtual DbSet<NhaTro> NhaTros { get; set; }

    public virtual DbSet<PhongTro> PhongTros { get; set; }

    public virtual DbSet<TienNghi> TienNghis { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiDang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BAI_DANG__3214EC070AE1945E");

            entity.ToTable("BAI_DANG");

            entity.HasIndex(e => e.NguoiDuyetId, "IX_BAI_DANG_NguoiDuyetId");

            entity.HasIndex(e => e.NhaTroId, "IX_BAI_DANG_NhaTroId");

            entity.HasIndex(e => e.TrangThaiDuyet, "IX_BAI_DANG_TrangThaiDuyet");

            entity.Property(e => e.LyDoTuChoi).HasMaxLength(500);
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayDuyet).HasPrecision(0);
            entity.Property(e => e.NgayGuiDuyet).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_BAI_DANG_NgayTao");
            entity.Property(e => e.TieuDe).HasMaxLength(250);
            entity.Property(e => e.TrangThaiDuyet)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("NHAP", "DF_BAI_DANG_TrangThaiDuyet");

            entity.HasOne(d => d.NguoiDuyet).WithMany(p => p.BaiDangs)
                .HasForeignKey(d => d.NguoiDuyetId)
                .HasConstraintName("FK_BAI_DANG_NGUOI_DUYET");

            entity.HasOne(d => d.NhaTro).WithMany(p => p.BaiDangs)
                .HasForeignKey(d => d.NhaTroId)
                .HasConstraintName("FK_BAI_DANG_NHA_TRO");

            entity.HasMany(d => d.LoaiPhongs).WithMany(p => p.BaiDangs)
                .UsingEntity<Dictionary<string, object>>(
                    "BaiDangLoaiPhong",
                    r => r.HasOne<LoaiPhong>().WithMany()
                        .HasForeignKey("LoaiPhongId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BDLP_LOAI_PHONG"),
                    l => l.HasOne<BaiDang>().WithMany()
                        .HasForeignKey("BaiDangId")
                        .HasConstraintName("FK_BDLP_BAI_DANG"),
                    j =>
                    {
                        j.HasKey("BaiDangId", "LoaiPhongId");
                        j.ToTable("BAI_DANG_LOAI_PHONG", tb => tb.HasTrigger("TR_BDLP_Check_NhaTro"));
                    });
        });

        modelBuilder.Entity<ChuNhaTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CHU_NHA___3214EC0756B4923F");

            entity.ToTable("CHU_NHA_TRO");

            entity.HasIndex(e => e.NguoiDungId, "IX_CHU_NHA_TRO_NguoiDungId");

            entity.HasIndex(e => e.NguoiDungId, "UQ_CHU_NHA_TRO_NguoiDungId").IsUnique();

            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_CHU_NHA_TRO_NgayTao");
            entity.Property(e => e.TrangThaiHoSo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HOAT_DONG", "DF_CHU_NHA_TRO_TrangThaiHoSo");

            entity.HasOne(d => d.NguoiDung).WithOne(p => p.ChuNhaTro)
                .HasForeignKey<ChuNhaTro>(d => d.NguoiDungId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHU_NHA_TRO_NGUOI_DUNG");
        });

        modelBuilder.Entity<DatThue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DAT_THUE__3214EC07F9FE1607");

            entity.ToTable("DAT_THUE", tb => tb.HasTrigger("TR_DAT_THUE_Check_Phong"));

            entity.HasIndex(e => e.LoaiPhongId, "IX_DAT_THUE_LoaiPhongId");

            entity.HasIndex(e => e.NguoiThueId, "IX_DAT_THUE_NguoiThueId");

            entity.HasIndex(e => e.PhongTroId, "IX_DAT_THUE_PhongTroId");

            entity.HasIndex(e => e.TrangThai, "IX_DAT_THUE_TrangThai");

            entity.Property(e => e.GhiChuChuTro).HasMaxLength(500);
            entity.Property(e => e.HoTenLienHe).HasMaxLength(150);
            entity.Property(e => e.LoiNhan).HasMaxLength(1000);
            entity.Property(e => e.LyDoTuChoi).HasMaxLength(500);
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayMuonXemPhong).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_DAT_THUE_NgayTao");
            entity.Property(e => e.NgayXuLy).HasPrecision(0);
            entity.Property(e => e.SoDienThoaiLienHe).HasMaxLength(20);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("MOI", "DF_DAT_THUE_TrangThai");

            entity.HasOne(d => d.LoaiPhong).WithMany(p => p.DatThues)
                .HasForeignKey(d => d.LoaiPhongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DAT_THUE_LOAI_PHONG");

            entity.HasOne(d => d.NguoiThue).WithMany(p => p.DatThues)
                .HasForeignKey(d => d.NguoiThueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DAT_THUE_NGUOI_THUE");

            entity.HasOne(d => d.PhongTro).WithMany(p => p.DatThues)
                .HasForeignKey(d => d.PhongTroId)
                .HasConstraintName("FK_DAT_THUE_PHONG_TRO");
        });

        modelBuilder.Entity<DiaChi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DIA_CHI__3214EC077A708683");

            entity.ToTable("DIA_CHI", tb => tb.HasTrigger("TR_DIA_CHI_Check_DVHC"));

            entity.HasIndex(e => new { e.DonViHanhChinhId, e.DuongPhoId }, "IX_DIA_CHI_DVHC_DUONG");

            entity.Property(e => e.DiaChiChiTiet).HasMaxLength(500);
            entity.Property(e => e.KinhDo).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.SoNha).HasMaxLength(50);
            entity.Property(e => e.ViDo).HasColumnType("decimal(10, 7)");

            entity.HasOne(d => d.DonViHanhChinh).WithMany(p => p.DiaChis)
                .HasForeignKey(d => d.DonViHanhChinhId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DIA_CHI_DVHC");

            entity.HasOne(d => d.DuongPho).WithMany(p => p.DiaChis)
                .HasForeignKey(d => d.DuongPhoId)
                .HasConstraintName("FK_DIA_CHI_DUONG_PHO");
        });

        modelBuilder.Entity<DonViHanhChinh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DON_VI_H__3214EC0750ABBE06");

            entity.ToTable("DON_VI_HANH_CHINH");

            entity.HasIndex(e => new { e.TenDonVi, e.LoaiDonVi, e.ThanhPho }, "UQ_DVHC_Ten_Loai").IsUnique();

            entity.Property(e => e.LoaiDonVi)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TenDonVi).HasMaxLength(150);
            entity.Property(e => e.ThanhPho)
                .HasMaxLength(100)
                .HasDefaultValue("Đà Nẵng", "DF_DVHC_ThanhPho");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HIEN_THI", "DF_DVHC_TrangThai");
        });

        modelBuilder.Entity<DuongPho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DUONG_PH__3214EC07D61904C6");

            entity.ToTable("DUONG_PHO");

            entity.HasIndex(e => e.DonViHanhChinhId, "IX_DUONG_PHO_DVHC");

            entity.HasIndex(e => new { e.DonViHanhChinhId, e.TenDuong }, "UQ_DUONG_PHO_DVHC_TenDuong").IsUnique();

            entity.Property(e => e.TenDuong).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HIEN_THI", "DF_DUONG_PHO_TrangThai");

            entity.HasOne(d => d.DonViHanhChinh).WithMany(p => p.DuongPhos)
                .HasForeignKey(d => d.DonViHanhChinhId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DUONG_PHO_DVHC");
        });

        modelBuilder.Entity<HinhAnh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HINH_ANH__3214EC07A21B614E");

            entity.ToTable("HINH_ANH");

            entity.HasIndex(e => e.LoaiPhongId, "IX_HINH_ANH_LoaiPhongId");

            entity.HasIndex(e => e.LoaiPhongId, "UX_HINH_ANH_OneCover")
                .IsUnique()
                .HasFilter("([LaAnhDaiDien]=(1))");

            entity.Property(e => e.DuongDanAnh).HasMaxLength(500);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_HINH_ANH_NgayTao");
            entity.Property(e => e.ThuTuHienThi).HasDefaultValue(1, "DF_HINH_ANH_ThuTuHienThi");

            entity.HasOne(d => d.LoaiPhong).WithOne(p => p.HinhAnh)
                .HasForeignKey<HinhAnh>(d => d.LoaiPhongId)
                .HasConstraintName("FK_HINH_ANH_LOAI_PHONG");
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LOAI_PHO__3214EC070E7C63A2");

            entity.ToTable("LOAI_PHONG");

            entity.HasIndex(e => e.NhaTroId, "IX_LOAI_PHONG_NhaTroId");

            entity.HasIndex(e => new { e.TrangThai, e.GiaThueThang, e.DienTich }, "IX_LOAI_PHONG_Search");

            entity.Property(e => e.DienTich).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.GiaThueThang).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_LOAI_PHONG_NgayTao");
            entity.Property(e => e.TenLoaiPhong).HasMaxLength(200);
            entity.Property(e => e.TienCoc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HOAT_DONG", "DF_LOAI_PHONG_TrangThai");

            entity.HasOne(d => d.NhaTro).WithMany(p => p.LoaiPhongs)
                .HasForeignKey(d => d.NhaTroId)
                .HasConstraintName("FK_LOAI_PHONG_NHA_TRO");

            entity.HasMany(d => d.TienNghis).WithMany(p => p.LoaiPhongs)
                .UsingEntity<Dictionary<string, object>>(
                    "LoaiPhongTienNghi",
                    r => r.HasOne<TienNghi>().WithMany()
                        .HasForeignKey("TienNghiId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_LPTN_TIEN_NGHI"),
                    l => l.HasOne<LoaiPhong>().WithMany()
                        .HasForeignKey("LoaiPhongId")
                        .HasConstraintName("FK_LPTN_LOAI_PHONG"),
                    j =>
                    {
                        j.HasKey("LoaiPhongId", "TienNghiId");
                        j.ToTable("LOAI_PHONG_TIEN_NGHI");
                    });
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NGUOI_DU__3214EC07C4305C84");

            entity.ToTable("NGUOI_DUNG");

            entity.HasIndex(e => e.TrangThai, "IX_NGUOI_DUNG_TrangThai");

            entity.HasIndex(e => e.Email, "UQ_NGUOI_DUNG_Email").IsUnique();

            entity.HasIndex(e => e.SoDienThoai, "UQ_NGUOI_DUNG_SoDienThoai").IsUnique();

            entity.Property(e => e.AnhDaiDien).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.HoTen).HasMaxLength(150);
            entity.Property(e => e.MatKhauHash).HasMaxLength(500);
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_NGUOI_DUNG_NgayTao");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HOAT_DONG", "DF_NGUOI_DUNG_TrangThai");
        });

        modelBuilder.Entity<NguoiDungVaiTro>(entity =>
        {
            entity.HasKey(e => new { e.NguoiDungId, e.VaiTroId });

            entity.ToTable("NGUOI_DUNG_VAI_TRO");

            entity.Property(e => e.NgayGan)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_NDVT_NgayGan");

            entity.HasOne(d => d.NguoiDung).WithMany(p => p.NguoiDungVaiTros)
                .HasForeignKey(d => d.NguoiDungId)
                .HasConstraintName("FK_NDVT_NGUOI_DUNG");

            entity.HasOne(d => d.VaiTro).WithMany(p => p.NguoiDungVaiTros)
                .HasForeignKey(d => d.VaiTroId)
                .HasConstraintName("FK_NDVT_VAI_TRO");
        });

        modelBuilder.Entity<NguoiThue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NGUOI_TH__3214EC0758546596");

            entity.ToTable("NGUOI_THUE");

            entity.HasIndex(e => e.NguoiDungId, "IX_NGUOI_THUE_NguoiDungId");

            entity.HasIndex(e => e.NguoiDungId, "UQ_NGUOI_THUE_NguoiDungId").IsUnique();

            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_NGUOI_THUE_NgayTao");
            entity.Property(e => e.NgheNghiep).HasMaxLength(150);
            entity.Property(e => e.NhuCauThue).HasMaxLength(255);

            entity.HasOne(d => d.NguoiDung).WithOne(p => p.NguoiThue)
                .HasForeignKey<NguoiThue>(d => d.NguoiDungId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGUOI_THUE_NGUOI_DUNG");
        });

        modelBuilder.Entity<NhaTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NHA_TRO__3214EC0762894AD2");

            entity.ToTable("NHA_TRO");

            entity.HasIndex(e => e.ChuNhaTroId, "IX_NHA_TRO_ChuNhaTroId");

            entity.HasIndex(e => e.TrangThai, "IX_NHA_TRO_TrangThai");

            entity.HasIndex(e => e.DiaChiId, "UQ_NHA_TRO_DiaChiId").IsUnique();

            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_NHA_TRO_NgayTao");
            entity.Property(e => e.TenNhaTro).HasMaxLength(200);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HOAT_DONG", "DF_NHA_TRO_TrangThai");

            entity.HasOne(d => d.ChuNhaTro).WithMany(p => p.NhaTros)
                .HasForeignKey(d => d.ChuNhaTroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NHA_TRO_CHU_NHA_TRO");

            entity.HasOne(d => d.DiaChi).WithOne(p => p.NhaTro)
                .HasForeignKey<NhaTro>(d => d.DiaChiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NHA_TRO_DIA_CHI");
        });

        modelBuilder.Entity<PhongTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PHONG_TR__3214EC07E49411A6");

            entity.ToTable("PHONG_TRO");

            entity.HasIndex(e => e.LoaiPhongId, "IX_PHONG_TRO_LoaiPhongId");

            entity.HasIndex(e => e.TrangThai, "IX_PHONG_TRO_TrangThai");

            entity.HasIndex(e => new { e.LoaiPhongId, e.MaPhong }, "UQ_PHONG_TRO_LoaiPhong_MaPhong").IsUnique();

            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.MaPhong).HasMaxLength(50);
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_PHONG_TRO_NgayTao");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("TRONG", "DF_PHONG_TRO_TrangThai");

            entity.HasOne(d => d.LoaiPhong).WithMany(p => p.PhongTros)
                .HasForeignKey(d => d.LoaiPhongId)
                .HasConstraintName("FK_PHONG_TRO_LOAI_PHONG");
        });

        modelBuilder.Entity<TienNghi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TIEN_NGH__3214EC07AC5EC846");

            entity.ToTable("TIEN_NGHI");

            entity.HasIndex(e => e.TenTienNghi, "UQ_TIEN_NGHI_TenTienNghi").IsUnique();

            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.TenTienNghi).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HIEN_THI", "DF_TIEN_NGHI_TrangThai");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VAI_TRO__3214EC07C89517D1");

            entity.ToTable("VAI_TRO");

            entity.HasIndex(e => e.TenVaiTro, "UQ_VAI_TRO_TenVaiTro").IsUnique();

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenVaiTro)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
