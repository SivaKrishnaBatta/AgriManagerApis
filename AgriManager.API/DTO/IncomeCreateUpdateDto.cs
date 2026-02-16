namespace AgriManager.API.DTOs
{
    public class IncomeCreateUpdateDto
    {
        public int CropId { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? PricePerUnit { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime SaleDate { get; set; }



        public string? Notes { get; set; }
    }
}
