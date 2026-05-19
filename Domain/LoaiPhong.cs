using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class LoaiPhong
{
    public int Id { get; set; }

    public int NhaTroId { get; set; }

    public string TenLoaiPhong { get; set; } = null!;

    public decimal DienTich { get; set; }

    public decimal GiaThueThang { get; set; }

    public decimal TienCoc { get; set; }

    public int? SoNguoiToiDa { get; set; }

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<DatThue> DatThues { get; set; } = new List<DatThue>();

    public virtual HinhAnh? HinhAnh { get; set; }

    public virtual NhaTro NhaTro { get; set; } = null!;

    public virtual ICollection<PhongTro> PhongTros { get; set; } = new List<PhongTro>();

    public virtual ICollection<BaiDang> BaiDangs { get; set; } = new List<BaiDang>();

    public virtual ICollection<TienNghi> TienNghis { get; set; } = new List<TienNghi>();
}
