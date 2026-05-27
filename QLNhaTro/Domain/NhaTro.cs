using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class NhaTro
{
    public int Id { get; set; }

    public int ChuNhaTroId { get; set; }

    public int? DuongPhoId { get; set; }

    public string TenNhaTro { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? SoNha { get; set; }

    public string DiaChiChiTiet { get; set; } = null!;

    public decimal? ViDo { get; set; }

    public decimal? KinhDo { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ChuNhaTro ChuNhaTro { get; set; } = null!;

    public virtual DuongPho? DuongPho { get; set; }

    public virtual ICollection<PhongTro> PhongTros { get; set; } = new List<PhongTro>();
}
