using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Domain;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.Admin.DiaChi;

namespace QLNhaTro.Repositories.DiaChi;

public class AdminDiaChiRepository : IAdminDiaChiRepository
{
    private readonly PhongTroDaNangContext _context;

    public AdminDiaChiRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<XaPageViewModel> GetXaPageAsync(string? tuKhoa, int? quanHuyenId, int? id)
    {
        var query = _context.Xas
            .AsNoTracking()
            .Include(x => x.Quanhuyen)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(x => x.TenXahuyen.Contains(keyword));
        }

        if (quanHuyenId.HasValue)
        {
            query = query.Where(x => x.Quanhuyenid == quanHuyenId.Value);
        }

        var danhSachXa = await query
            .OrderBy(x => x.Id)
            .Select(x => new XaListItemViewModel
            {
                Id = x.Id,
                TenXa = x.TenXahuyen,
                QuanHuyenId = x.Quanhuyenid,
                TenQuanHuyen = x.Quanhuyen.TenQuanhuyen
            })
            .ToListAsync();

        var danhSachQuanHuyen = await _context.Quanhuyens
            .AsNoTracking()
            .Where(q => q.TrangThai == DiaChiStatus.HienThi)
            .OrderBy(q => q.TenQuanhuyen)
            .Select(q => new SelectListItem(q.TenQuanhuyen, q.Id.ToString()))
            .ToListAsync();

        var page = new XaPageViewModel
        {
            DanhSachXa = danhSachXa,
            DanhSachQuanHuyen = danhSachQuanHuyen,
            TuKhoa = tuKhoa,
            QuanHuyenId = quanHuyenId
        };

        if (id.HasValue)
        {
            var xa = await _context.Xas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
            if (xa is not null)
            {
                page.Form = new XaFormViewModel
                {
                    Id = xa.Id,
                    TenXa = xa.TenXahuyen,
                    QuanHuyenId = xa.Quanhuyenid
                };
            }
        }

