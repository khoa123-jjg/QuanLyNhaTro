using System.Globalization;

namespace QLNhaTroV7.Helpers;

public static class FormatHelper // dùng để format tiền, diện tích, ngày tháng năm
{
    public static string FormatTien(decimal soTien)
    {
        return string.Format(
            new CultureInfo("vi-VN"),
            "{0:N0} đ",
            soTien);
    }

    public static string FormatDienTich(decimal dienTich)
    {
        return $"{dienTich:N1} m²";
    }

    public static string FormatNgay(DateTime? ngay)
    {
        if (ngay == null) return "-"; // Trả về dấu gạch ngang nếu ngày trống
        return ngay.Value.ToString("dd/MM/yyyy");
    }

    public static string FormatNgayGio(DateTime? ngay)
    {
        if (ngay == null) return "-";
        return ngay.Value.ToString("dd/MM/yyyy HH:mm");
    }
}