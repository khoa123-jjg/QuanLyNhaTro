using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLNhaTro.Models.BaiDang
{
    public class BaiDangListPageViewModel
    {
        public string? TuKhoa { get; set; }

        public int? NhaTroId { get; set; }

        public string? TrangThaiDuyet { get; set; }

        public List<SelectListItem> DanhSachNhaTro { get; set; } = new();

        public List<SelectListItem> DanhSachTrangThai { get; set; } =
        [
            new SelectListItem("Chờ duyệt", "CHO_DUYET"),
            new SelectListItem("Đã duyệt", "DA_DUYET"),
            new SelectListItem("Bị từ chối", "BI_TU_CHOI"),
        ];

        public List<BaiDangListItemViewModel> DanhSachBaiDang { get; set; } = new();
    }
}
