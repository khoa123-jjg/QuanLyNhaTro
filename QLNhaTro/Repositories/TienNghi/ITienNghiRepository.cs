using QLNhaTro.Models.Admin.TienNghi;

namespace QLNhaTro.Repositories.TienNghi;

public interface ITienNghiRepository
{
    Task<TienNghiPageViewModel> GetPageAsync(int? id);
    Task<bool> LuuTienNghiAsync(TienNghiFormViewModel model);
    Task<bool> DoiTrangThaiAsync(int id);
    Task<(bool Success, string Message)> XoaTienNghiAsync(int id);
}
