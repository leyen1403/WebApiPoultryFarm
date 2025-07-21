namespace WebApiPoultryFarm.Api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; } // Chỉ dùng khi thất bại

        public static ApiResponse<T> Ok(T? data = default, string? message = null)
            => new ApiResponse<T> { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message)
            => new ApiResponse<T> { Success = false, Message = message, Data = default };

        public static ApiResponse<T> Fail(string message, Dictionary<string, string[]> errors)
            => new ApiResponse<T> { Success = false, Message = message, Errors = errors, Data = default };
    }
}
