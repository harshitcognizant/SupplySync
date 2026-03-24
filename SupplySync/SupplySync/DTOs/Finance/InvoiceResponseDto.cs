namespace SupplySync.DTOs.Finance
{
    public class InvoiceResponseDto
    {
        public int InvoiceId { get; set; }
        public int POID { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
