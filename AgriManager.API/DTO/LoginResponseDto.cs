namespace AgriManager.API.DTOs
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
