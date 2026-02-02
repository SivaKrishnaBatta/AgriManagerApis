using System.ComponentModel.DataAnnotations;

namespace AgriManager.API.DTOs
{
    public class FieldCreateUpdateDto
    {
        [Required(ErrorMessage = "Farm is required")]
        public int FarmId { get; set; }

        [Required(ErrorMessage = "Field name is required")]
        [StringLength(50, ErrorMessage = "Field name cannot exceed 50 characters")]
        public string FieldName { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Area cannot exceed 200 characters")]
        public string? Area { get; set; }

        [StringLength(200, ErrorMessage = "Notes cannot exceed 200 characters")]
        public string? Notes { get; set; }
    }
}

