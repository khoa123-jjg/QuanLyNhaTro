namespace QLNhaTro.Helpers.Constants;

public static class TienNghiStatus
{
    public const string HienThi = "HIEN_THI";
    public const string An = "AN";

    public static bool IsValid(string? status)
    {
        return status == HienThi || status == An;
    }

    public static string GetDisplayName(string? status)
    {
        return status switch
        {
            HienThi => "Hiển thị",
            An => "Ẩn",
            _ => string.Empty
        };
    }
}
