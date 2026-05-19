using System;
using System.Collections.Generic;

namespace QuanLyNhaTro.Domain;

public partial class HinhAnh
{
    public int Id { get; set; }

    public int LoaiPhongId { get; set; }

    public string DuongDanAnh { get; set; } = null!;

    public bool LaAnhDaiDien { get; set; }

    public int ThuTuHienThi { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual LoaiPhong LoaiPhong { get; set; } = null!;
}
