using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.BaiDang;
using QLNhaTro.Models.ChuTro.CaiDat;
using QLNhaTro.Models.PhongTro;
using QLNhaTro.Repositories.BaiDang;
using QLNhaTro.Repositories.ChuTro;
using QLNhaTro.Repositories.NhaTro;
using QLNhaTro.Repositories.PhongTro;

namespace QLNhaTro.Controllers;

[Authorize(Roles = "CHU_TRO")]
public class ChuTroController : Controller
{
    private static readonly HashSet<string> HinhAnhExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const long MaxHinhAnhBytes = 5 * 1024 * 1024;

    private readonly IPhongTroManagementRepository _phongTroManagementRepository;
    private readonly INhaTroRepository _nhaTroRepository;
    private readonly IBaiDangRepository _baiDangRepository;
    private readonly IChuTroTaiKhoanRepository _chuTroTaiKhoanRepository;
    private readonly IChuTroYeuCauThueRepository _chuTroYeuCauThueRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ChuTroController(
        IPhongTroManagementRepository phongTroManagementRepository,
        INhaTroRepository nhaTroRepository,
        IBaiDangRepository baiDangRepository,
        IChuTroTaiKhoanRepository chuTroTaiKhoanRepository,
        IChuTroYeuCauThueRepository chuTroYeuCauThueRepository,
        IWebHostEnvironment webHostEnvironment)
    {
        _phongTroManagementRepository = phongTroManagementRepository;
        _nhaTroRepository = nhaTroRepository;
        _baiDangRepository = baiDangRepository;
        _chuTroTaiKhoanRepository = chuTroTaiKhoanRepository;
        _chuTroYeuCauThueRepository = chuTroYeuCauThueRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult TongQuan()
    {
        return RedirectToAction(nameof(PhongCuThe));
    }

    [HttpGet]
    public async Task<IActionResult> NhaTro()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        var danhSach = await _nhaTroRepository.GetNhaTroCuaChuTro(userId);
        return View(danhSach);
    }

