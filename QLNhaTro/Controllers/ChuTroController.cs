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
    private static readonly HashSet<string> HinhAnhExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const long MaxHinhAnhBytes = 5 * 1024 * 1024;

    private readonly IPhongTroManagementRepository _phongTroManagementRepository;
    private readonly INhaTroRepository _nhaTroRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ChuTroController(
        IPhongTroManagementRepository phongTroManagementRepository,
        INhaTroRepository nhaTroRepository,
        IWebHostEnvironment webHostEnvironment)
    {
        _phongTroManagementRepository = phongTroManagementRepository;
        _nhaTroRepository = nhaTroRepository;
        _webHostEnvironment = webHostEnvironment;
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
    public async Task<IActionResult> TienNghi(int phongTroId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

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
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (phongTroId <= 0)
        {
            TempData["Error"] = "Vui lòng chọn phòng cần gắn tiện nghi.";
            return RedirectToAction(nameof(PhongCuThe));
        }

        var result = await _phongTroManagementRepository.LuuTienNghiPhongAsync(userId, phongTroId, tienNghiIds);

        if (result.Success)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(TienNghi), new { phongTroId });
    }
    [HttpGet]
    public async Task<IActionResult> HinhAnh(int phongTroId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

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
    // Xử lý khi submit form upload ảnh phòng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadHinhAnhPhong(int phongTroId, List<IFormFile>? files)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (phongTroId <= 0)
        {
            TempData["Error"] = "Vui lòng chọn phòng cần quản lý hình ảnh.";
            return RedirectToAction(nameof(PhongCuThe));
        }
        //Chống null làm lỗi chương trình
        if (files is null || files.Count == 0 || files.All(f => f.Length == 0))
        {
            TempData["Error"] = "Vui lòng chọn ít nhất một ảnh để tải lên.";
            return RedirectToAction(nameof(HinhAnh), new { phongTroId });
        }

        var soAnhHienTai = await _phongTroManagementRepository.DemSoAnhPhongAsync(phongTroId);
       // List chưa tên file và đuôi file
        var fileHopLe = new List<(IFormFile File, string Extension)>();

        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                continue;
            }

            if (file.Length > MaxHinhAnhBytes)
            {
                TempData["Error"] = $"Ảnh \"{file.FileName}\" vượt quá 5MB.";
                // Chuyển hướng về trang quản lý hình ảnh, giữ nguyên id phòng để người dùng có thể dễ dàng sửa lỗi và tải lại ảnh
                return RedirectToAction(nameof(HinhAnh), new { phongTroId });
            }
            //extension sẽ trả về phần đuôi của file, ví dụ: .jpg, .png, ... để kiểm tra xem có phải là file ảnh hợp lệ hay không
            var extension = Path.GetExtension(file.FileName);// Lấy đuôi của file
            //HinhAnhExtensions chưa các đuôi file ảnh hợp lệ được triển khai ở đầu controller
            if (string.IsNullOrEmpty(extension) || !HinhAnhExtensions.Contains(extension))
            {
                TempData["Error"] = $"Ảnh \"{file.FileName}\" không đúng định dạng (JPG, JPEG, PNG, WebP).";
                return RedirectToAction(nameof(HinhAnh), new { phongTroId });
            }

            fileHopLe.Add((file, extension.ToLowerInvariant()));//extension.ToLowerInvariant() chuyển đuôi file về chữ thường để đồng bộ và không bị lỗi
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
        //Path.Combine kết hợp tất cả tạo thành 1 đường dẫn uploads/phong-tro/{phongTroId} để lưu ảnh của phòng đó, tránh trùng lặp tên file giữa các phòng khác nhau
        var uploadDir = Path.Combine(
            _webHostEnvironment.WebRootPath,//đường dẫn tuyệt đối tới thư mục wwwroot
            "uploads",
            "phong-tro",
            phongTroId.ToString());
        //Tạo thư mục
        Directory.CreateDirectory(uploadDir);
        var duongDanDaLuu = new List<string>();
        //fileDaGhi
        var fileDaGhi = new List<string>();

        try
        {
            // duyệt qua từng mỗi phần tử trong fileHopLe (file đã có đuôi dạt yêu cầu: jpg, png, ...)
            foreach (var (file, extension) in fileHopLe)
            {
                var fileName = $"{Guid.NewGuid()}{extension}";
                //Path.Combine(uploadDir, fileName) -> tạo đường dẫn 
                var physicalPath = Path.Combine(uploadDir, fileName);
                await using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                //fileDaGhi
                fileDaGhi.Add(physicalPath);
                duongDanDaLuu.Add($"/uploads/phong-tro/{phongTroId}/{fileName}");
            }
            //Tiến hành lưu đường dẫn vào database
            var result = await _phongTroManagementRepository.ThemHinhAnhPhongAsync(
                userId, phongTroId, duongDanDaLuu);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                foreach (var path in fileDaGhi)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                TempData["Error"] = result.Message;
            }
        }
        catch
        {
            foreach (var path in fileDaGhi)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            TempData["Error"] = "Không thể tải ảnh lên. Vui lòng thử lại.";
        }

        return RedirectToAction(nameof(HinhAnh), new { phongTroId });
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
