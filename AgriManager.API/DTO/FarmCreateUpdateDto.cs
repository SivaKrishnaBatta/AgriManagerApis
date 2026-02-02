using System.ComponentModel.DataAnnotations;

namespace AgriManager.API.DTOs
{
    public class FarmCreateUpdateDto
    {
        [Required]
        public string FarmName { get; set; } = null!;

        public string? Location { get; set; }

        public int? TotalFields { get; set; }

        public string? Notes { get; set; }
    }
}
