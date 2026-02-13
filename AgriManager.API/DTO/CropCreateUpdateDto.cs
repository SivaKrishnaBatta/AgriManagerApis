using System;

namespace AgriManager.API.DTOs
{
    public class CropCreateUpdateDto
    {
        public int FarmId { get; set; }
        public int FieldId { get; set; }
        public string CropName { get; set; }
        public int CropStatusId { get; set; }
        public string? Season { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }

        public string? ExpectedYield { get; set; }
    }
}
