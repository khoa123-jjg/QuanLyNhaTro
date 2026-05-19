using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class NhaTro
{
    public int Id { get; set; }

    public int ChuNhaTroId { get; set; }

    public int DiaChiId { get; set; }

    public string TenNhaTro { get; set; } = null!;

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<BaiDang> BaiDangs { get; set; } = new List<BaiDang>();

    public virtual ChuNhaTro ChuNhaTro { get; set; } = null!;

    public virtual DiaChi DiaChi { get; set; } = null!;

    public virtual ICollection<LoaiPhong> LoaiPhongs { get; set; } = new List<LoaiPhong>();
}
