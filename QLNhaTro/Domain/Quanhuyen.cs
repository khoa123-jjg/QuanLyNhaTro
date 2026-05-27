using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class Quanhuyen
{
    public int Id { get; set; }

    public string TenQuanhuyen { get; set; } = null!;

    public string ThanhPho { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<Xa> Xas { get; set; } = new List<Xa>();
}
