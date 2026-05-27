using System;
using System.Collections.Generic;

namespace QLNhaTroV7.Domain;

public partial class PhongTro
{
    public int Id { get; set; }

    public int NhaTroId { get; set; }

    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public int? Tang { get; set; }

    public decimal DienTich { get; set; }

    public decimal GiaThueThang { get; set; }

    public decimal TienCoc { get; set; }

    public int? SoNguoiToiDa { get; set; }

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<BaiDang> BaiDangs { get; set; } = new List<BaiDang>();

    public virtual ICollection<DatThue> DatThues { get; set; } = new List<DatThue>();

    public virtual HinhAnh? HinhAnh { get; set; }

    public virtual NhaTro NhaTro { get; set; } = null!;

    public virtual ICollection<TienNghi> TienNghis { get; set; } = new List<TienNghi>();
}
