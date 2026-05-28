namespace QLNhaTro.Helpers.Constants;

public static class YeuCauThueStatus
{
    public const string Moi = "MOI";
    public const string DongY = "DONG_Y";
    public const string TuChoi = "TU_CHOI";

    public static string GetDisplayName(string? trangThai)
    {
        return trangThai switch
        {
            Moi => "Mới",
            DongY => "Chủ trọ đồng ý",
            TuChoi => "Chủ trọ từ chối",
            _ => "Không xác định"
        };
    }
}
