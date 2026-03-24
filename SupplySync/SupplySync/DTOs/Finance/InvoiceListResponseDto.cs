namespace SupplySync.DTOs.Finance
{
    public class InvoiceListResponseDto
    {
        public int InvoiceId { get; set; }
        public int POID { get; set; }
        public string VendorName { get; set; } 
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateOnly Date { get; set; }
    }
}
