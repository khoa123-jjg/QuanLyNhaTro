using System;
using System.Collections.Generic;

namespace QLNhaTroV7.Domain;

public partial class HinhAnh
{
    public int Id { get; set; }

    public int PhongTroId { get; set; }

    public string DuongDanAnh { get; set; } = null!;

    public bool LaAnhDaiDien { get; set; }

    public int ThuTuHienThi { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual PhongTro PhongTro { get; set; } = null!;
}
