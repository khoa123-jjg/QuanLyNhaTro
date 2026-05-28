using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers;
using QLNhaTro.Models.ChuTro.CaiDat;

namespace QLNhaTro.Repositories.ChuTro;

public class ChuTroTaiKhoanRepository : IChuTroTaiKhoanRepository
{
    private readonly PhongTroDaNangContext _context;

    public ChuTroTaiKhoanRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<ChuTroCaiDatViewModel?> GetCaiDatAsync(string userId)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return null;
        }
        // câu lênh cơ sở dữ liệu lấy thông tin
        var nguoiDung = await _context.NguoiDungs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == nguoiDungId);
        if (nguoiDung is null)
        {
            return null;
        }
        // trả về thông tin người dùng, kiểu ChuTroCaiDatViewModel lưu trong Model
        return new ChuTroCaiDatViewModel
        {
            ThongTin = new CapNhatThongTinViewModel
            {
                HoTen = nguoiDung.HoTen,
                Email = nguoiDung.Email,
                SoDienThoai = nguoiDung.SoDienThoai ?? string.Empty,
                AnhDaiDien = nguoiDung.AnhDaiDien
            }
        };
    }

    public async Task<(bool Success, string Message)> CapNhatThongTinAsync(string userId, CapNhatThongTinViewModel model)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return (false, "Không xác định được tài khoản hiện tại.");
        }

        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == nguoiDungId);
        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        var hoTen = model.HoTen.Trim();
        var email = model.Email.Trim();
        var soDienThoai = model.SoDienThoai.Trim().Replace(" ", string.Empty);

        if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(soDienThoai))
        {
            return (false, "Vui lòng nhập đầy đủ thông tin tài khoản.");
        }

        if (string.IsNullOrWhiteSpace(nguoiDung.MatKhauHash))
        {
            return (false, "Tài khoản chưa được cấu hình mật khẩu hợp lệ.");
        }

        var emailDaTonTai = await _context.NguoiDungs.AnyAsync(x => x.Email == email && x.Id != nguoiDungId);
        if (emailDaTonTai)
        {
            return (false, "Email đã được sử dụng bởi tài khoản khác.");
        }

        nguoiDung.HoTen = hoTen;
        nguoiDung.Email = email;
        nguoiDung.SoDienThoai = soDienThoai;
        nguoiDung.NgayCapNhat = DateTime.Now;

        await _context.SaveChangesAsync();
        return (true, "Cập nhật thông tin thành công.");
    }

    public async Task<(bool Success, string Message)> DoiMatKhauAsync(string userId, DoiMatKhauViewModel model)
    {
        if (!int.TryParse(userId, out var nguoiDungId))
        {
            return (false, "Không xác định được tài khoản hiện tại.");
        }

        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == nguoiDungId);
        if (nguoiDung is null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }

        if (string.IsNullOrWhiteSpace(nguoiDung.MatKhauHash) || !PasswordHelper.VerifyPassword(model.MatKhauHienTai, nguoiDung.MatKhauHash))
        {
            return (false, "Mật khẩu hiện tại không đúng.");
        }

        if (model.MatKhauMoi != model.XacNhanMatKhauMoi)
        {
            return (false, "Xác nhận mật khẩu không khớp.");
        }

        if (model.MatKhauMoi.Length < 6)
        {
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
        }

        nguoiDung.MatKhauHash = PasswordHelper.HashPassword(model.MatKhauMoi);
        nguoiDung.NgayCapNhat = DateTime.Now;
        await _context.SaveChangesAsync();

        return (true, "Đổi mật khẩu thành công.");
    }
}
