namespace QLNhaTro.Helpers.Constants;

public static class NguoiDungStatus
{
    public const string HoatDong = "HOAT_DONG";
    public const string BiKhoa = "BI_KHOA";

    public static string GetDisplayName(string? trangThai)
    {
        return trangThai switch
        {
            HoatDong => "Hoạt động",
            BiKhoa => "Bị khóa",
            _ => "Không xác định"
        };
    }

    public static bool IsActive(string? trangThai)
    {
        return string.Equals(trangThai, HoatDong, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsLocked(string? trangThai)
    {
        return string.Equals(trangThai, BiKhoa, StringComparison.OrdinalIgnoreCase);
    }
}
