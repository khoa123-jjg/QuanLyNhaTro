using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QuanLyNhaTro.Helpers.Constants;
using QLNhaTro.Models.PhongTro;
using QuanLyNhaTro.Models.TienNghi;
using TienNghiEntity = QLNhaTro.Domain.TienNghi;
namespace QLNhaTro.Repositories.PhongTro;

public class PhongTroManagementRepository : IPhongTroManagementRepository
{
    private readonly PhongTroDaNangContext _context;

    public PhongTroManagementRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }
    // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown chọn nhà trọ khi tìm kiếm được đưa lên controller để dùng
    public async Task<List<SelectListItem>> GetDanhSachNhaTroCuaChuTroAsync(string userId)
    {
        // userId do hệ thống xác thực cung cấp thường là kiểu chuỗi, cần trả về int để truy vấn
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return [];
        }
        //Truy vấn danh sách nhà trọ lấy kèm id để xử lý khi chon option đưa lên database
        return await _context.NhaTros
            .AsNoTracking()
            .Where(n => n.ChuNhaTro.NguoiDungId == nguoiDungId)
            .OrderBy(n => n.TenNhaTro)
            .Select(n => new SelectListItem(n.TenNhaTro, n.Id.ToString()))// kèm id để khi chọn option sẽ lấy được id đưa lên database
            .ToListAsync();
    }
    // Lấy danh sách phòng theo tìm kiếm và lọc, nếu có, và lấy dữ liệu để đổ vào dropdown chọn nhà trọ, tầng, trạng thái khi quản lý phòng
    public async Task<PhongTroListPageViewModel> GetDanhSachPhongAsync(
        string userId,
        string? tuKhoa,
        int? nhaTroId,
        int? tang,
        string? trangThai)
    {
        var page = new PhongTroListPageViewModel
        {
            TuKhoa = tuKhoa,
            NhaTroId = nhaTroId,
            Tang = tang,
            TrangThai = trangThai
        };
        // userId do hệ thống xác thực cung cấp thường là kiểu chuỗi, cần trả về int để truy vấn
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return page;
        }
        // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown chọn nhà trọ khi quản lý phòng
        page.DanhSachNhaTro = await GetDanhSachNhaTroCuaChuTroAsync(userId);

        var tangValues = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Where(p => p.Tang != null)
            .Select(p => p.Tang!.Value)// ép kiểu int
            .Distinct()// Loại bỏ trùng lặp nếu có 5 phòng ở tầng 1 thì chỉ lấy giá trị 1 một lần nếu không sẽ có 5 option tầng 1 trong dropdown
            .OrderBy(t => t)
            .ToListAsync();

            page.DanhSachTang = tangValues
            .Select(t => new SelectListItem($"Tầng {t}", t.ToString()))
            .ToList();

        var query = _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);
        //nhaTroId nếu có thì thêm vào truy vấn, không có thì bỏ qua
        if (nhaTroId is > 0)
        {
            query = query.Where(p => p.NhaTroId == nhaTroId.Value);
        }
        //tang nếu có thì thêm vào truy vấn, không có thì bỏ qua
        if (tang.HasValue)
        {
            query = query.Where(p => p.Tang == tang.Value);
        }
        //trangThai là các điều kiện lọc, nếu có thì thêm vào truy vấn, không có thì bỏ qua
        if (!string.IsNullOrWhiteSpace(trangThai))
        {
            //Helper/Constants/PhongTroStatus là Helper dùng chung định nghĩa các trạng thái của phòng trọ được lưu trong Constants
            var trangThaiLoc = trangThai.Trim();
            if (PhongTroStatus.IsValid(trangThaiLoc))
            {
                query = query.Where(p => p.TrangThai == trangThaiLoc);
            }
        }

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(p =>
                p.MaPhong.Contains(keyword) ||
                p.TenPhong.Contains(keyword) ||
                (p.GhiChu != null && p.GhiChu.Contains(keyword)));
        }
        //Bắt đầu đưa ra kết quả sau khi đã lọc xong, sắp xếp theo tên nhà trọ, tầng, mã phòng
        page.DanhSachPhong = await query
            .OrderBy(p => p.NhaTro.TenNhaTro)
            .ThenBy(p => p.Tang)
            .ThenBy(p => p.MaPhong)
            .Select(p => new PhongTroListItemViewModel
            {
                Id = p.Id,
                MaPhong = p.MaPhong,
                TenPhong = p.TenPhong,
                TenNhaTro = p.NhaTro.TenNhaTro,
                Tang = p.Tang,
                DienTich = p.DienTich,
                GiaThueThang = p.GiaThueThang,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa,
                TrangThai = p.TrangThai,
                GhiChu = p.GhiChu,
                MoTa = p.MoTa
            })
            .ToListAsync();

        return page;
    }
    // Chỉ trả về form không hộ trọ nhập dữ liệu hay cập nhật
    public async Task<PhongTroCreateUpdateViewModel?> GetPhongFormAsync(int? id, string userId)
    {
        // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown sửa nếu có id,  không thì thêm mới
        var danhSachNhaTro = await GetDanhSachNhaTroCuaChuTroAsync(userId);
        // Trường hợp không có id
        if (id is null or <= 0)
        {
            return new PhongTroCreateUpdateViewModel
            {
                TrangThai = PhongTroStatus.MacDinh,
                // Đưa danh sách nhà trọ vào để hiển thị trong dropdown chọn nhà trọ khi quản lý phòng
                DanhSachNhaTro = danhSachNhaTro
            };
        }
        //Không tìm thấy nhà trọ nào của chủ trọ
        if (danhSachNhaTro.Count == 0 || !TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        var row = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == id.Value)
            .Where(p => p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(p => new PhongTroCreateUpdateViewModel
            {
                Id = p.Id,
                NhaTroId = p.NhaTroId,
                MaPhong = p.MaPhong,
                TenPhong = p.TenPhong,
                Tang = p.Tang,
                DienTich = p.DienTich,
                GiaThueThang = p.GiaThueThang,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa,
                MoTa = p.MoTa,
                TrangThai = p.TrangThai,
                GhiChu = p.GhiChu
            })
            .FirstOrDefaultAsync();

        if (row is null)
        {
            return null;
        }
        // Lấy danh sách nhà trọ của chủ trọ để hiển thị trong dropdown sửa nếu có id,  không thì thêm mới
        row.DanhSachNhaTro = danhSachNhaTro;
        return row;
    }

    private static PhongTroManagementResult ThanhCong(string message) =>
        new() { Success = true, Message = message };

    private static PhongTroManagementResult ThatBai(string message) =>
        new() { Success = false, Message = message };
    // Hỗ trợ cho việc thêm mới và cập nhật phòng, biết chủ nhà trọ nào đang thêm phòng hoặc cập nhật
    private async Task<int?> LayChuNhaTroIdTheoNguoiDung(string userId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId))
        {
            return null;
        }

        return await _context.ChuNhaTros
            .AsNoTracking()
            .Where(c => c.NguoiDungId == nguoiDungId)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
    }
    //Tạo phòng
    public async Task<PhongTroManagementResult> CreatePhong(string userId, PhongTroCreateUpdateViewModel model)
    {
        var chuNhaTroId = await LayChuNhaTroIdTheoNguoiDung(userId);
        //asp-validation-summary
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }
        //asp-validation-summary
        //Helper/Constants/PhongTroStatus là Helper dùng chung định nghĩa các trạng thái của phòng trọ được lưu trong Constants
        if (!PhongTroStatus.IsValid(model.TrangThai))
        {
            return ThatBai("Trạng thái phòng không hợp lệ.");
        }
        //asp-validation-summary
        if (!await NhaTroThuocChu(chuNhaTroId.Value, model.NhaTroId))
        {
            return ThatBai("Nhà trọ không thuộc quyền quản lý của bạn.");
        }
        //asp-validation-summary
        var maPhong = model.MaPhong.Trim();
        if (await MaPhongDaTonTai(model.NhaTroId, maPhong, excludePhongId: null))
        {
            return ThatBai("Mã phòng đã tồn tại trong nhà trọ này.");
        }

        var now = DateTime.Now;
        var phong = new Domain.PhongTro
        {
            NhaTroId = model.NhaTroId,
            MaPhong = maPhong,
            TenPhong = model.TenPhong.Trim(),
            Tang = model.Tang,
            DienTich = model.DienTich,
            GiaThueThang = model.GiaThueThang,
            TienCoc = model.TienCoc,
            SoNguoiToiDa = model.SoNguoiToiDa,
            MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim(),
            TrangThai = model.TrangThai,
            GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim(),
            NgayTao = now,
            NgayCapNhat = now
        };

        _context.PhongTros.Add(phong);
        await _context.SaveChangesAsync();

        return ThanhCong("Thêm phòng thành công.");
    }
    //Sửa phòng
    public async Task<PhongTroManagementResult> UpdatePhongAsync(string userId,PhongTroCreateUpdateViewModel model)
    {
        if (model.Id <= 0)
        {
            return ThatBai("Mã phòng không hợp lệ.");
        }

        var chuNhaTroId = await LayChuNhaTroIdTheoNguoiDung(userId);
        if (chuNhaTroId is null)
        {
            return ThatBai("Không xác định được hồ sơ chủ trọ.");
        }
        //Helper/Constants/PhongTroStatus là Helper dùng chung định nghĩa các trạng thái của phòng trọ được lưu trong Constants
        if (!PhongTroStatus.IsValid(model.TrangThai))
        {
            return ThatBai("Trạng thái phòng không hợp lệ.");
        }

        if (!await NhaTroThuocChu(chuNhaTroId.Value, model.NhaTroId))
        {
            return ThatBai("Nhà trọ không thuộc quyền quản lý của bạn.");
        }

        var phong = await _context.PhongTros
            .Include(p => p.NhaTro)
            .FirstOrDefaultAsync(p =>
                p.Id == model.Id
                && p.NhaTro.ChuNhaTroId == chuNhaTroId.Value);

        if (phong is null)
        {
            return ThatBai("Không tìm thấy phòng hoặc bạn không có quyền sửa.");
        }

        var maPhong = model.MaPhong.Trim();
        if (await MaPhongDaTonTai(model.NhaTroId, maPhong, excludePhongId: model.Id))
        {
            return ThatBai("Mã phòng đã tồn tại trong nhà trọ này.");
        }

        phong.NhaTroId = model.NhaTroId;
        phong.MaPhong = maPhong;
        phong.TenPhong = model.TenPhong.Trim();
        phong.Tang = model.Tang;
        phong.DienTich = model.DienTich;
        phong.GiaThueThang = model.GiaThueThang;
        phong.TienCoc = model.TienCoc;
        phong.SoNguoiToiDa = model.SoNguoiToiDa;
        phong.MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim();
        phong.TrangThai = model.TrangThai;
        phong.GhiChu = string.IsNullOrWhiteSpace(model.GhiChu) ? null : model.GhiChu.Trim();
        phong.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();

        return ThanhCong("Cập nhật phòng thành công.");
    }

    private async Task<bool> NhaTroThuocChu(int chuNhaTroId, int nhaTroId) =>
        await _context.NhaTros
            .AsNoTracking()
            .AnyAsync(n => n.Id == nhaTroId && n.ChuNhaTroId == chuNhaTroId);

    private async Task<bool> MaPhongDaTonTai(int nhaTroId, string maPhong, int? excludePhongId)
    {
        var query = _context.PhongTros
            .AsNoTracking()
            .Where(p => p.NhaTroId == nhaTroId && p.MaPhong == maPhong);

        if (excludePhongId is > 0)
        {
            query = query.Where(p => p.Id != excludePhongId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<TienNghiPhongPageViewModel?> GetTienNghiPhongPageAsync(string userId, int phongTroId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId) || phongTroId <= 0)
        {
            return null;
        }
        //Lấy thông tin phòng để hiển thị ở đầu
        var phong = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == phongTroId && p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(p => new
            {
                p.Id,
                p.MaPhong,
                p.TenPhong,
                p.Tang,
                p.TrangThai,
                TenNhaTro = p.NhaTro.TenNhaTro
            })
            .FirstOrDefaultAsync();

        if (phong is null)
        {
            return null;
        }

        var daChonIds = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == phongTroId)
            .SelectMany(p => p.TienNghis.Select(t => t.Id))
            .ToListAsync();

        var daChonSet = daChonIds;
        //DisplayStatus.HienThi được triển khai trong Helper/Constants/DisplayStatus để định nghĩa các trạng thái hiển thị của tiện nghi, chỉ lấy những tiện nghi đang hiển thị để gắn cho phòng
        var danhSachTienNghi = await _context.TienNghis
            .AsNoTracking()
            .Where(t => t.TrangThai == DisplayStatus.HienThi)
            .OrderBy(t => t.TenTienNghi)
            .Select(t => new TienNghiCheckboxViewModel
            {
                Id = t.Id,
                TenTienNghi = t.TenTienNghi,
                // DaChon là kiểu bool nếu có trả về true nếu không trả về false
                DaChon = daChonSet.Contains(t.Id)
            })
            .ToListAsync();

        return new TienNghiPhongPageViewModel
        {
            PhongTroId = phong.Id,
            MaPhong = phong.MaPhong,
            TenPhong = phong.TenPhong,
            TenNhaTro = phong.TenNhaTro,
            TangHienThi = phong.Tang.HasValue ? $"Tầng {phong.Tang}" : null,
            TrangThaiHienThi = PhongTroStatus.GetDisplayName(phong.TrangThai),//trả về tên trạng thái dễ hiểu
            //DanhSachTienNghi chưa DaChon sẽ trả về false, đã chọn sẽ trả về true để hiển thị checkbox đã chọn hay chưa
            DanhSachTienNghi = danhSachTienNghi
        };
    }
    //tienNghiIds được chuyển vào từ name
    public async Task<PhongTroManagementResult> LuuTienNghiPhongAsync(string userId,int phongTroId,List<int>? tienNghiIds)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId) || phongTroId <= 0)
        {
            return new PhongTroManagementResult
            {
                Success = false,
                Message = "Không tìm thấy phòng hoặc bạn không có quyền gắn tiện nghi cho phòng này."
            };
        }

        var phong = await _context.PhongTros
            .Include(p => p.TienNghis)
            .Where(p => p.Id == phongTroId && p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .FirstOrDefaultAsync();

        if (phong is null)
        {
            return new PhongTroManagementResult
            {
                Success = false,
                Message = "Không tìm thấy phòng hoặc bạn không có quyền gắn tiện nghi cho phòng này."
            };
        }
        // Lọc ra những id tiện nghi hợp lệ, tránh trường hợp có id âm hoặc trùng lặp
        var idsHopLe = (tienNghiIds ?? [])
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        List<TienNghiEntity> tienNghiMoi = [];
        //Tiện nghi có trạng thái hiển thị -- DisplayStatus.HienThi trong helper/Constants/DisplayStatus để định nghĩa các trạng thái hiển thị của tiện nghi, chỉ lấy những tiện nghi đang hiển thị để gắn cho phòng
        if (idsHopLe.Count > 0)
        {
            tienNghiMoi = await _context.TienNghis
                .Where(t => idsHopLe.Contains(t.Id) && t.TrangThai == DisplayStatus.HienThi)
                .ToListAsync();
        }
        // Dọn sạch hết tất cả tiện nghi sau đó thêm lại
        phong.TienNghis.Clear();
        foreach (var tienNghi in tienNghiMoi)
        {
            phong.TienNghis.Add(tienNghi);
        }

        await _context.SaveChangesAsync();

        return new PhongTroManagementResult
        {
            Success = true,
            Message = $"Đã lưu {tienNghiMoi.Count} tiện nghi cho phòng."
        };
    }

    public async Task<HinhAnhPhongPageViewModel?> GetHinhAnhPhongPageAsync(string userId, int phongTroId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId) || phongTroId <= 0)
        {
            return null;
        }

        var phong = await _context.PhongTros
            .AsNoTracking()
            .Where(p => p.Id == phongTroId && p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId)
            .Select(p => new
            {
                p.Id,
                p.MaPhong,
                p.TenPhong,
                p.Tang,
                p.DienTich,
                p.TrangThai,
                TenNhaTro = p.NhaTro.TenNhaTro
            })
            .FirstOrDefaultAsync();

        if (phong is null)
        {
            return null;
        }

        var anhRows = await _context.HinhAnhs
            .AsNoTracking()
            .Where(h => h.PhongTroId == phongTroId)
            .OrderBy(h => h.ThuTuHienThi)
            .ThenBy(h => h.Id)
            .Select(h => new HinhAnhItemViewModel
            {
                Id = h.Id,
                DuongDanAnh = h.DuongDanAnh,
                LaAnhDaiDien = h.LaAnhDaiDien,
                ThuTuHienThi = h.ThuTuHienThi
            })
            .ToListAsync();

        return new HinhAnhPhongPageViewModel
        {
            PhongTroId = phong.Id,
            PhongDangChon = new HinhAnhPhongThongTinViewModel
            {
                Id = phong.Id,
                MaPhong = phong.MaPhong,
                TenPhong = phong.TenPhong,
                TenNhaTro = phong.TenNhaTro,
                Tang = phong.Tang,
                DienTich = phong.DienTich,
                TrangThai = phong.TrangThai,
                TrangThaiHienThi = PhongTroStatus.GetDisplayName(phong.TrangThai)
            },
            DanhSachAnh = anhRows
        };
    }

    public async Task<bool> PhongThuocChuTroAsync(int phongTroId, string userId)
    {
        if (!TryParseNguoiDungId(userId, out var nguoiDungId) || phongTroId <= 0)
        {
            return false;
        }

        return await _context.PhongTros
            .AsNoTracking()
            .AnyAsync(p => p.Id == phongTroId && p.NhaTro.ChuNhaTro.NguoiDungId == nguoiDungId);
    }

    public Task<int> DemSoAnhPhongAsync(int phongTroId) =>
        _context.HinhAnhs.AsNoTracking().CountAsync(h => h.PhongTroId == phongTroId);

    public async Task<PhongTroManagementResult> ThemHinhAnhPhongAsync(
        string userId,
        int phongTroId,
        IReadOnlyList<string> duongDanAnh)
    {
        if (duongDanAnh.Count == 0)
        {
            return new PhongTroManagementResult
            {
                Success = false,
                Message = "Không có ảnh hợp lệ để lưu."
            };
        }

        if (!await PhongThuocChuTroAsync(phongTroId, userId))
        {
            return new PhongTroManagementResult
            {
                Success = false,
                Message = "Phòng không tồn tại hoặc bạn không có quyền quản lý."
            };
        }
        //giời hạn số lượng 20 ảnh cho mỗi phòng để tránh việc chủ trọ tải lên quá nhiều
        var soAnhHienTai = await DemSoAnhPhongAsync(phongTroId);
        if (soAnhHienTai + duongDanAnh.Count > HinhAnhPhongPageViewModel.MaxAnhMoiPhong)
        {
            return new PhongTroManagementResult
            {
                Success = false,
                Message = $"Mỗi phòng tối đa {HinhAnhPhongPageViewModel.MaxAnhMoiPhong} ảnh."
            };
        }
        //tìm thứ tự cao nhất
        var maxThuTu = await _context.HinhAnhs
            .Where(h => h.PhongTroId == phongTroId)
            .Select(h => (int?)h.ThuTuHienThi)
            .MaxAsync() ?? 0;
        // xem có ảnh bìa hay chưa
        var coAnhBia = await _context.HinhAnhs
            .AnyAsync(h => h.PhongTroId == phongTroId && h.LaAnhDaiDien);

        var thuTu = maxThuTu;
        foreach (var duongDan in duongDanAnh)
        {
            thuTu++;
            // mục đích của đoạn code if này là nên đã có ảnh bìa rồi thì coAnhBia sẽ là true, tránh code bị lặp lại, tạo ra nhiều ảnh bìa
            var laAnhBia = !coAnhBia && thuTu == maxThuTu + 1;
            if (laAnhBia)
            {
                coAnhBia = true;
            }
            // thêm
            _context.HinhAnhs.Add(new HinhAnh
            {
                PhongTroId = phongTroId,
                DuongDanAnh = duongDan,
                LaAnhDaiDien = laAnhBia,
                ThuTuHienThi = thuTu,
                NgayTao = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        return new PhongTroManagementResult
        {
            Success = true,
            Message = $"Đã tải lên {duongDanAnh.Count} ảnh."
        };
    }

    private static bool TryParseNguoiDungId(string userId, out int nguoiDungId) =>
        int.TryParse(userId, out nguoiDungId) && nguoiDungId > 0;
}
