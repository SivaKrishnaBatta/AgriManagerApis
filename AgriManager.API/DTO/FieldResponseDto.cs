namespace AgriManager.API.DTOs
{
    public class FieldResponseDto
    {
        public int FieldId { get; set; }
        public int FarmId { get; set; }
        public string FarmName { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Notes { get; set; }
    }
}

