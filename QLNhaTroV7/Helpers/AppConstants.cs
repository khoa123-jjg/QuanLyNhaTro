namespace QLNhaTroV7.Helpers;
// Helpers là nơi chứa các hàm hoặc hằng số dùng chung cho toàn hệ thống.
// Chứa tên vai trò, trạng thái bài đăng, trạng thái phòng, trạng thái đặt thuê.
public static class AppConstants
{
    public static class VaiTro
    {
        public const string Admin = "ADMIN";
        public const string ChuTro = "CHU_TRO";
        public const string NguoiThue = "NGUOI_THUE";
    }

    public static class TrangThaiTaiKhoan
    {
        public const string HoatDong = "HOAT_DONG";
        public const string BiKhoa = "BI_KHOA";
    }

    public static class TrangThaiPhong
    {
        public const string Trong = "TRONG";
        public const string DangThue = "DANG_THUE";
        public const string TamAn = "TAM_AN";
    }

    public static class TrangThaiBaiDang
    {
        public const string Nhap = "NHAP";
        public const string ChoDuyet = "CHO_DUYET";
        public const string DaDuyet = "DA_DUYET";
        public const string TuChoi = "TU_CHOI";
        public const string An = "AN";
    }

    public static class TrangThaiDatThue
    {
        public const string Moi = "MOI";
        public const string ChuTroDongY = "CHU_TRO_DONG_Y";
        public const string ChuTroTuChoi = "CHU_TRO_TU_CHOI";
        public const string NguoiThueHuy = "NGUOI_THUE_HUY";
    }

    public static class TrangThaiHienThi
    {
        public const string HienThi = "HIEN_THI";
        public const string An = "AN";
    }
}