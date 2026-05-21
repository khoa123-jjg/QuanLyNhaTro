using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Models.PhongTro;
using QLNhaTro.Repositories.NhaTro;
using QLNhaTro.Repositories.PhongTro;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "CHU_TRO")]
public class ChuTroController : Controller
{
    private readonly IPhongTroManagementRepository _phongTroManagementRepository;
    private readonly INhaTroRepository _nhaTroRepository;

    public ChuTroController(
        IPhongTroManagementRepository phongTroManagementRepository,
        INhaTroRepository nhaTroRepository)
    {
        _phongTroManagementRepository = phongTroManagementRepository;
        _nhaTroRepository = nhaTroRepository;
    }

    [HttpGet]
    public IActionResult TongQuan()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> NhaTro()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var danhSach = await _nhaTroRepository.GetNhaTroCuaChuTro(userId);
        return View(danhSach);
    }

    [HttpGet]
    public async Task<IActionResult> ThemSuaNhaTro(int? id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var model = await _nhaTroRepository.GetForm(id, userId);
        if (model is null)
        {
            return id is > 0 ? NotFound() : Challenge();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemSuaNhaTro(QLNhaTro.Models.NhaTro.NhaTroCreateUpdateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            model.DanhSachQuanHuyen = await _nhaTroRepository.GetQuanHuyenOptions();
            model.DanhSachXa = await _nhaTroRepository.GetXaOptions(model.QuanHuyenId);
            model.DanhSachDuongPho = await _nhaTroRepository.GetDuongPhoOptions(model.XaId);
            return View(model);
        }

        var result = model.Id <= 0
            ? await _nhaTroRepository.CreateAsync(userId, model)
            : await _nhaTroRepository.UpdateAsync(userId, model);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(NhaTro));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        model.DanhSachQuanHuyen = await _nhaTroRepository.GetQuanHuyenOptions();
        model.DanhSachXa = await _nhaTroRepository.GetXaOptions(model.QuanHuyenId);
        model.DanhSachDuongPho = await _nhaTroRepository.GetDuongPhoOptions(model.XaId);
        return View(model);
    }

    [HttpGet]
    public IActionResult QuanLyDiaChi()
    {
        return View();
    }


    [HttpGet]
    public IActionResult TienNghi()
    {
        return View();
    }

    [HttpGet]
    public IActionResult HinhAnh()
    {
        return View();
    }

    [HttpGet]
    public IActionResult PhongCuThe()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ThemSuaPhongCuThe(int? id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var model = await _phongTroManagementRepository.GetPhongFormAsync(id, userId);
        if (id is > 0 && model is null)
        {
            return NotFound();
        }

        return View(model!);
    }

    [HttpPost]
    public async Task<IActionResult> ThemPhong(PhongTroCreateUpdateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            model.DanhSachNhaTro = await _phongTroManagementRepository.GetDanhSachNhaTroCuaChuTroAsync(userId);
            return View(model);
        }

        var result = model.Id <= 0
            ? await _phongTroManagementRepository.CreatePhong(userId, model)
            : await _phongTroManagementRepository.UpdatePhongAsync(userId, model);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(PhongCuThe));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        model.DanhSachNhaTro = await _phongTroManagementRepository.GetDanhSachNhaTroCuaChuTroAsync(userId);
        return View(model);
    }

    [HttpGet]
    public IActionResult BaiDang()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ThemSuaBaiDang()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GuiBaiChoDuyet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult YeuCauThue()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTietYeuCauThue()
    {
        return View();
    }

    [HttpGet]
    public IActionResult XuLyYeuCauThue()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CaiDat()
    {
        return View();
    }

   
}
