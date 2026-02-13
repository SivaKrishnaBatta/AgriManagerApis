namespace AgriManager.API.DTOs
{
    public class ExpenseCreateUpdateDto
    {
        public int CropId { get; set; }
        public int CategoryId { get; set; }

        public decimal Amount { get; set; }

        // Frontend always uses DateTime
        public DateTime ExpenseDate { get; set; }

        public string? Notes { get; set; }
    }
}
