namespace QLNhaTro.Helpers.Constants;

public static class YeuCauThueStatus
{
    public const string Moi = "MOI";
    public const string ChuTroDongY = "CHU_TRO_DONG_Y";
    public const string ChuTroTuChoi = "CHU_TRO_TU_CHOI";
    public const string NguoiThueHuy = "NGUOI_THUE_HUY";

    public const string DongY = ChuTroDongY;
    public const string TuChoi = ChuTroTuChoi;

    public static string GetDisplayName(string? trangThai)
    {
        return trangThai switch
        {
            Moi => "Mới",
            ChuTroDongY => "Đã đồng ý",
            ChuTroTuChoi => "Đã từ chối",
            NguoiThueHuy => "Người thuê hủy",
            _ => "Không xác định"
        };
    }

    public static bool CoTheXuLy(string? trangThai)
        => string.Equals(trangThai, Moi, StringComparison.OrdinalIgnoreCase);

    public static string GetBadgeClass(string? trangThai)
    {
        return trangThai switch
        {
            Moi => "rent-request-status rent-request-status--moi",
            ChuTroDongY => "rent-request-status rent-request-status--approved",
            ChuTroTuChoi => "rent-request-status rent-request-status--rejected",
            NguoiThueHuy => "rent-request-status rent-request-status--cancelled",
            _ => "rent-request-status rent-request-status--unknown"
        };
    }

    public static string GetTenantBadgeClass(string? trangThai) => GetBadgeClass(trangThai);
}
