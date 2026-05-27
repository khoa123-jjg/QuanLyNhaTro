using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class DuongPho
{
    public int Id { get; set; }

    public int Xaid { get; set; }

    public string TenDuong { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<NhaTro> NhaTros { get; set; } = new List<NhaTro>();

    public virtual Xa Xa { get; set; } = null!;
}
