using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.Admin.TienNghi;
using QLNhaTro.Repositories.TienNghi;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "ADMIN")]
public class AdminController : Controller
{
    private readonly ITienNghiRepository _tienNghiRepository;

    public AdminController(ITienNghiRepository tienNghiRepository)
    {
        _tienNghiRepository = tienNghiRepository;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    public IActionResult NguoiDung()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTietNguoiDung()
    {
        return View();
    }

    [HttpGet]
    public IActionResult KhoaMoKhoaTaiKhoan()
    {
        return View();
    }

    [HttpGet]
    public IActionResult BaiChoDuyet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ChiTietBaiChoDuyet()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DuyetBai()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TuChoiBai()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TienNghi(int? id)
    {
        ViewData["ActiveAdminMenu"] = "TienNghi";
        var model = await _tienNghiRepository.GetPageAsync(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuTienNghi(TienNghiFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            var pageModel = await _tienNghiRepository.GetPageAsync(form.Id);
            pageModel.Form = form;
            return View("TienNghi", pageModel);
        }

        var success = await _tienNghiRepository.LuuTienNghiAsync(form);
        if (success)
        {
            TempData["Success"] = "Đã lưu tiện nghi thành công.";
        }
        else
        {
            TempData["Error"] = "Không thể lưu tiện nghi.";
        }

        return RedirectToAction(nameof(TienNghi));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThaiTienNghi(int id)
    {
        var success = await _tienNghiRepository.DoiTrangThaiAsync(id);
        TempData[success ? "Success" : "Error"] = success ? "Đã cập nhật trạng thái tiện nghi." : "Không thể cập nhật trạng thái tiện nghi.";
        return RedirectToAction(nameof(TienNghi));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaTienNghi(int id)
    {
        var result = await _tienNghiRepository.XoaTienNghiAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(TienNghi));
    }

    [HttpGet]
    public IActionResult DonViHanhChinh()
    {
        return View();
    }

    [HttpGet]
    public IActionResult DuongPho()
    {
        return View();
    }

    [HttpGet]
    public IActionResult HoSo()
    {
        return View();
    }
}
