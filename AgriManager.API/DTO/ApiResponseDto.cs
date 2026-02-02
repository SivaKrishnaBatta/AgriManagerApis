namespace AgriManager.API.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
    }
}

