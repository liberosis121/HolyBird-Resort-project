# HolyBird Resort Management System
HolyBird Resort Management System là một hệ thống quản lý việc đặt trả phòng tại HolyBird Resort.

### 🛠 Công nghệ sử dụng (Tech Stack)

Ngôn ngữ: C#.

Framework: .NET Framework 4.8 (WinForms).

Giao diện: Guna UI2 WinForms Components (tối ưu hóa giao diện hiện đại).

Cơ sở dữ liệu: Microsoft SQL Server.

Công nghệ kết nối: ADO.NET (SQL Connection, Command, DataAdapter, Transaction).

Quản lý mã nguồn: Git & GitHub.

### ✨ Tính năng chính (Key Features)

#### 👤 Dành cho Khách hàng

Quản lý thông tin tài khoản: Xem và cập nhật thông tin cá nhân của đoàn khách.

Quản lý đặt phòng: Theo dõi danh sách phòng đã đặt, thời gian nhận/trả phòng thực tế.

Tra cứu đặt phòng: Tra cứu danh sách phòng có sẵn, đặt phòng tùy thích.

Thanh toán đa phương thức: Hỗ trợ thanh toán qua Tiền mặt, Thẻ tín dụng và Ví điện tử (QR Code).

#### 👨‍💼 Dành cho Nhân viên

Quản lý phòng: Thêm, sửa thông tin phòng, hạng phòng và hình thức phòng.

Kiểm soát bồi thường: Ghi nhận hư hại tài sản cho từng chi tiết giao dịch (CTGD) trong phòng.

Xác nhận trả phòng: Kiểm tra và xác nhận trạng thái trả phòng cho từng phòng trước khi khách thanh toán.

Đăng ký đoàn và tạo tài khoản giúp khách hàng.

### 🔄 Luồng Logic Hệ thống (System Workflow)

Hệ thống vận hành dựa trên chuỗi trạng thái nghiêm ngặt để đảm bảo tính nhất quán dữ liệu:

Giai đoạn đặt phòng: Khách hàng đặt phòng, trạng thái khởi tạo là Chưa nhận phòng nếu chưa đến thời gian.

Giai đoạn lưu trú: * Đúng giờ nhận: Nút Nhận phòng hiển thị xanh.

Sau khi nhấn: Trạng thái chuyển sang Chưa trả phòng.

Giai đoạn hoàn tất:

Khách nhấn Trả phòng: Trạng thái chuyển sang Yêu cầu trả phòng.

Nhân viên kiểm kê & bồi thường: Trạng thái chuyển sang Đã trả phòng.

Thanh toán: Khách xem hóa đơn tổng hợp (Tiền phòng + Tiền bồi thường) và hoàn tất giao dịch.

### 🚀 Hướng dẫn cài đặt và khởi chạy

#### 1. Cấu hình Database

Mở SQL Server Management Studio (SSMS).

Tạo database mới tên là DB_HolyBird.

Chạy script SQL đính kèm trong thư mục Database/DB_HolyBird.sql để tạo bảng, triggers và stored procedures.

#### 2. Cấu hình Ứng dụng

Mở file solution HolyBirdResort.sln bằng Visual Studio 2022.

Mở file DAO/DBConnection.cs (hoặc App.config) và cập nhật chuỗi kết nối (Connection String) phù hợp với SQL Server của bạn.

Build solution (Ctrl + Shift + B).
