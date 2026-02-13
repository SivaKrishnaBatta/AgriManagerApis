using System;

namespace AgriManager.API.DTOs
{
    public class CropListDto
    {
        public int CropId { get; set; }

        public int FarmId { get; set; }
        public string FarmName { get; set; }

        public int FieldId { get; set; }
        public string FieldName { get; set; }

        public string CropName { get; set; }

        public int CropStatusId { get; set; }
        public string CropStatusName { get; set; }

        public string? Season { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? ExpectedEndDate { get; set; }
        public string? ExpectedYield { get; set; }
    }
}
