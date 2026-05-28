namespace QuanLyNhaTro.Helpers.Constants;

public static class NhaTroStatus
{
    public const string HoatDong = "HOAT_DONG";

    public const string TamAn = "TAM_AN";

    public const string NgungHoatDong = "NGUNG_HOAT_DONG";

    public const string MacDinh = HoatDong;

    public static IReadOnlyCollection<string> Values { get; } =
    [
        HoatDong,
        TamAn,
        NgungHoatDong
    ];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) && Values.Contains(status.Trim());
}
