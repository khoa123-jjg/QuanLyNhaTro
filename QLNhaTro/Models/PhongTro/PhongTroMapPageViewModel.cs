namespace QLNhaTro.Models.PhongTro;

public class PhongTroMapPageViewModel
{
    public List<PhongTroMapItemViewModel> PhongTros { get; set; } = new();

    public int? FocusPhongTroId { get; set; }
}
