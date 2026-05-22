namespace QuanLyNhaTro.Helpers.Constants;

public static class DisplayStatus
{
    public const string HienThi = "HIEN_THI";

    public const string An = "AN";

    public static IReadOnlyCollection<string> Values { get; } =
    [
        HienThi,
        An
    ];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) && Values.Contains(status.Trim());
}