    [HttpGet]
    public async Task<IActionResult> ThemSuaNhaTro(int? id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        var model = await _nhaTroRepository.GetForm(id, userId);
        if (model is null) return id is > 0 ? NotFound() : Challenge();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ThemSuaNhaTro(QLNhaTro.Models.NhaTro.NhaTroCreateUpdateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (!ModelState.IsValid)
        {
            model.DanhSachQuanHuyen = await _nhaTroRepository.GetQuanHuyenOptions();
            model.DanhSachXa = await _nhaTroRepository.GetXaOptions(model.QuanHuyenId);
            model.DanhSachDuongPho = await _nhaTroRepository.GetDuongPhoOptions(model.XaId);
            return View(model);
        }
        var result = model.Id <= 0 ? await _nhaTroRepository.CreateAsync(userId, model) : await _nhaTroRepository.UpdateAsync(userId, model);
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
    public async Task<IActionResult> YeuCauThue(string? trangThai, int? nhaTroId, string? sapXep)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");
        if (!int.TryParse(userId, out var nguoiDungId)) return RedirectToAction("Login", "Account");
        var model = await _chuTroYeuCauThueRepository.GetDanhSachAsync(nguoiDungId, trangThai, nhaTroId, sapXep);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTietYeuCauThue(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var nguoiDungId)) return RedirectToAction("Login", "Account");
        var model = await _chuTroYeuCauThueRepository.GetChiTietAsync(id, nguoiDungId);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy yêu cầu thuê hoặc bạn không có quyền xem.";
            return RedirectToAction(nameof(YeuCauThue));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> XuLyYeuCauThue(int id, string? hanhDong)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var nguoiDungId)) return RedirectToAction("Login", "Account");
        var model = await _chuTroYeuCauThueRepository.GetXuLyAsync(id, nguoiDungId);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy yêu cầu thuê hoặc bạn không có quyền xử lý.";
            return RedirectToAction(nameof(YeuCauThue));
        }
        model.TrangThaiXuLy = string.Equals(hanhDong, "tu-choi", StringComparison.OrdinalIgnoreCase)
            ? YeuCauThueStatus.ChuTroTuChoi
            : YeuCauThueStatus.ChuTroDongY;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XuLyYeuCauThue(QLNhaTro.Models.ChuTro.YeuCauThue.ChuTroXuLyYeuCauThueViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var nguoiDungId)) return RedirectToAction("Login", "Account");

        if (model.TrangThaiXuLy == YeuCauThueStatus.ChuTroTuChoi && string.IsNullOrWhiteSpace(model.LyDoTuChoi))
        {
            ModelState.AddModelError(nameof(model.LyDoTuChoi), "Vui lòng nhập lý do từ chối.");
        }

        if (!ModelState.IsValid)
        {
            var reload = await _chuTroYeuCauThueRepository.GetXuLyAsync(model.Id, nguoiDungId);
            if (reload is not null)
            {
                reload.TrangThaiXuLy = model.TrangThaiXuLy;
                reload.GhiChuChuTro = model.GhiChuChuTro;
                reload.LyDoTuChoi = model.LyDoTuChoi;
                return View(reload);
            }
            return View(model);
        }

        var result = await _chuTroYeuCauThueRepository.XuLyYeuCauAsync(model.Id, nguoiDungId, model.TrangThaiXuLy, model.GhiChuChuTro, model.LyDoTuChoi);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            var reload = await _chuTroYeuCauThueRepository.GetXuLyAsync(model.Id, nguoiDungId);
            if (reload is not null)
            {
                reload.TrangThaiXuLy = model.TrangThaiXuLy;
                reload.GhiChuChuTro = model.GhiChuChuTro;
                reload.LyDoTuChoi = model.LyDoTuChoi;
                return View(reload);
            }
            return View(model);
        }

        TempData["Success"] = result.Message;
        return RedirectToAction(nameof(YeuCauThue));
    }

    [HttpGet]
    public async Task<IActionResult> GetXaTheoQuanHuyen(int quanHuyenId)
    {
        if (quanHuyenId <= 0)
        {
            return Json(Array.Empty<object>());
        }

        var items = await _nhaTroRepository.GetXaOptions(quanHuyenId);
        return Json(items.Select(i => new { value = i.Value, text = i.Text }));
    }

    [HttpGet]
    public async Task<IActionResult> GetDuongPhoTheoXa(int xaId)
    {
        if (xaId <= 0)
        {
            return Json(Array.Empty<object>());
        }

        var items = await _nhaTroRepository.GetDuongPhoOptions(xaId);
        return Json(items.Select(i => new { value = i.Value, text = i.Text }));
    }

    [HttpGet]
    public IActionResult QuanLyDiaChi()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TienNghi(int phongTroId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (phongTroId <= 0)
        {
            TempData["Error"] = "Vui lòng chọn phòng cần gắn tiện nghi.";
            return RedirectToAction(nameof(PhongCuThe));
        }

        var model = await _phongTroManagementRepository.GetTienNghiPhongPageAsync(userId, phongTroId);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy phòng hoặc bạn không có quyền gắn tiện nghi cho phòng này.";
            return RedirectToAction(nameof(PhongCuThe));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuTienNghiPhong(int phongTroId, List<int>? tienNghiIds)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (phongTroId <= 0)
        {
            TempData["Error"] = "Vui lòng chọn phòng cần gắn tiện nghi.";
            return RedirectToAction(nameof(PhongCuThe));
        }

        var result = await _phongTroManagementRepository.LuuTienNghiPhongAsync(userId, phongTroId, tienNghiIds);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToAction(nameof(TienNghi), new { phongTroId });
    }

    [HttpGet]
    public async Task<IActionResult> HinhAnh(int phongTroId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (phongTroId <= 0)
        {
            TempData["Error"] = "Vui lòng chọn phòng cần quản lý hình ảnh.";
            return RedirectToAction(nameof(PhongCuThe));
        }

        var model = await _phongTroManagementRepository.GetHinhAnhPhongPageAsync(userId, phongTroId);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy phòng hoặc bạn không có quyền quản lý hình ảnh phòng này.";
            return RedirectToAction(nameof(PhongCuThe));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadHinhAnhPhong(int phongTroId, List<IFormFile>? files)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (phongTroId <= 0)
        {
            TempData["Error"] = "Vui lòng chọn phòng cần quản lý hình ảnh.";
            return RedirectToAction(nameof(PhongCuThe));
        }
        if (files is null || files.Count == 0 || files.All(f => f.Length == 0))
        {
            TempData["Error"] = "Vui lòng chọn ít nhất một ảnh để tải lên.";
            return RedirectToAction(nameof(HinhAnh), new { phongTroId });
        }

        var soAnhHienTai = await _phongTroManagementRepository.DemSoAnhPhongAsync(phongTroId);
        var fileHopLe = new List<(IFormFile File, string Extension)>();
        foreach (var file in files)
        {
            if (file.Length == 0) continue;
            if (file.Length > MaxHinhAnhBytes)
            {
                TempData["Error"] = $"Ảnh \"{file.FileName}\" vượt quá 5MB.";
                return RedirectToAction(nameof(HinhAnh), new { phongTroId });
            }
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension) || !HinhAnhExtensions.Contains(extension))
            {
                TempData["Error"] = $"Ảnh \"{file.FileName}\" không đúng định dạng (JPG, JPEG, PNG, WebP).";
                return RedirectToAction(nameof(HinhAnh), new { phongTroId });
            }
            fileHopLe.Add((file, extension.ToLowerInvariant()));
        }

        if (fileHopLe.Count == 0)
        {
            TempData["Error"] = "Vui lòng chọn ít nhất một ảnh hợp lệ để tải lên.";
            return RedirectToAction(nameof(HinhAnh), new { phongTroId });
        }

        if (soAnhHienTai + fileHopLe.Count > HinhAnhPhongPageViewModel.MaxAnhMoiPhong)
        {
            TempData["Error"] = $"Mỗi phòng tối đa {HinhAnhPhongPageViewModel.MaxAnhMoiPhong} ảnh (hiện có {soAnhHienTai}).";
            return RedirectToAction(nameof(HinhAnh), new { phongTroId });
        }

        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "phong-tro", phongTroId.ToString());
        Directory.CreateDirectory(uploadDir);
        var duongDanDaLuu = new List<string>();
        var fileDaGhi = new List<string>();

        try
        {
            foreach (var (file, extension) in fileHopLe)
            {
                var fileName = $"{Guid.NewGuid()}{extension}";
                var physicalPath = Path.Combine(uploadDir, fileName);
                await using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                fileDaGhi.Add(physicalPath);
                duongDanDaLuu.Add($"/uploads/phong-tro/{phongTroId}/{fileName}");
            }

            var result = await _phongTroManagementRepository.ThemHinhAnhPhongAsync(userId, phongTroId, duongDanDaLuu);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                foreach (var path in fileDaGhi)
                {
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                TempData["Error"] = result.Message;
            }
        }
        catch
        {
            foreach (var path in fileDaGhi)
            {
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            TempData["Error"] = "Không thể tải ảnh lên. Vui lòng thử lại.";
        }

        return RedirectToAction(nameof(HinhAnh), new { phongTroId });
    }

    [HttpGet]
    public async Task<IActionResult> PhongCuThe(string? tuKhoa, int? nhaTroId, int? tang, string? trangThai)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        var model = await _phongTroManagementRepository.GetDanhSachPhongAsync(userId, tuKhoa, nhaTroId, tang, trangThai);
        ViewData["ActiveLandlordMenu"] = "PhongCuThe";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ThemSuaPhongCuThe(int? id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        var model = await _phongTroManagementRepository.GetPhongFormAsync(id, userId);
        if (model is null) return id is > 0 ? NotFound() : Challenge();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemSuaPhongCuThe(PhongTroCreateUpdateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (!ModelState.IsValid)
        {
            model.DanhSachNhaTro = await _phongTroManagementRepository.GetDanhSachNhaTroCuaChuTroAsync(userId);
            return View(model);
        }

        var result = model.Id <= 0 ? await _phongTroManagementRepository.CreatePhong(userId, model) : await _phongTroManagementRepository.UpdatePhongAsync(userId, model);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(PhongCuThe));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        model.DanhSachNhaTro = await _phongTroManagementRepository.GetDanhSachNhaTroCuaChuTroAsync(userId);
        return View("ThemSuaPhongCuThe", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("ThemPhong")]
    public Task<IActionResult> ThemPhong(PhongTroCreateUpdateViewModel model) => ThemSuaPhongCuThe(model);

    [HttpGet]
    public async Task<IActionResult> BaiDang(string? tuKhoa, int? nhaTroId, string? trangThaiDuyet)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        var model = await _baiDangRepository.GetDanhSachBaiDangAsync(userId, tuKhoa, nhaTroId, trangThaiDuyet);
        ViewData["ActiveLandlordMenu"] = "BaiDang";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ThemSuaBaiDang(int? id, int? nhaTroId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        BaiDangCreateUpdateViewModel? model = id is > 0
            ? await _baiDangRepository.GetUpdateModelAsync(userId, id.Value)
            : await _baiDangRepository.GetCreateModelAsync(userId, nhaTroId);
        if (model is null)
        {
            TempData["Error"] = id is > 0 ? "Chỉ có thể sửa bài đăng ở trạng thái Nháp." : "Không tìm thấy bài đăng hoặc bạn không có quyền chỉnh sửa.";
            return RedirectToAction(nameof(BaiDang));
        }
        ViewData["ActiveLandlordMenu"] = "BaiDang";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemSuaBaiDang(BaiDangCreateUpdateViewModel model, string submitAction)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (!ModelState.IsValid)
        {
            await TaiLaiFormBaiDangAsync(userId, model);
            ViewData["ActiveLandlordMenu"] = "BaiDang";
            return View(model);
        }

        if (submitAction == "GuiChoDuyet")
        {
            var baiDangId = await _baiDangRepository.LuuNhapVaTraVeIdAsync(userId, model);
            if (baiDangId is null)
            {
                TempData["Error"] = "Không thể lưu bài đăng. Vui lòng kiểm tra phòng đã chọn và thử lại.";
                await TaiLaiFormBaiDangAsync(userId, model);
                ViewData["ActiveLandlordMenu"] = "BaiDang";
                return View(model);
            }
            return RedirectToAction(nameof(GuiBaiChoDuyet), new { id = baiDangId.Value });
        }

        var luuNhapThanhCong = await _baiDangRepository.LuuNhapAsync(userId, model);
        if (!luuNhapThanhCong)
        {
            TempData["Error"] = "Không thể lưu bài đăng. Vui lòng kiểm tra phòng đã chọn và thử lại.";
            await TaiLaiFormBaiDangAsync(userId, model);
            ViewData["ActiveLandlordMenu"] = "BaiDang";
            return View(model);
        }

        TempData["Success"] = "Đã lưu nháp bài đăng.";
        return RedirectToAction(nameof(BaiDang));
    }

    [HttpGet]
    public async Task<IActionResult> GuiBaiChoDuyet(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Challenge();
        if (id <= 0)
        {
            TempData["Error"] = "Bài đăng không hợp lệ.";
            return RedirectToAction(nameof(BaiDang));
        }

        var model = await _baiDangRepository.GetGuiBaiChoDuyetAsync(userId, id);
        if (model is null)
        {
            TempData["Error"] = "Không tìm thấy bài đăng nháp hoặc bạn không có quyền gửi bài này.";
            return RedirectToAction(nameof(BaiDang));
        }
        ViewData["ActiveLandlordMenu"] = "BaiDang";
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhanGuiBaiChoDuyet(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (id <= 0)
        {
            TempData["Error"] = "Bài đăng không hợp lệ.";
            return RedirectToAction(nameof(BaiDang));
        }

        var thanhCong = await _baiDangRepository.XacNhanGuiChoDuyetAsync(userId, id);
        if (!thanhCong)
        {
            TempData["Error"] = "Không thể gửi bài đăng. Chỉ bài ở trạng thái Nháp mới được gửi chờ duyệt.";
            return RedirectToAction(nameof(BaiDang));
        }

        TempData["Success"] = "Đã gửi bài đăng chờ admin duyệt.";
        return RedirectToAction(nameof(BaiDang));
    }
    private async Task TaiLaiFormBaiDangAsync(string userId, BaiDangCreateUpdateViewModel model)
    {
        if (model.Id is > 0)
        {
            var reload = await _baiDangRepository.GetUpdateModelAsync(userId, model.Id.Value);
            if (reload is not null)
            {
                model.DanhSachNhaTro = reload.DanhSachNhaTro;
                model.DanhSachPhong = reload.DanhSachPhong;
                return;
            }
        }

        var create = await _baiDangRepository.GetCreateModelAsync(userId, model.NhaTroId);
        model.DanhSachNhaTro = create.DanhSachNhaTro;
        model.DanhSachPhong = create.DanhSachPhong;
    }

    [HttpGet]
    public async Task<IActionResult> CaiDat()
    {
        ViewData["Title"] = "Cài đặt";
        ViewData["ActiveLandlordMenu"] = "CaiDat";
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return RedirectToAction("Login", "Account");
        var model = await _chuTroTaiKhoanRepository.GetCaiDatAsync(userId);
        if (model is null) return RedirectToAction("Login", "Account");
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongTin([Bind(Prefix = "ThongTin")] CapNhatThongTinViewModel model)
    {
        ViewData["Title"] = "Cài đặt";
        ViewData["ActiveLandlordMenu"] = "CaiDat";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var pageModel = await _chuTroTaiKhoanRepository.GetCaiDatAsync(userId)
                ?? new ChuTroCaiDatViewModel();

            pageModel.ThongTin = model;
            pageModel.DoiMatKhau = new DoiMatKhauViewModel();

            return View("CaiDat", pageModel);
        }

        var result = await _chuTroTaiKhoanRepository.CapNhatThongTinAsync(userId, model);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(CaiDat));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiMatKhau([Bind(Prefix = "DoiMatKhau")] DoiMatKhauViewModel model)
    {
        ViewData["Title"] = "Cài đặt";
        ViewData["ActiveLandlordMenu"] = "CaiDat";

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var pageModel = await _chuTroTaiKhoanRepository.GetCaiDatAsync(userId)
                ?? new ChuTroCaiDatViewModel();

            pageModel.DoiMatKhau = new DoiMatKhauViewModel();
            return View("CaiDat", pageModel);
        }

        var result = await _chuTroTaiKhoanRepository.DoiMatKhauAsync(userId, model);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(CaiDat));
        }

        TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
