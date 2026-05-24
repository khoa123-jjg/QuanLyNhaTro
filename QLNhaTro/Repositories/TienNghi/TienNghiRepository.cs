using Microsoft.EntityFrameworkCore;
using QLNhaTro.Data;
using QLNhaTro.Helpers.Constants;
using QLNhaTro.Models.Admin.TienNghi;
using TienNghiEntity = QLNhaTro.Domain.TienNghi;

namespace QLNhaTro.Repositories.TienNghi;

public class TienNghiRepository : ITienNghiRepository
{
    private readonly PhongTroDaNangContext _context;

    public TienNghiRepository(PhongTroDaNangContext context)
    {
        _context = context;
    }

    public async Task<TienNghiPageViewModel> GetPageAsync(int? id)
    {
        var danhSach = await _context.TienNghis
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new TienNghiListItemViewModel
            {
                Id = x.Id,
                TenTienNghi = x.TenTienNghi,
                TrangThai = x.TrangThai,
                TenTrangThai = TienNghiStatus.GetDisplayName(x.TrangThai),
                DangHienThi = x.TrangThai == TienNghiStatus.HienThi
            })
            .ToListAsync();

        var form = new TienNghiFormViewModel
        {
            TrangThai = TienNghiStatus.HienThi
        };

        if (id.HasValue)
        {
            var entity = await _context.TienNghis.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
            if (entity != null)
            {
                form.Id = entity.Id;
                form.TenTienNghi = entity.TenTienNghi;
                form.TrangThai = TienNghiStatus.IsValid(entity.TrangThai) ? entity.TrangThai : TienNghiStatus.HienThi;
            }
        }

        return new TienNghiPageViewModel
        {
            DanhSachTienNghi = danhSach,
            Form = form,
            TongSoTienNghi = danhSach.Count
        };
    }

    public async Task<bool> LuuTienNghiAsync(TienNghiFormViewModel model)
    {
        var tenTienNghi = (model.TenTienNghi ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(tenTienNghi) || !TienNghiStatus.IsValid(model.TrangThai))
        {
            return false;
        }

        if (model.Id.HasValue)
        {
            var entity = await _context.TienNghis.FirstOrDefaultAsync(x => x.Id == model.Id.Value);
            if (entity == null)
            {
                return false;
            }

            entity.TenTienNghi = tenTienNghi;
            entity.TrangThai = model.TrangThai;
        }
        else
        {
            _context.TienNghis.Add(new TienNghiEntity
            {
                TenTienNghi = tenTienNghi,
                TrangThai = model.TrangThai
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DoiTrangThaiAsync(int id)
    {
        var entity = await _context.TienNghis.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return false;
        }

        entity.TrangThai = entity.TrangThai == TienNghiStatus.HienThi ? TienNghiStatus.An : TienNghiStatus.HienThi;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Success, string Message)> XoaTienNghiAsync(int id)
    {
        var entity = await _context.TienNghis.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return (false, "Không tìm thấy tiện nghi.");
        }

        var dangDuocSuDung = await _context.PhongTros
            .AsNoTracking()
            .AnyAsync(x => x.TienNghis.Any(t => t.Id == id));

        if (dangDuocSuDung)
        {
            return (false, "Tiện nghi đang được sử dụng, hãy ẩn thay vì xóa.");
        }

        _context.TienNghis.Remove(entity);
        await _context.SaveChangesAsync();
        return (true, "Đã xóa tiện nghi thành công.");
    }
}
