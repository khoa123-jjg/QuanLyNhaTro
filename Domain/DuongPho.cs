using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class DuongPho
{
    public int Id { get; set; }

    public int DonViHanhChinhId { get; set; }

    public string TenDuong { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();

    public virtual DonViHanhChinh DonViHanhChinh { get; set; } = null!;
}
