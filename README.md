# FUNews Management System

Phiên bản hoàn thiện — Hệ thống quản lý tin tức (ASP.NET Core Razor Pages + SignalR)

Đây là bản README hoàn chỉnh cho bài tập PRN222 — Assignment 02. Tài liệu mô tả nhanh chức năng, cách cài đặt, cấu trúc dự án và các bước kiểm thử cơ bản.

## Tổng quan

Ứng dụng cung cấp một hệ thống quản lý tin tức theo kiến trúc 3 lớp (Presentation / Business / Data) sử dụng Entity Framework Core và SignalR để cập nhật thời gian thực.

Ngôn ngữ giao diện: Razor Pages (ASP.NET Core). Database: SQL Server (database script: `FUNewsManagement.sql`).

## Tính năng chính

- Đăng nhập phân quyền (Admin / Staff / Lecturer)
- Quản lý bài viết: CRUD, gán tag, tìm kiếm, phân trang
- Quản lý chuyên mục (có cây phân cấp, khóa xóa khi có bài liên kết)
- Quản lý tài khoản (Admin)
- Báo cáo thống kê theo khoảng thời gian (Admin)
- Cập nhật thời gian thực cho các sự kiện Create/Update/Delete bằng SignalR
- Validation client + server

## Thông tin tài khoản mặc định

- Email: `admin@FUNewsManagementSystem.org`
- Password: `@@abc123@@`

> Lưu ý: tài khoản Admin này được cấu hình trong file `NguyenBinhAnRazorPages/appsettings.json`.

## Yêu cầu môi trường

- .NET SDK 10.0
- SQL Server 2012 trở lên (local hoặc remote)
- Visual Studio 2019/2022 hoặc VS Code

## Cài đặt & chạy nhanh (Quickstart)

1. Mở terminal tại thư mục gốc của repo.

2. Tạo cơ sở dữ liệu (nếu chưa có) bằng script SQL:

```powershell
sqlcmd -S .\\SQLEXPRESS -i FUNewsManagement.sql
```

Thay `.\\SQLEXPRESS` bằng tên server SQL của bạn nếu cần.

3. Mở solution `NguyenBinhAn_QE190061_A02.slnx` bằng Visual Studio hoặc chạy từ terminal:

```powershell
cd NguyenBinhAnRazorPages
dotnet build
dotnet run
```

4. Mở trình duyệt tại: `http://localhost:5295` (port có thể khác, xem log khi khởi chạy).

## Cấu hình kết nối

- Connection string nằm trong `NguyenBinhAnRazorPages/appsettings.json` (key `ConnectionStrings:DefaultConnection`).

Ví dụ:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\\\SQLEXPRESS;Database=FUNewsManagement;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

## Cấu trúc dự án (tổng quan)

- `NguyenBinhAnRazorPages/` — Razor Pages (presentation)
  - `Pages/` — Các trang Razor (Login, News, Categories, Accounts, Profile, Statistics)
  - `Hubs/NewsHub.cs` — SignalR hub

- `NguyenBinhAn_A02_Business/` — Services & Repositories (business logic)

- `NguyenBinhAn_A02_Data/` — EF Core models và `FUNewsManagementDbContext`

## Kiểm thử nhanh các chức năng chính

1. Mở trang Login tại `/Login` — đăng nhập bằng tài khoản Admin (xem trên). Kiểm tra chuyển hướng tới Dashboard.
2. Dùng tài khoản Admin để vào trang `Accounts` — tạo/sửa/xóa tài khoản (Admin chỉ xem/quan lý).
3. Dùng tài khoản Staff để tạo sửa xóa bài viết và chuyên mục. Kiểm tra:
   - Popup tạo/sửa hoạt động
   - Xác thực đầu vào hoạt động
   - Xóa chuyên mục bị khóa khi có bài liên quan
4. Mở hai trình duyệt: khi 1 user tạo bài mới, các client khác nhận sự kiện SignalR (`NewsCreated`) và cập nhật giao diện tự động.
5. Tạo báo cáo thống kê: vào `Statistics`, chọn `StartDate` / `EndDate`, sinh báo cáo; kết quả sắp xếp giảm dần theo ngày tạo.

## Cách kiểm tra đăng nhập Admin bằng lệnh (headless)

Bạn có thể sử dụng curl hoặc một script nhỏ để kiểm tra POST login. Lưu ý ứng dụng dùng anti-forgery token trên form, do đó cần GET trang login trước, lấy token rồi POST.

Ví dụ (PowerShell, minh họa):

```powershell
# $r = Invoke-WebRequest 'http://localhost:5295/Login'
# $token = ($r.Content -match 'name="__RequestVerificationToken" value="([^"]+)"' | Out-Null; $matches[1])
# $body = @{ __RequestVerificationToken = $token; Email = 'admin@FUNewsManagementSystem.org'; Password = '@@abc123@@'; RememberMe = 'false' }
# $resp = Invoke-WebRequest -Uri 'http://localhost:5295/Login' -Method Post -Body $body -WebSession $r.Session -MaximumRedirection 0 -ErrorAction SilentlyContinue
# Write-Output $resp.StatusCode
```

## Ghi chú kỹ thuật & kiểm tra lỗi

- Nếu ứng dụng bị lỗi khi khởi chạy, kiểm tra `appsettings.json` và connection string.
- Xem logs console để biết các truy vấn EF Core đã thực hiện (thông tin hữu ích để debug dữ liệu mẫu).
- Nếu gặp lỗi copy DLL khi build: đóng mọi process đang chạy dự án (Visual Studio / dotnet run) rồi build lại.

## Các tệp quan trọng cần biết

- File SQL tạo DB: `FUNewsManagement.sql`
- Startup / cấu hình: `NguyenBinhAnRazorPages/Program.cs`
- Hub SignalR: `NguyenBinhAnRazorPages/Hubs/NewsHub.cs`
- Login page: `NguyenBinhAnRazorPages/Pages/Login.cshtml[.cs]`
- News management: `NguyenBinhAnRazorPages/Pages/News/Index.cshtml[.cs]`
- DbContext: `NguyenBinhAn_A02_Data/FUNewsManagementDbContext.cs`

## Tính năng có thể mở rộng (gợi ý)

- Upload ảnh cho bài viết, lưu file trên blob hoặc hệ thống file
- Gửi email thông báo khi có bài mới
- Tích hợp API REST cho mobile
- Audit log cho thao tác người dùng

---
