using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLNhaTroV7.Domain;

namespace QLNhaTroV7.Data;

public partial class PhongTroDaNangContext : DbContext
{
    public PhongTroDaNangContext(DbContextOptions<PhongTroDaNangContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaiDang> BaiDangs { get; set; }

    public virtual DbSet<ChuNhaTro> ChuNhaTros { get; set; }

    public virtual DbSet<DatThue> DatThues { get; set; }

    public virtual DbSet<DonViHanhChinh> DonViHanhChinhs { get; set; }

    public virtual DbSet<DuongPho> DuongPhos { get; set; }

    public virtual DbSet<HinhAnh> HinhAnhs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NguoiDungVaiTro> NguoiDungVaiTros { get; set; }

    public virtual DbSet<NguoiThue> NguoiThues { get; set; }

    public virtual DbSet<NhaTro> NhaTros { get; set; }

    public virtual DbSet<PhongTro> PhongTros { get; set; }

    public virtual DbSet<TienNghi> TienNghis { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiDang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BAI_DANG__3214EC07D60CD980");

            entity.ToTable("BAI_DANG");

            entity.HasIndex(e => e.PhongTroId, "IX_BAI_DANG_PhongTroId");

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

            entity.HasOne(d => d.PhongTro).WithMany(p => p.BaiDangs)
                .HasForeignKey(d => d.PhongTroId)
                .HasConstraintName("FK_BAI_DANG_PHONG_TRO");
        });

        modelBuilder.Entity<ChuNhaTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CHU_NHA___3214EC07C213CFA0");

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
            entity.HasKey(e => e.Id).HasName("PK__DAT_THUE__3214EC07B0E5DD5E");

            entity.ToTable("DAT_THUE");

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

            entity.HasOne(d => d.NguoiThue).WithMany(p => p.DatThues)
                .HasForeignKey(d => d.NguoiThueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DAT_THUE_NGUOI_THUE");

            entity.HasOne(d => d.PhongTro).WithMany(p => p.DatThues)
                .HasForeignKey(d => d.PhongTroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DAT_THUE_PHONG_TRO");
        });

        modelBuilder.Entity<DonViHanhChinh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DON_VI_H__3214EC07F1478B2B");

            entity.ToTable("DON_VI_HANH_CHINH");

            entity.HasIndex(e => new { e.TenDonVi, e.LoaiDonVi, e.TinhThanh }, "UQ_DVHC_Ten_Loai_TinhThanh").IsUnique();

