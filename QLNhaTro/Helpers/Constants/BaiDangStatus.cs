namespace QLNhaTro.Helpers.Constants;

public static class BaiDangStatus
{
    public const string Nhap = "NHAP";

    public const string ChoDuyet = "CHO_DUYET";

    public const string DaDuyet = "DA_DUYET";

    public const string BiTuChoi = "BI_TU_CHOI";

    public static IReadOnlyCollection<string> Values { get; } =
    [
        Nhap,
        ChoDuyet,
        DaDuyet,
        BiTuChoi
    ];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) && Values.Contains(status.Trim());

    public static string GetDisplayName(string? status)
    {
        return status switch
        {
            Nhap => "Nháp",
            ChoDuyet => "Chờ duyệt",
            DaDuyet => "Đã duyệt",
            BiTuChoi => "Bị từ chối",
            _ => "Không xác định"
        };
    }
}
