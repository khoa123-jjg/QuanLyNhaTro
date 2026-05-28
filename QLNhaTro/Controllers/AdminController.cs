using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Models.Admin.BaiDuyet;
using QLNhaTro.Models.Admin.Dashboard;
using QLNhaTro.Models.Admin.DiaChi;
using QLNhaTro.Models.Admin.HoSo;
using QLNhaTro.Models.Admin.NguoiDung;
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
    private readonly IAdminNguoiDungRepository _adminNguoiDungRepository;
    private readonly IAdminBaiDuyetRepository _adminBaiDuyetRepository;
    private readonly IAdminDashboardRepository _adminDashboardRepository;

    public AdminController(
        ITienNghiRepository tienNghiRepository,
        IAdminDiaChiRepository adminDiaChiRepository,
        IAdminTaiKhoanRepository adminTaiKhoanRepository,
        IAdminNguoiDungRepository adminNguoiDungRepository,
        IAdminBaiDuyetRepository adminBaiDuyetRepository,
        IAdminDashboardRepository adminDashboardRepository)
    {
        _tienNghiRepository = tienNghiRepository;
        _adminDiaChiRepository = adminDiaChiRepository;
        _adminTaiKhoanRepository = adminTaiKhoanRepository;
        _adminNguoiDungRepository = adminNguoiDungRepository;
        _adminBaiDuyetRepository = adminBaiDuyetRepository;
        _adminDashboardRepository = adminDashboardRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        ViewData["Title"] = "Dashboard Admin";
        ViewData["ActiveAdminMenu"] = "Dashboard";

        var model = await _adminDashboardRepository.GetDashboardAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> NguoiDung(string? tuKhoa, string? vaiTro, string? trangThai)
    {
        ViewData["Title"] = "Quản lý người dùng";
        ViewData["ActiveAdminMenu"] = "NguoiDung";

        var model = await _adminNguoiDungRepository.GetDanhSachNguoiDungAsync(tuKhoa, vaiTro, trangThai);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTietNguoiDung(int id)
    {
        ViewData["Title"] = "Chi tiết người dùng";
        ViewData["ActiveAdminMenu"] = "NguoiDung";

        var model = await _adminNguoiDungRepository.GetChiTietNguoiDungAsync(id);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy người dùng.";
            return RedirectToAction(nameof(NguoiDung));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoaMoKhoaNguoiDung(int id)
    {
        var adminUserIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(adminUserIdText, out var adminId))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _adminNguoiDungRepository.KhoaMoKhoaNguoiDungAsync(id, adminId);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(NguoiDung));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaNguoiDung(int id)
    {
        var adminUserIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(adminUserIdText, out var adminId))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _adminNguoiDungRepository.XoaNguoiDungAsync(id, adminId);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(NguoiDung));
    }

    [HttpGet]
    public async Task<IActionResult> BaiChoDuyet(string? tuKhoa, int? nhaTroId)
    {
        ViewData["ActiveAdminMenu"] = "BaiChoDuyet";
        var model = await _adminBaiDuyetRepository.GetDanhSachChoDuyetAsync(tuKhoa, nhaTroId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTietBaiChoDuyet(int id)
    {
        ViewData["ActiveAdminMenu"] = "BaiChoDuyet";
        var model = await _adminBaiDuyetRepository.GetChiTietChoDuyetAsync(id);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy bài đăng đang chờ duyệt.";
            return RedirectToAction(nameof(BaiChoDuyet));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DuyetBai(int id)
    {
        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(adminUserId))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _adminBaiDuyetRepository.DuyetBaiAsync(id, adminUserId);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(BaiChoDuyet));
    }

    [HttpGet]
    public async Task<IActionResult> TuChoiBai(int id)
    {
        ViewData["ActiveAdminMenu"] = "BaiChoDuyet";
        var model = await _adminBaiDuyetRepository.GetTuChoiViewModelAsync(id);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy bài đăng đang chờ duyệt.";
            return RedirectToAction(nameof(BaiChoDuyet));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoiBai(AdminTuChoiBaiViewModel model)
    {
        ViewData["ActiveAdminMenu"] = "BaiChoDuyet";
        if (!ModelState.IsValid)
        {
            var reload = await _adminBaiDuyetRepository.GetTuChoiViewModelAsync(model.Id);
            if (reload is null)
            {
                TempData["Error"] = "Không tìm thấy bài đăng đang chờ duyệt.";
                return RedirectToAction(nameof(BaiChoDuyet));
            }

            reload.LyDoTuChoi = model.LyDoTuChoi;
            return View(reload);
        }

        var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(adminUserId))
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _adminBaiDuyetRepository.TuChoiBaiAsync(model.Id, adminUserId, model.LyDoTuChoi);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(BaiChoDuyet));
        }

        TempData["Error"] = result.Message;
        return RedirectToAction(nameof(BaiChoDuyet));
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
