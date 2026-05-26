using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Models.Admin.DiaChi;
using QLNhaTro.Models.Admin.HoSo;
using QLNhaTro.Models.Admin.TienNghi;
using QLNhaTro.Repositories.Admin;
using QLNhaTro.Repositories.DiaChi;
using QLNhaTro.Repositories.TienNghi;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "ADMIN")]
public class AdminController : Controller
{
    private readonly ITienNghiRepository _tienNghiRepository;
    private readonly IAdminDiaChiRepository _adminDiaChiRepository;
    private readonly IAdminTaiKhoanRepository _adminTaiKhoanRepository;

    public AdminController(
        ITienNghiRepository tienNghiRepository,
        IAdminDiaChiRepository adminDiaChiRepository,
        IAdminTaiKhoanRepository adminTaiKhoanRepository)
    {
        _tienNghiRepository = tienNghiRepository;
        _adminDiaChiRepository = adminDiaChiRepository;
        _adminTaiKhoanRepository = adminTaiKhoanRepository;
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
    public async Task<IActionResult> DonViHanhChinh(string? tuKhoa, int? quanHuyenId, int? id)
    {
        ViewData["ActiveAdminMenu"] = "DonViHanhChinh";
        var model = await _adminDiaChiRepository.GetXaPageAsync(tuKhoa, quanHuyenId, id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuXa(XaFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            var model = await _adminDiaChiRepository.GetXaPageAsync(null, null, form.Id);
            model.Form = form;
            return View("DonViHanhChinh", model);
        }

        var result = await _adminDiaChiRepository.LuuXaAsync(form);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(DonViHanhChinh));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaXa(int id)
    {
        var result = await _adminDiaChiRepository.XoaXaAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(DonViHanhChinh));
    }

    [HttpGet]
    public async Task<IActionResult> DuongPho(string? tuKhoa, int? xaId, int? id)
    {
        ViewData["ActiveAdminMenu"] = "DuongPho";
        var model = await _adminDiaChiRepository.GetDuongPhoPageAsync(tuKhoa, xaId, id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuDuongPho(DuongPhoFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            var model = await _adminDiaChiRepository.GetDuongPhoPageAsync(null, null, form.Id);
            model.Form = form;
            return View("DuongPho", model);
        }

        var result = await _adminDiaChiRepository.LuuDuongPhoAsync(form);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(DuongPho));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaDuongPho(int id)
    {
        var result = await _adminDiaChiRepository.XoaDuongPhoAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(DuongPho));
    }

    [HttpGet]
    public async Task<IActionResult> HoSo()
    {
        ViewData["Title"] = "Hồ sơ admin";
        ViewData["ActiveAdminMenu"] = "HoSo";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var model = await _adminTaiKhoanRepository.GetHoSoAsync(userId);
        if (model is null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiMatKhauAdmin([Bind(Prefix = "DoiMatKhau")] AdminDoiMatKhauViewModel model)
    {
        ViewData["Title"] = "Hồ sơ admin";
        ViewData["ActiveAdminMenu"] = "HoSo";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var pageModel = await _adminTaiKhoanRepository.GetHoSoAsync(userId);
            if (pageModel is null)
            {
                return RedirectToAction("Login", "Account");
            }

            pageModel.DoiMatKhau = new AdminDoiMatKhauViewModel();
            return View("HoSo", pageModel);
        }

        var result = await _adminTaiKhoanRepository.DoiMatKhauAsync(userId, model);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(HoSo));
        }

        TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
