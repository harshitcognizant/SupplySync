namespace SupplySync.DTOs.PurchaseOrder
{
    public class CreatePurchaseOrderRequestDto
    {
        public int ContractID { get; set; }
        public DateTime Date { get; set; }
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = "Draft";
    }

    public class UpdatePurchaseOrderRequestDto
    {
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PurchaseOrderResponseDto
    {
        public int POID { get; set; }
        public int ContractID { get; set; }
        public DateTime Date { get; set; }
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PurchaseOrderListResponseDto
    {
        public List<PurchaseOrderResponseDto> PurchaseOrders { get; set; } = new();
        public int TotalCount { get; set; }
    }
}