namespace QLNhaTroV7.Helpers;

public static class DungchungHelper
{
    public static class VaiTro
    {
        public const string Admin = "ADMIN";
        public const string ChuTro = "CHU_TRO";
        public const string NguoiThue = "NGUOI_THUE";

        public static string GetName(string? value) => value switch
        {
            Admin => "Quản trị viên",
            ChuTro => "Chủ trọ",
            NguoiThue => "Người thuê",
            _ => "Không xác định"
        };
    }

    public static class TrangThaiTaiKhoan
    {
        public const string HoatDong = "HOAT_DONG";
        public const string BiKhoa = "BI_KHOA";
        public const string ChoXacThuc = "CHO_XAC_THUC";

        public static string GetName(string? value) => value switch
        {
            HoatDong => "Hoạt động",
            BiKhoa => "Bị khóa",
            ChoXacThuc => "Chờ xác thực",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            HoatDong => "success",
            BiKhoa => "danger",
            ChoXacThuc => "warning",
            _ => "secondary"
        };
    }

    public static class TrangThaiHoSoChuTro
    {
        public const string HoatDong = "HOAT_DONG";
        public const string TamKhoa = "TAM_KHOA";
        public const string ChoDuyet = "CHO_DUYET";

        public static string GetName(string? value) => value switch
        {
            HoatDong => "Hoạt động",
            TamKhoa => "Tạm khóa",
            ChoDuyet => "Chờ duyệt",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            HoatDong => "success",
            TamKhoa => "danger",
            ChoDuyet => "warning",
            _ => "secondary"
        };
    }

    public static class TrangThaiHienThi
    {
        public const string HienThi = "HIEN_THI";
        public const string An = "AN";

        public static string GetName(string? value) => value switch
        {
            HienThi => "Hiển thị",
            An => "Ẩn",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            HienThi => "success",
            An => "secondary",
            _ => "secondary"
        };
    }

    public static class TrangThaiNhaTro
    {
        public const string HoatDong = "HOAT_DONG";
        public const string TamAn = "TAM_AN";
        public const string NgungHoatDong = "NGUNG_HOAT_DONG";

        public static string GetName(string? value) => value switch
        {
            HoatDong => "Hoạt động",
            TamAn => "Tạm ẩn",
            NgungHoatDong => "Ngừng hoạt động",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            HoatDong => "success",
            TamAn => "warning",
            NgungHoatDong => "secondary",
            _ => "secondary"
        };
    }

    public static class TrangThaiPhong
    {
        public const string Trong = "TRONG";
        public const string DangThue = "DANG_THUE";
        public const string DangSua = "DANG_SUA";
        public const string TamAn = "TAM_AN";

        public static string GetName(string? value) => value switch
        {
            Trong => "Còn trống",
            DangThue => "Đang thuê",
            DangSua => "Đang sửa",
            TamAn => "Tạm ẩn",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            Trong => "success",
            DangThue => "primary",
            DangSua => "warning",
            TamAn => "secondary",
            _ => "secondary"
        };
    }

    public static class TrangThaiBaiDang
    {
        public const string Nhap = "NHAP";
        public const string ChoDuyet = "CHO_DUYET";
        public const string DaDuyet = "DA_DUYET";
        public const string TuChoi = "TU_CHOI";
        public const string An = "AN";

        public static string GetName(string? value) => value switch
        {
            Nhap => "Bản nháp",
            ChoDuyet => "Chờ duyệt",
            DaDuyet => "Đã đăng",
            TuChoi => "Bị từ chối",
            An => "Đã ẩn",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            Nhap => "secondary",
            ChoDuyet => "warning",
            DaDuyet => "success",
            TuChoi => "danger",
            An => "dark",
            _ => "secondary"
        };
    }

    public static class TrangThaiDatThue
    {
        public const string Moi = "MOI";
        public const string ChuTroDongY = "CHU_TRO_DONG_Y";
        public const string ChuTroTuChoi = "CHU_TRO_TU_CHOI";
        public const string NguoiThueHuy = "NGUOI_THUE_HUY";

        public static string GetName(string? value) => value switch
        {
            Moi => "Mới",
            ChuTroDongY => "Chủ trọ đồng ý",
            ChuTroTuChoi => "Chủ trọ từ chối",
            NguoiThueHuy => "Người thuê hủy",
            _ => "Không xác định"
        };

        public static string GetColor(string? value) => value switch
        {
            Moi => "warning",
            ChuTroDongY => "success",
            ChuTroTuChoi => "danger",
            NguoiThueHuy => "secondary",
            _ => "secondary"
        };
    }
}