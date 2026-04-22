namespace SupplySync.Constants.Enums
{
	public enum NotificationEvent
	{
		// Vendor lifecycle
		VendorApplicationSubmitted,
		VendorApplicationApproved,
		VendorApplicationRejected,
		VendorApprovedInternal,

		// Admin & user management
		UserAccountCreated,
		UserRoleChanged,

		// Contract & PO
		ContractCreated,
		PurchaseOrderCreated,

		// Delivery & Warehouse
		DeliveryMarkedDone,
		GoodsReceivedConfirmation,

		// Inventory
		InventoryUpdated,

		// Invoice & Finance
		InvoiceSubmitted,
		InvoicePendingApprovalReminder,
		InvoiceApproved,
		InvoiceRejected,
		PaymentCompleted,

		// Compliance & Audit
		ComplianceReviewStarted,
		CompliancePassed,
		ComplianceFailed
	}
}