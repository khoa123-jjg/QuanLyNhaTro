using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QLNhaTro.Helpers;
using QLNhaTro.Models.NguoiThue.HoSo;
using NguoiThueEntity = QLNhaTro.Domain.NguoiThue;

namespace QLNhaTro.Repositories.NguoiThue;

public class NguoiThueHoSoRepository : INguoiThueHoSoRepository
{
    private readonly PhongTroDaNangContext _context;

    public NguoiThueHoSoRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }
    //Trả giao diện hồ sơ người thuê
    public async Task<NguoiThueHoSoViewModel?> GetHoSoAsync(string userId)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return null;
        }

        var nguoiDung = await _context.NguoiDungs
            .AsNoTracking()
            .Include(x => x.NguoiThue)
            .FirstOrDefaultAsync(x => x.Id == nguoiDungId);

        if (nguoiDung is null)
        {
            return null;
        }

        return new NguoiThueHoSoViewModel
        {
            ThongTin = new CapNhatHoSoNguoiThueViewModel
            {
                HoTen = nguoiDung.HoTen,
                Email = nguoiDung.Email,
                SoDienThoai = NormalizePhoneForDisplay(nguoiDung.SoDienThoai),
                NgheNghiep = nguoiDung.NguoiThue?.NgheNghiep,
                NhuCauThue = nguoiDung.NguoiThue?.NhuCauThue
            },
            DoiMatKhau = new DoiMatKhauNguoiThueViewModel()
        };
    }

    public async Task<(bool Success, string Message)> CapNhatHoSoAsync(string userId, CapNhatHoSoNguoiThueViewModel model)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        var nguoiDung = await _context.NguoiDungs
            .Include(x => x.NguoiThue)
            .FirstOrDefaultAsync(x => x.Id == nguoiDungId);

        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }
        // Kiểm tra email đã được sử dụng hay chưa
        var email = model.Email.Trim();
        var emailTrung = await _context.NguoiDungs.AnyAsync(x => x.Email == email && x.Id != nguoiDungId);
        if (emailTrung)
        {
            return (false, "Email đã được sử dụng.");
        }

        nguoiDung.HoTen = model.HoTen.Trim();
        nguoiDung.Email = email;
        nguoiDung.SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoai)
            ? null
            : NormalizePhoneForStorage(model.SoDienThoai);
        //Tách khoản trắng của số điện thoại
        nguoiDung.NgayCapNhat = DateTime.Now;
        // Tạo hồ sơ người thuê nếu chưa có, thông thường khi tạo tài khoản thì chưa có người thuê
        var nguoiThue = nguoiDung.NguoiThue;
        //Kiểm tra tài khoản người dùng này đã trên bảng người thuê hay chưa
        // Nếu chưa thì tạo
        if (nguoiThue is null)
        {
            // gán id và ngày tạo
            nguoiThue = new NguoiThueEntity
            {
                NguoiDungId = nguoiDung.Id,
                NgayTao = DateTime.Now
            };
            //add vào bảng người thuê
            _context.NguoiThues.Add(nguoiThue);
            nguoiDung.NguoiThue = nguoiThue;
        }
        // đưa thông tin trên web vào
        nguoiThue.NgheNghiep = string.IsNullOrWhiteSpace(model.NgheNghiep)
            ? null
            : model.NgheNghiep.Trim();
        nguoiThue.NhuCauThue = string.IsNullOrWhiteSpace(model.NhuCauThue)
            ? null
            : model.NhuCauThue.Trim();
        nguoiThue.NgayCapNhat = DateTime.Now;
        // Lưu thông tin
        await _context.SaveChangesAsync();
        return (true, "Cập nhật hồ sơ thành công.");
    }
    // Đổi  mật khẩu
    public async Task<(bool Success, string Message)> DoiMatKhauAsync(string userId, DoiMatKhauNguoiThueViewModel model)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == nguoiDungId);
        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        if (!PasswordHelper.VerifyPassword(model.MatKhauHienTai, nguoiDung.MatKhauHash))
        {
            return (false, "Mật khẩu hiện tại không đúng.");
        }

        if (model.MatKhauMoi != model.XacNhanMatKhauMoi)
        {
            return (false, "Xác nhận mật khẩu không khớp.");
        }

        nguoiDung.MatKhauHash = PasswordHelper.HashPassword(model.MatKhauMoi);
        nguoiDung.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Đổi mật khẩu thành công.");
    }

    private static string? NormalizePhoneForDisplay(string? phone)
    {
        return string.IsNullOrWhiteSpace(phone) ? null : phone;
    }

    private static string NormalizePhoneForStorage(string phone)
    {
        return phone.Trim();
    }
}
