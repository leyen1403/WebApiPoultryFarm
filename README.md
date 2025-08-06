# PoultryFarmApi – Workflow Cơ Bản

1. **Client** (web, mobile, Postman, v.v...) gửi HTTP request tới API (ví dụ: đăng ký user, đăng nhập).

2. **Controller** (ví dụ: `UserController.cs` trong `WebApiPoultryFarm/Controllers`) nhận request, kiểm tra dữ liệu, chuyển thành Command/Query.

3. **Application Layer** (`PoultryFarm.Application`)

   * Controller tạo và gửi **Command** hoặc **Query** tới **Handler**.
   * **Command Handler** xử lý logic nghiệp vụ (validate, gọi repository, v.v...) và trả về **Response**.
   * Ví dụ: `CreateUserCommandHandler` nhận command, xử lý tạo mới user.

4. **Domain Layer** (`WebApiPoultryFarm.Domain`)

   * Chứa **Entities** (như `User.cs`) và **Interfaces** (như `IUserRepository.cs`).
   * Định nghĩa quy tắc nghiệp vụ và hợp đồng cho repository.

5. **Infrastructure Layer** (`WebApiPoultryFarm.Infrastructure`)

   * **Triển khai repository** (ví dụ: `UserRepository.cs` trong `Repositories/`), thao tác với DB qua `PoultryFarmDbContext`.
   * Thực hiện lưu, truy vấn, cập nhật dữ liệu.

6. **Kết quả** trả ngược lại qua các layer, từ handler → controller → client (HTTP response).

---

## Minh họa luồng "Đăng ký người dùng"

```plaintext
Client
  ↓
[UserController] → [CreateUserCommand] → [CreateUserCommandHandler] → [IUserRepository] → [UserRepository] → Database
  ↑                                                                                                         ↓
   ←-------------------------- Trả về kết quả tạo User thành công/thất bại ---------------------------------←
```

---

### Tóm tắt

* **Controllers** nhận request và chuyển giao cho Application Layer.
* **Application Layer** xử lý nghiệp vụ qua Command/Handler.
* **Domain Layer** định nghĩa thực thể và interface repository.
* **Infrastructure Layer** cài đặt lưu trữ, thao tác DB.
* **Response** trả ngược lại cho client.
