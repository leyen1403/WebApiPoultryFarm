# WebApiPoultryFarm

API quản lý trang trại gia cầm — kiến trúc tách lớp: **Domain / Application / Infrastructure / Api / Share**.

> Repo này dùng **.NET** + **EF Core** + **MediatR**; swagger bật ở môi trường Development.
> Hash mật khẩu bằng **BCrypt**; phản hồi API theo chuẩn `ApiResponse<T>`.

---

## ✨ Tính năng hiện có

- Đăng ký tài khoản người dùng
- Đăng nhập người dùng
- Bao hàm lỗi/validation dưới dạng `ApiResponse<T>` thống nhất

> _Gợi ý roadmap_: Thêm JWT + Refresh Token, phân quyền (Authorization), CORS, logging (Serilog), HealthChecks, Unit Tests.

---

## 🧱 Kiến trúc & Dự án con

```
WebApiPoultryFarm.sln
├─ WebApiPoultryFarm/                  # API (entrypoint): Controllers, Program.cs, ApiResponse
│  └─ WebApiPoultryFarm.Api.csproj
├─ PoultryFarm.Application/            # Application layer: CQRS (Commands/Handlers), DI (MediatR)
│  └─ WebApiPoultryFarm.Application.csproj
├─ WebApiPoultryFarm.Domain/           # Domain layer: Entities, Interfaces (Repository)
│  └─ WebApiPoultryFarm.Domain.csproj
├─ WebApiPoultryFarm.Infrastructure/   # Infrastructure: EF Core DbContext, Repository triển khai
│  └─ WebApiPoultryFarm.Infrastructure.csproj
├─ WebApiPoultryFarm.Share/            # Shared: Exceptions, Helpers (Email, Password)
│  └─ WebApiPoultryFarm.Share.csproj
```

> ⚠️ Lưu ý: thư mục `WebApiPoultryFarm.Share/Exeptions/` nên đổi thành **`Exceptions`**.

### Các điểm DI/Middleware chính
- `WebApiPoultryFarm/Program.cs`:
  - `AddControllers()`, `AddEndpointsApiExplorer()`, `AddSwaggerGen()`
  - `app.UseSwagger()` + `app.UseSwaggerUI()` khi `IsDevelopment()`
  - `UseHttpsRedirection()` và `UseAuthorization()`
- `WebApiPoultryFarm/DependencyInjection.cs`:
  - `services.AddApplication()` (đăng ký MediatR)
  - `services.AddInfrastructure(configuration)` (DbContext + Repository)
  - Chuẩn hoá **InvalidModelState** → trả về `ApiResponse<string>.Fail(...)`

---

## 🛠 Yêu cầu hệ thống

- .NET SDK **8.x** (khuyến nghị) hoặc 7.x phù hợp với máy bạn
- SQL Server (mặc định; có thể thay bằng provider khác)
- (Tuỳ chọn) `dotnet-ef` để chạy migrations:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 🚀 Chạy nhanh (Development)

```bash
# 1) Khôi phục & build
dotnet restore
dotnet build --no-restore

# 2) Chạy API (hot reload)
dotnet watch run --project WebApiPoultryFarm/WebApiPoultryFarm.Api.csproj
```

- Swagger: `http://localhost:<port>/swagger` (tự bật ở Development)

---

## ⚙️ Cấu hình ứng dụng

Tạo/chỉnh `WebApiPoultryFarm/appsettings.Development.json`:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PoultryFarm;User Id=<user>;Password=<pass>;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*"
}
```

> 🔐 **Không commit** thông tin nhạy cảm (`Password`, keys…). Khi cần, dùng Secret Manager:
> ```bash
> dotnet user-secrets init --project WebApiPoultryFarm/WebApiPoultryFarm.Api.csproj
> dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>" --project WebApiPoultryFarm/WebApiPoultryFarm.Api.csproj
> ```

---

## 🗄 Database & Migrations (EF Core)

```bash
# Tạo migration
dotnet ef migrations add InitialCreate   --project WebApiPoultryFarm.Infrastructure   --startup-project WebApiPoultryFarm

