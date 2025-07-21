namespace WebApiPoultryFarm.Share.Exeptions
{
    public class BusinessException : Exception
    {
        public string? Code { get; set; } // Mã lỗi nghiệp vụ, tuỳ chọn

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, string? code) : base(message)
        {
            Code = code;
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException) { }
    }
}
