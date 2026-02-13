namespace AgriManager.API.DTOs
{
    public class ExpenseListDto
    {
        public int ExpenseId { get; set; }

        public int CropId { get; set; }
        public string CropName { get; set; } = null!;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public decimal Amount { get; set; }

        // Sent back to frontend as DateTime
        public DateTime ExpenseDate { get; set; }

        public string? Notes { get; set; }
    }
}
