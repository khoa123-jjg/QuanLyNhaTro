namespace QuanLyNhaTro.Helpers.Constants;

public static class PhongTroStatus
{
    public const string Trong = "TRONG";

    public const string DangThue = "DANG_THUE";

    public const string DangSua = "DANG_SUA";

    public const string TamAn = "TAM_AN";

    public const string MacDinh = Trong;

    public static IReadOnlyCollection<string> Values { get; } =
    [
        Trong,
        DangThue,
        DangSua,
        TamAn
    ];
    //So sánh trạng thái
    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) && Values.Contains(status.Trim());
}
