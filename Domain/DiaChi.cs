using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class DiaChi
{
    public int Id { get; set; }

    public int DonViHanhChinhId { get; set; }

    public int? DuongPhoId { get; set; }

    public string? SoNha { get; set; }

    public string DiaChiChiTiet { get; set; } = null!;

    public decimal? ViDo { get; set; }

    public decimal? KinhDo { get; set; }

    public virtual DonViHanhChinh DonViHanhChinh { get; set; } = null!;

    public virtual DuongPho? DuongPho { get; set; }

    public virtual NhaTro? NhaTro { get; set; }
}
