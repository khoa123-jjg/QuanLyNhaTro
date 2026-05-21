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
    // Nếu id có giá trị thì là sửa, ngược lại là thêm mới
    //Là phương thức khi vừa load form thêm mới hoặc sửa
    // [HttpGet]
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
    // Nếu model.Id có giá trị thì là sửa, ngược lại là thêm mới
    // Là phương thức xử lý khi submit form thêm mới hoặc sửa
    // [HttpPost]
    public async Task<IActionResult> ThemSuaNhaTro(QLNhaTro.Models.NhaTro.NhaTroCreateUpdateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            // Nếu có lỗi validate, cần load lại các dropdown để hiển thị form đúng
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
        // Nếu có lỗi từ service/repository, cũng cần load lại các dropdown để hiển thị form đúng
        model.DanhSachQuanHuyen = await _nhaTroRepository.GetQuanHuyenOptions();
        model.DanhSachXa = await _nhaTroRepository.GetXaOptions(model.QuanHuyenId);
        model.DanhSachDuongPho = await _nhaTroRepository.GetDuongPhoOptions(model.XaId);
        return View(model);
    }

    [HttpGet]
    // Ân vào quận sẽ hiện xã theo id quận đó
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
    // Ấn vào xã sẽ hiện đường phố theo id xã đó
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
    public async Task<IActionResult> TienNghi(int? phongTroId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var model = await _phongTroManagementRepository.GetGanTienNghiTrangAsync(phongTroId, userId);
        if (model is null)
        {
            return Challenge();
        }

        if (!phongTroId.HasValue && model.DanhSachPhong.Count > 0)
        {
            if (int.TryParse(model.DanhSachPhong[0].Value, out var firstPhongId))
            {
                return RedirectToAction(nameof(TienNghi), new { phongTroId = firstPhongId });
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult HinhAnh()
    {
        return View();
    }
    // Trả về danh sách các phòng theo tìm kiếm và lọc, nếu có
    [HttpGet]
    public async Task<IActionResult> PhongCuThe(string? tuKhoa, int? nhaTroId, int? tang, string? trangThai)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var model = await _phongTroManagementRepository.GetDanhSachPhongAsync(
            userId, tuKhoa, nhaTroId, tang, trangThai);

        return View(model);
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
        if (model is null)
        {
            return id is > 0 ? NotFound() : Challenge();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemSuaPhongCuThe(PhongTroCreateUpdateViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            //Lấy danh sách nhà trọ đổ vào dropdown để nhập và phòng của chủ trọ sau mỗi lần lỗi valdate
            model.DanhSachNhaTro = await _phongTroManagementRepository.GetDanhSachNhaTroCuaChuTroAsync(userId);
            return View(model);
        }
        // Nếu model có id tức là đang cập nhập thì sẽ Update, ngược lại sẽ Create
        var result = model.Id <= 0? await _phongTroManagementRepository.CreatePhong(userId, model): await _phongTroManagementRepository.UpdatePhongAsync(userId, model);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
            //Thành công quay trở về trang phòng
            return RedirectToAction(nameof(PhongCuThe));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        //Lấy danh sách nhà trọ đổ vào dropdown để nhập và phòng của chủ trọ sau mỗi lần lỗi valdate
        model.DanhSachNhaTro = await _phongTroManagementRepository.GetDanhSachNhaTroCuaChuTroAsync(userId);
        return View("ThemSuaPhongCuThe", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("ThemPhong")]
    public Task<IActionResult> ThemPhong(PhongTroCreateUpdateViewModel model) =>
        ThemSuaPhongCuThe(model);

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
