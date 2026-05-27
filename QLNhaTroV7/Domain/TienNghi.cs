using System;
using System.Collections.Generic;

namespace QLNhaTroV7.Domain;

public partial class TienNghi
{
    public int Id { get; set; }

    public string TenTienNghi { get; set; } = null!;

    public string? Icon { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<PhongTro> PhongTros { get; set; } = new List<PhongTro>();
}
