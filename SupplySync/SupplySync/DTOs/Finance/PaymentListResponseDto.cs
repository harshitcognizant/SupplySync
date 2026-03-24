namespace SupplySync.DTOs.Finance
{
    public class PaymentListResponseDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; }
    }
}
