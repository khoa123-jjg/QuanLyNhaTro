using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class Xa
{
    public int Id { get; set; }

    public int Quanhuyenid { get; set; }

    public string TenXahuyen { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<DuongPho> DuongPhos { get; set; } = new List<DuongPho>();

    public virtual Quanhuyen Quanhuyen { get; set; } = null!;
}
