using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class NguoiDung
{
    public int Id { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string MatKhauHash { get; set; } = null!;

    public string? AnhDaiDien { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<BaiDang> BaiDangs { get; set; } = new List<BaiDang>();

    public virtual ChuNhaTro? ChuNhaTro { get; set; }

    public virtual ICollection<NguoiDungVaiTro> NguoiDungVaiTros { get; set; } = new List<NguoiDungVaiTro>();

    public virtual NguoiThue? NguoiThue { get; set; }
}
