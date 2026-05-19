using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class PhongTro
{
    public int Id { get; set; }

    public int LoaiPhongId { get; set; }

    public string MaPhong { get; set; } = null!;

    public int? Tang { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<DatThue> DatThues { get; set; } = new List<DatThue>();

    public virtual LoaiPhong LoaiPhong { get; set; } = null!;
}
