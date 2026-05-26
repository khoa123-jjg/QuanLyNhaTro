using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Models.NguoiThue.HoSo;
using QLNhaTro.Repositories.NguoiThue;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "NGUOI_THUE")]
public class NguoiThueController : Controller
{
    private readonly INguoiThueHoSoRepository _nguoiThueHoSoRepository;

    public NguoiThueController(INguoiThueHoSoRepository nguoiThueHoSoRepository)
    {
        _nguoiThueHoSoRepository = nguoiThueHoSoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> HoSo()
    {
        ViewData["Title"] = "Hồ sơ cá nhân";
        ViewData["ActiveTenantMenu"] = "HoSo";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var model = await _nguoiThueHoSoRepository.GetHoSoAsync(userId);
        if (model is null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatHoSo([Bind(Prefix = "ThongTin")] CapNhatHoSoNguoiThueViewModel model)
    {
        ViewData["Title"] = "Hồ sơ cá nhân";
        ViewData["ActiveTenantMenu"] = "HoSo";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var pageModel = await _nguoiThueHoSoRepository.GetHoSoAsync(userId) ?? new NguoiThueHoSoViewModel();
            pageModel.ThongTin = model;
            pageModel.DoiMatKhau = new DoiMatKhauNguoiThueViewModel();
            return View("HoSo", pageModel);
        }

        var result = await _nguoiThueHoSoRepository.CapNhatHoSoAsync(userId, model);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(HoSo));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiMatKhau([Bind(Prefix = "DoiMatKhau")] DoiMatKhauNguoiThueViewModel model)
    {
        ViewData["Title"] = "Hồ sơ cá nhân";
        ViewData["ActiveTenantMenu"] = "HoSo";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var pageModel = await _nguoiThueHoSoRepository.GetHoSoAsync(userId) ?? new NguoiThueHoSoViewModel();
            pageModel.DoiMatKhau = new DoiMatKhauNguoiThueViewModel();
            return View("HoSo", pageModel);
        }

        var result = await _nguoiThueHoSoRepository.DoiMatKhauAsync(userId, model);
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
