using Microsoft.AspNetCore.Mvc.Rendering;
using QLNhaTro.Models.NhaTro;

namespace QLNhaTro.Repositories.NhaTro;

public class NhaTroRepositoryResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;
}

public interface INhaTroRepository
{
    Task<List<NhaTroListItemViewModel>> GetNhaTroCuaChuTro(string userId);

    Task<NhaTroCreateUpdateViewModel?> GetForm(int? id, string userId);

    Task<NhaTroRepositoryResult> CreateAsync(string userId, NhaTroCreateUpdateViewModel model);

    Task<NhaTroRepositoryResult> UpdateAsync(string userId, NhaTroCreateUpdateViewModel model);

    Task<List<SelectListItem>> GetQuanHuyenOptions();

    Task<List<SelectListItem>> GetXaOptions(int? quanHuyenId);

    Task<List<SelectListItem>> GetDuongPhoOptions(int? xaId);
}
