using System;
using System.Collections.Generic;

namespace QLNhaTro.Domain;

public partial class NguoiThue
{
    public int Id { get; set; }

    public int NguoiDungId { get; set; }

    public string? NgheNghiep { get; set; }

    public string? NhuCauThue { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual ICollection<DatThue> DatThues { get; set; } = new List<DatThue>();

    public virtual NguoiDung NguoiDung { get; set; } = null!;
}
