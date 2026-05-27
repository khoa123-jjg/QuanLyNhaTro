using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class VaiTro
{
    public int Id { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<NguoiDungVaiTro> NguoiDungVaiTros { get; set; } = new List<NguoiDungVaiTro>();
}
