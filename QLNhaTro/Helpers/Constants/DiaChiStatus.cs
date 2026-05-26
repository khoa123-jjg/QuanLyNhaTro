namespace QLNhaTro.Helpers.Constants;

public static class DiaChiStatus
{
    public const string HienThi = "HIEN_THI";
    public const string An = "AN";

    public static bool IsValid(string? status)
    {
        return status == HienThi || status == An;
    }
}
