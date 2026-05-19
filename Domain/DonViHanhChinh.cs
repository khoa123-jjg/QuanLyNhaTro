using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class DonViHanhChinh
{
    public int Id { get; set; }

    public string TenDonVi { get; set; } = null!;

    public string LoaiDonVi { get; set; } = null!;

    public string ThanhPho { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();

    public virtual ICollection<DuongPho> DuongPhos { get; set; } = new List<DuongPho>();
}
