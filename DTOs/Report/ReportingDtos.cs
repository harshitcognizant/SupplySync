namespace SupplySync.DTOs.Report
{
    public class VendorPerformanceDto
    {
        public int VendorID { get; set; }
        public string VendorName { get; set; } = default!;
        public int TotalPurchaseOrders { get; set; }
        public int TotalDeliveredQuantity { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public double AverageDeliveryDelayDays { get; set; } // positive = late
    }

    public class DeliveryDelayDto
    {
        public int DeliveryID { get; set; }
        public int POID { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; } = default!;
        public DateTime DeliveryDate { get; set; }
        public int ExpectedDeliveryDays { get; set; }
        public int DelayDays { get; set; } // positive if late
    }

    public class ProcurementSpendingDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalSpent { get; set; }
        public int InvoiceCount { get; set; }
    }

    public class InventoryLevelDto
    {
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; } = default!;
        public string Item { get; set; } = default!;
        public int Quantity { get; set; }
    }

    public class InvoiceTurnaroundDto
    {
        public int InvoiceId { get; set; }
        public int POID { get; set; }
        public int VendorId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime ApprovedAt { get; set; }
        public double TurnaroundHours { get; set; }
    }
}