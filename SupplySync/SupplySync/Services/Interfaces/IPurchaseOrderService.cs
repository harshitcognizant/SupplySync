using SupplySync.DTOs.PurchaseOrder;

namespace SupplySync.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<int> CreateAsync(CreatePurchaseOrderRequestDto dto);
        Task<PurchaseOrderResponseDto?> GetByIdAsync(int poId);
        Task<PurchaseOrderResponseDto?> UpdateAsync(int poId, UpdatePurchaseOrderRequestDto dto);
        Task<bool> DeleteAsync(int poId);

        Task<PurchaseOrderListResponseDto> ListAsync(
            int? contractId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate
        );
    }
}