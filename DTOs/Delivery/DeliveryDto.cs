namespace SupplySync.DTOs.Delivery
{
    public class CreateDeliveryItemRequestDto
    {
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class CreateDeliveryRequestDto
    {
        public int POID { get; set; }
        public int VendorID { get; set; }
        public DateTime Date { get; set; }

        // legacy single-item support
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // preferred: submit multiple items
        public List<CreateDeliveryItemRequestDto>? Items { get; set; }

        public string Status { get; set; } = "Shipped";
    }

    public class UpdateDeliveryRequestDto
    {
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DeliveryResponseDto
    {
        public int DeliveryID { get; set; }
        public int POID { get; set; }
        public int VendorID { get; set; }
        public DateTime Date { get; set; }
        public string Item { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;

        // items returned as array (deserialize ItemsJson)
        public List<CreateDeliveryItemRequestDto>? Items { get; set; }
    }

    public class DeliveryListResponseDto
    {
        public List<DeliveryResponseDto> Deliveries { get; set; } = new();
        public int TotalCount { get; set; }
    }
}