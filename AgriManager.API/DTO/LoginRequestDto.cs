using System.ComponentModel.DataAnnotations;

namespace AgriManager.API.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "CustomerId is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
