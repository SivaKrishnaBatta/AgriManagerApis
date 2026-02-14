namespace AgriManager.API.DTOs
{
    public class IncomeListDto
    {
        public int IncomeId { get; set; }

        public int CropId { get; set; }

        public string CropName { get; set; } = null!;

        public decimal? Quantity { get; set; }

        public decimal? PricePerUnit { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime SaleDate { get; set; }

        public string? Notes { get; set; }
    }
}