            entity.Property(e => e.LoaiDonVi)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TenDonVi).HasMaxLength(150);
            entity.Property(e => e.TinhThanh)
                .HasMaxLength(100)
                .HasDefaultValue("Đà Nẵng", "DF_DVHC_TinhThanh");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HIEN_THI", "DF_DVHC_TrangThai");
        });

        modelBuilder.Entity<DuongPho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DUONG_PH__3214EC0772B1AF2B");

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
            entity.HasKey(e => e.Id).HasName("PK__HINH_ANH__3214EC07C8F0DF5A");

            entity.ToTable("HINH_ANH");

            entity.HasIndex(e => e.PhongTroId, "IX_HINH_ANH_PhongTroId");

            entity.HasIndex(e => e.PhongTroId, "UX_HINH_ANH_OneCover")
                .IsUnique()
                .HasFilter("([LaAnhDaiDien]=(1))");

            entity.Property(e => e.DuongDanAnh).HasMaxLength(500);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_HINH_ANH_NgayTao");
            entity.Property(e => e.ThuTuHienThi).HasDefaultValue(1, "DF_HINH_ANH_ThuTuHienThi");

            entity.HasOne(d => d.PhongTro).WithOne(p => p.HinhAnh)
                .HasForeignKey<HinhAnh>(d => d.PhongTroId)
                .HasConstraintName("FK_HINH_ANH_PHONG_TRO");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NGUOI_DU__3214EC077C5C6D02");

            entity.ToTable("NGUOI_DUNG");

            entity.HasIndex(e => e.TrangThai, "IX_NGUOI_DUNG_TrangThai");

            entity.HasIndex(e => e.Email, "UQ_NGUOI_DUNG_Email").IsUnique();

            entity.HasIndex(e => e.SoDienThoai, "UX_NGUOI_DUNG_SoDienThoai_NotNull")
                .IsUnique()
                .HasFilter("([SoDienThoai] IS NOT NULL)");

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
            entity.HasKey(e => e.Id).HasName("PK__NGUOI_TH__3214EC070B25B4ED");

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
            entity.HasKey(e => e.Id).HasName("PK__NHA_TRO__3214EC0714EF2651");

            entity.ToTable("NHA_TRO");

            entity.HasIndex(e => e.ChuNhaTroId, "IX_NHA_TRO_ChuNhaTroId");

            entity.HasIndex(e => e.DuongPhoId, "IX_NHA_TRO_DuongPhoId");

            entity.HasIndex(e => e.TrangThai, "IX_NHA_TRO_TrangThai");

            entity.Property(e => e.KinhDo).HasColumnType("decimal(10, 7)");
            entity.Property(e => e.MoTaDiaChi).HasMaxLength(500);
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_NHA_TRO_NgayTao");
            entity.Property(e => e.SoNha).HasMaxLength(50);
            entity.Property(e => e.TenNhaTro).HasMaxLength(200);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("HOAT_DONG", "DF_NHA_TRO_TrangThai");
            entity.Property(e => e.ViDo).HasColumnType("decimal(10, 7)");

            entity.HasOne(d => d.ChuNhaTro).WithMany(p => p.NhaTros)
                .HasForeignKey(d => d.ChuNhaTroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NHA_TRO_CHU_NHA_TRO");

            entity.HasOne(d => d.DuongPho).WithMany(p => p.NhaTros)
                .HasForeignKey(d => d.DuongPhoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NHA_TRO_DUONG_PHO");
        });

        modelBuilder.Entity<PhongTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PHONG_TR__3214EC071A1067BC");

            entity.ToTable("PHONG_TRO");

            entity.HasIndex(e => e.NhaTroId, "IX_PHONG_TRO_NhaTroId");

            entity.HasIndex(e => new { e.TrangThai, e.GiaThueThang, e.DienTich }, "IX_PHONG_TRO_Search");

            entity.HasIndex(e => e.TrangThai, "IX_PHONG_TRO_TrangThai");

            entity.HasIndex(e => new { e.NhaTroId, e.MaPhong }, "UQ_PHONG_TRO_NhaTro_MaPhong").IsUnique();

            entity.Property(e => e.DienTich).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.GiaThueThang).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaPhong).HasMaxLength(50);
            entity.Property(e => e.NgayCapNhat).HasPrecision(0);
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_PHONG_TRO_NgayTao");
            entity.Property(e => e.TenPhong).HasMaxLength(200);
            entity.Property(e => e.TienCoc).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("TRONG", "DF_PHONG_TRO_TrangThai");

            entity.HasOne(d => d.NhaTro).WithMany(p => p.PhongTros)
                .HasForeignKey(d => d.NhaTroId)
                .HasConstraintName("FK_PHONG_TRO_NHA_TRO");

            entity.HasMany(d => d.TienNghis).WithMany(p => p.PhongTros)
                .UsingEntity<Dictionary<string, object>>(
                    "PhongTroTienNghi",
                    r => r.HasOne<TienNghi>().WithMany()
                        .HasForeignKey("TienNghiId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PTTN_TIEN_NGHI"),
                    l => l.HasOne<PhongTro>().WithMany()
                        .HasForeignKey("PhongTroId")
                        .HasConstraintName("FK_PTTN_PHONG_TRO"),
                    j =>
                    {
                        j.HasKey("PhongTroId", "TienNghiId");
                        j.ToTable("PHONG_TRO_TIEN_NGHI");
                    });
        });

        modelBuilder.Entity<TienNghi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TIEN_NGH__3214EC07D8E3E17C");

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
            entity.HasKey(e => e.Id).HasName("PK__VAI_TRO__3214EC07EA087A11");

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
