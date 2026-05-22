using System.Globalization;

namespace QuanLyNhaTro.Helpers;

public static class FormatHelper
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string FormatGiaThue(decimal gia) =>
        $"{gia.ToString("N0", ViCulture)} đ";

    public static string FormatTienCoc(decimal tienCoc) =>
        $"{tienCoc.ToString("N0", ViCulture)} đ";

    public static string FormatDienTich(decimal dienTich)
    {
        if (dienTich % 1 == 0)
        {
            return $"{dienTich.ToString("N0", ViCulture)} m²";
        }

        return $"{dienTich.ToString("0.##", ViCulture)} m²";
    }

    public static string FormatSoNguoi(int? soNguoi) =>
        soNguoi.HasValue ? $"{soNguoi.Value} người" : "Chưa cập nhật";

    public static string FormatNgay(DateTime ngay) =>
        ngay.ToString("dd/MM/yyyy", ViCulture);

    public static string FormatNgay(DateTime? ngay) =>
        ngay.HasValue ? FormatNgay(ngay.Value) : "Chưa cập nhật";
}