# Áp dụng DB
dotnet ef database update   --project WebApiPoultryFarm.Infrastructure   --startup-project WebApiPoultryFarm
```

- `PoultryFarmDbContext` tại: `WebApiPoultryFarm.Infrastructure/Data/PoultryFarmDbContext.cs`
- Entity mẫu: `WebApiPoultryFarm.Domain/Entities/User.cs`

---

## 📚 API Endpoints

### Base URL
- `api/[controller]` → với `UserController` là `api/User`

### 1) Đăng ký
**POST** `api/User/register`

Request body (`CreateUserCommand`):
```json
{
  "userName": "johndoe",
  "password": "P@ssw0rd",
  "fullName": "John Doe",
  "email": "john@example.com"
}
```

Response (`ApiResponse<UserResponse>`):
```jsonc
{
  "success": true,
  "message": "Đăng ký thành công!",
  "data": {
    "id": 1,
    "userName": "johndoe",
    "fullName": "John Doe",
    "email": "john@example.com",
    "accessToken": "",
    "refreshToken": "",
    "createdAt": "2025-08-09T00:00:00Z",
    "updatedAt": "2025-08-09T00:00:00Z",
    "lastLoginAt": null
  }
}
```

### 2) Đăng nhập
**POST** `api/User/login`

Request body (`LoginUserCommand`):
```json
{
  "userName": "johndoe",
  "password": "P@ssw0rd"
}
```

Response (`ApiResponse<UserResponse>`):
```jsonc
{
  "success": true,
  "message": "Đăng nhập thành công!",
  "data": {
    "id": 1,
    "userName": "johndoe",
    "fullName": "John Doe",
    "email": "john@example.com",
    "accessToken": "",
    "refreshToken": "",
    "createdAt": "2025-08-09T00:00:00Z",
    "updatedAt": "2025-08-09T00:00:00Z",
    "lastLoginAt": "2025-08-09T00:00:00Z"
  }
}
```

### Lỗi & Validation (mặc định)
- Khi `ModelState` không hợp lệ, API trả về `400 Bad Request` dạng:
```json
{
  "success": false,
  "message": "Dữ liệu không hợp lệ",
  "errors": {
    "UserName": ["Username tối thiểu 5 ký tự"],
    "Password": ["Password tối thiểu 6 ký tự"]
  }
}
```

---

## 🔐 Bảo mật & Hash mật khẩu

- Hash/verify với **BCrypt** (`PasswordHelper`)
- (Roadmap) Bổ sung Authentication (JWT) + Refresh Token life cycle
- (Roadmap) Giới hạn CORS theo môi trường

---

## 🧪 Kiểm thử (gợi ý)

- Tạo project `WebApiPoultryFarm.Tests` dùng xUnit + FluentAssertions
- Test *handlers* (Application) độc lập với Infrastructure bằng *in-memory* repository hoặc EF InMemory

---

## 🧰 Scripts tiện dụng

```bash
# Format code
dotnet format

# Build Release
dotnet build -c Release

# Publish (linux-x64, framework-dependent)
dotnet publish WebApiPoultryFarm/WebApiPoultryFarm.Api.csproj -c Release -r linux-x64 --self-contained false -o out
```

---

## 🗺 Roadmap / TODO

- [ ] Đổi `Exeptions` → `Exceptions`
- [ ] Thêm JWT + Refresh Token, Authorize attribute
- [ ] CORS theo domain
- [ ] Logging bằng Serilog + enrichers
- [ ] HealthChecks `/health` `/ready`
- [ ] Unit/Integration Tests: User đăng ký/đăng nhập
- [ ] Seed dữ liệu mẫu

---

## 📄 License

Chọn giấy phép phù hợp (MIT/Apache-2.0/Proprietary). Mặc định: MIT.

---

## 👥 Đóng góp

PR/Issues được chào mừng. Vui lòng dùng Conventional Commits (`feat:`, `fix:`, `refactor:`) và `dotnet format` trước khi gửi PR.
