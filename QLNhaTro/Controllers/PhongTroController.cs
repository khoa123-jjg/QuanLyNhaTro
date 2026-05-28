using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Repositories.PhongTro;

namespace QLNhaTro.Controllers;

public class PhongTroController : Controller
{
    private readonly IPhongTroRepository _phongTroRepository;

    public PhongTroController(IPhongTroRepository phongTroRepository)
    {
        _phongTroRepository = phongTroRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? khuVuc, string? mucGia, string? dienTich)
    {
        var model = await _phongTroRepository.SearchPhongAsync(khuVuc, mucGia, dienTich);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id)
    {
        var model = await _phongTroRepository.GetChiTietPhongAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> BanDo(int? id)
    {
        var model = await _phongTroRepository.GetBanDoPhongAsync(id);
        return View(model);
    }
}
