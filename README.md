# Website Quản lý Phòng trọ Đà Nẵng
## Mô tả tổng quan
 Hệ thống web hỗ trợ người thuê tìm kiếm phòng trọ, chủ trọ quản lý thông tin nhà trọ/phòng trọ và quản trị viên kiểm duyệt bài đăng trước khi hiển thị công khai.
## Mục tiêu
Chuẩn hóa quy trình đăng phòng, tìm phòng và gửi yêu cầu thuê; giảm việc liên hệ thủ công rời rạc; giúp thông tin phòng trọ rõ ràng hơn về giá, diện tích, tiện nghi, hình ảnh và vị trí.
## Xác định vấn đề thực tế
Nhu cầu tìm phòng trọ tại Đà Nẵng phổ biến với sinh viên, người lao động và người mới chuyển đến thành phố. Tuy nhiên, quá trình tìm kiếm và quản lý phòng trọ còn nhiều bất cập nếu chỉ thực hiện qua mạng xã hội hoặc ghi chép thủ công.
### Người thuê
Thông tin phòng phân tán, khó kiểm chứng, thiếu ảnh và thông tin giá/tiện nghi thống nhất. Người thuê mất thời gian liên hệ nhiều chủ trọ để hỏi phòng còn trống hay không.
### Chủ trọ
Quản lý nhà trọ, loại phòng, phòng cụ thể và trạng thái phòng dễ nhầm lẫn khi ghi chép thủ công. Việc đăng tin nhiều nơi khiến khó theo dõi yêu cầu thuê.
### Quản trị viên
Cần kiểm soát nội dung bài đăng, tài khoản người dùng và danh mục dữ liệu để tránh tin sai, tin thiếu thông tin hoặc không phù hợp.
## Đối tượng sử dụng
Khách vãng lai, Thành viên, Người thuê, Chủ trọ, Quản trị viên
## Phân công nhiệm vụ
1. Phạm Nhật Khoa
   - Vai trò chính: Backend + Database
   - Công việc phụ trách: Thiết kế CSDL, tạo model, migration, phân quyền, controller lõi, xử lý nghiệp vụ yêu cầu thuê và bài đăng.
   - Sản phẩm bàn giao: ERD, database, model, controller, API/service chính.
2. Lê Hoài Nam
   - Vai trò chính: Giao diện User + Chủ trọ
   - Công việc phụ trách: Thiết kế và xây dựng giao diện trang chủ, tìm kiếm, chi tiết phòng, gửi yêu cầu thuê, dashboard chủ trọ, quản lý nhà trọ/phòng/bài đăng.
   - Sản phẩm bàn giao: Wireframe/UI user, UI chủ trọ, form nhập liệu, bảng danh sách.
3. Nguyễn Vương Trọng
   - Vai trò chính: Admin + Kiểm thử + Integration
   - Công việc phụ trách: Xây dựng giao diện admin, duyệt bài, quản lý người dùng, tiện nghi, địa chỉ; kiểm thử luồng; tổng hợp báo cáo và demo.
   - Sản phẩm bàn giao: Admin dashboard, module kiểm duyệt, test case, bản tích hợp cuối.