        return page;
    }

    public async Task<(bool Success, string Message)> LuuXaAsync(XaFormViewModel form)
    {
        var tenXa = form.TenXa.Trim();
        if (string.IsNullOrWhiteSpace(tenXa))
        {
            return (false, "Vui lòng nhập tên phường/xã.");
        }

        if (!form.QuanHuyenId.HasValue)
        {
            return (false, "Vui lòng chọn quận/huyện.");
        }

        var quanHuyenTonTai = await _context.Quanhuyens.AnyAsync(q => q.Id == form.QuanHuyenId.Value);
        if (!quanHuyenTonTai)
        {
            return (false, "Quận/huyện không tồn tại.");
        }

        var trungTen = await _context.Xas.AnyAsync(x => x.TenXahuyen == tenXa && x.Id != form.Id);
        if (trungTen)
        {
            return (false, "Tên phường/xã đã tồn tại.");
        }

        if (form.Id.HasValue)
        {
            var xa = await _context.Xas.FirstOrDefaultAsync(x => x.Id == form.Id.Value);
            if (xa is null)
            {
                return (false, "Không tìm thấy phường/xã cần cập nhật.");
            }

            xa.TenXahuyen = tenXa;
            xa.Quanhuyenid = form.QuanHuyenId.Value;
        }
        else
        {
            _context.Xas.Add(new Xa
            {
                TenXahuyen = tenXa,
                Quanhuyenid = form.QuanHuyenId.Value,
                TrangThai = DiaChiStatus.HienThi
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Đã lưu phường/xã thành công.");
    }

    public async Task<(bool Success, string Message)> XoaXaAsync(int id)
    {
        var xa = await _context.Xas.FirstOrDefaultAsync(x => x.Id == id);
        if (xa is null)
        {
            return (false, "Không tìm thấy phường/xã.");
        }

        var daDuocSuDung = await _context.DuongPhos.AnyAsync(d => d.Xaid == id);
        if (daDuocSuDung)
        {
            return (false, "Phường/xã đang có đường phố, không thể xóa.");
        }

        _context.Xas.Remove(xa);
        await _context.SaveChangesAsync();
        return (true, "Đã xóa phường/xã thành công.");
    }

    public async Task<DuongPhoPageViewModel> GetDuongPhoPageAsync(string? tuKhoa, int? xaId, int? id)
    {
        var query = _context.DuongPhos
            .AsNoTracking()
            .Include(d => d.Xa)
            .ThenInclude(x => x.Quanhuyen)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(d => d.TenDuong.Contains(keyword));
        }

        if (xaId.HasValue)
        {
            query = query.Where(d => d.Xaid == xaId.Value);
        }

        var danhSachDuongPho = await query
            .OrderBy(d => d.Id)
            .Select(d => new DuongPhoListItemViewModel
            {
                Id = d.Id,
                TenDuong = d.TenDuong,
                XaId = d.Xaid,
                TenXa = d.Xa.TenXahuyen,
                TenQuanHuyen = d.Xa.Quanhuyen.TenQuanhuyen
            })
            .ToListAsync();

        var danhSachXa = await _context.Xas
            .AsNoTracking()
            .Where(x => x.TrangThai == DiaChiStatus.HienThi)
            .Include(x => x.Quanhuyen)
            .OrderBy(x => x.TenXahuyen)
            .Select(x => new SelectListItem($"{x.TenXahuyen} - {x.Quanhuyen.TenQuanhuyen}", x.Id.ToString()))
            .ToListAsync();

        var page = new DuongPhoPageViewModel
        {
            DanhSachDuongPho = danhSachDuongPho,
            DanhSachXa = danhSachXa,
            TuKhoa = tuKhoa,
            XaId = xaId
        };

        if (id.HasValue)
        {
            var duongPho = await _context.DuongPhos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id.Value);
            if (duongPho is not null)
            {
                page.Form = new DuongPhoFormViewModel
                {
                    Id = duongPho.Id,
                    TenDuong = duongPho.TenDuong,
                    XaId = duongPho.Xaid
                };
            }
        }

        return page;
    }

    public async Task<(bool Success, string Message)> LuuDuongPhoAsync(DuongPhoFormViewModel form)
    {
        var tenDuong = form.TenDuong.Trim();
        if (string.IsNullOrWhiteSpace(tenDuong))
        {
            return (false, "Vui lòng nhập tên đường.");
        }

        if (!form.XaId.HasValue)
        {
            return (false, "Vui lòng chọn phường/xã.");
        }

        var xaTonTai = await _context.Xas.AnyAsync(x => x.Id == form.XaId.Value);
        if (!xaTonTai)
        {
            return (false, "Phường/xã không tồn tại.");
        }

        var trungTen = await _context.DuongPhos.AnyAsync(d => d.Xaid == form.XaId.Value && d.TenDuong == tenDuong && d.Id != form.Id);
        if (trungTen)
        {
            return (false, "Tên đường đã tồn tại trong phường/xã này.");
        }

        if (form.Id.HasValue)
        {
            var duongPho = await _context.DuongPhos.FirstOrDefaultAsync(d => d.Id == form.Id.Value);
            if (duongPho is null)
            {
                return (false, "Không tìm thấy đường phố cần cập nhật.");
            }

            duongPho.TenDuong = tenDuong;
            duongPho.Xaid = form.XaId.Value;
        }
        else
        {
            _context.DuongPhos.Add(new DuongPho
            {
                TenDuong = tenDuong,
                Xaid = form.XaId.Value,
                TrangThai = DiaChiStatus.HienThi
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Đã lưu đường phố thành công.");
    }

    public async Task<(bool Success, string Message)> XoaDuongPhoAsync(int id)
    {
        var duongPho = await _context.DuongPhos.FirstOrDefaultAsync(d => d.Id == id);
        if (duongPho is null)
        {
            return (false, "Không tìm thấy đường phố.");
        }

        var daDuocSuDung = await _context.NhaTros.AnyAsync(n => n.DuongPhoId == id);
        if (daDuocSuDung)
        {
            return (false, "Đường phố đang được nhà trọ sử dụng, không thể xóa.");
        }

        _context.DuongPhos.Remove(duongPho);
        await _context.SaveChangesAsync();
        return (true, "Đã xóa đường phố thành công.");
    }

    public async Task<QuanHuyenPageViewModel> GetQuanHuyenPageAsync(string? tuKhoa, int? id)
    {
        var query = _context.Quanhuyens.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(q => q.TenQuanhuyen.Contains(keyword) || q.ThanhPho.Contains(keyword));
        }

        var list = await query
            .OrderBy(q => q.Id)
            .Select(q => new QuanHuyenListItemViewModel
            {
                Id = q.Id,
                TenQuanHuyen = q.TenQuanhuyen,
                ThanhPho = q.ThanhPho
            })
            .ToListAsync();

        var page = new QuanHuyenPageViewModel
        {
            DanhSachQuanHuyen = list,
            TuKhoa = tuKhoa
        };

        if (id.HasValue)
        {
            var item = await _context.Quanhuyens.AsNoTracking().FirstOrDefaultAsync(q => q.Id == id.Value);
            if (item is not null)
            {
                page.Form = new QuanHuyenFormViewModel
                {
                    Id = item.Id,
                    TenQuanHuyen = item.TenQuanhuyen,
                    ThanhPho = item.ThanhPho
                };
            }
        }

        return page;
    }

    public async Task<(bool Success, string Message)> LuuQuanHuyenAsync(QuanHuyenFormViewModel form)
    {
        var tenQuanHuyen = form.TenQuanHuyen.Trim();
        var thanhPho = form.ThanhPho.Trim();

        if (string.IsNullOrWhiteSpace(tenQuanHuyen))
        {
            return (false, "Vui lòng nhập tên quận/huyện.");
        }

        if (string.IsNullOrWhiteSpace(thanhPho))
        {
            return (false, "Vui lòng nhập tên thành phố.");
        }

        var trungTen = await _context.Quanhuyens.AnyAsync(q => q.TenQuanhuyen == tenQuanHuyen && q.Id != form.Id);
        if (trungTen)
        {
            return (false, "Tên quận/huyện đã tồn tại.");
        }

        if (form.Id.HasValue && form.Id.Value > 0)
        {
            var item = await _context.Quanhuyens.FirstOrDefaultAsync(q => q.Id == form.Id.Value);
            if (item is null)
            {
                return (false, "Không tìm thấy quận/huyện cần cập nhật.");
            }

            item.TenQuanhuyen = tenQuanHuyen;
            item.ThanhPho = thanhPho;
        }
        else
        {
            _context.Quanhuyens.Add(new Quanhuyen
            {
                TenQuanhuyen = tenQuanHuyen,
                ThanhPho = thanhPho,
                TrangThai = DiaChiStatus.HienThi
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Đã lưu quận/huyện thành công.");
    }

    public async Task<(bool Success, string Message)> XoaQuanHuyenAsync(int id)
    {
        var item = await _context.Quanhuyens.FirstOrDefaultAsync(q => q.Id == id);
        if (item is null)
        {
            return (false, "Không tìm thấy quận/huyện.");
        }

        var daDuocSuDung = await _context.Xas.AnyAsync(x => x.Quanhuyenid == id);
        if (daDuocSuDung)
        {
            return (false, "Quận/huyện đang có phường/xã trực thuộc, không thể xóa.");
        }

        _context.Quanhuyens.Remove(item);
        await _context.SaveChangesAsync();
        return (true, "Đã xóa quận/huyện thành công.");
    }
}
